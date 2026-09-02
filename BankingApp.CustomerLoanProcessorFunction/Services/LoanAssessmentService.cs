using BankingApp.Data.BankingDb.Tables;
using BankingApp.Data.DocumentDb.Containers;
using BankingApp.Shared.Constants.Enums;

namespace BankingApp.CustomerLoanProcessorFunction.Services
{
    public class LoanAssessmentService(IUnitOfWork unitOfWork, ILoanDocumentRepository loanDocumentRepository, IServiceBusHandler serviceBusHandler, IConfiguration configuration, ILogger<LoanAssessmentService> logger) : ILoanAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILoanDocumentRepository _loanDocumentRepository = loanDocumentRepository;
        private readonly IServiceBusHandler _serviceBusHandler = serviceBusHandler;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<LoanAssessmentService> _logger = logger;

        public async Task ProcessLoanApplication(LoanApplicationMessage message)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(message.CustomerId) ?? throw new InvalidDataException("Customer not found.");
                var loanApplication = await _unitOfWork.LoanApplicationRepository.GetByIdAsync(message.LoanApplicationId) ?? throw new InvalidDataException("Loan application not found.");
                
                var riskAssesmentScore = CreditRiskAssesmentHelper.CalculateCustomerRisk(customer.CreditScore);                
                var loanStatus = CalculateLoanEligibility(loanApplication, riskAssesmentScore, customer);
                var interestRate = CalculateInterestRate(loanApplication.LoanType);
                var monthlyEMI = CalculateMonthlyEMI(loanApplication.LoanAmount, loanApplication.TenureMonths, interestRate);
                loanApplication.InterestRate = interestRate;
                loanApplication.MonthlyEMI = monthlyEMI;
                await _unitOfWork.TransactionManager.SaveChangesAsync();



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private decimal CalculateInterestRate(LoanType loanType)
        {
            // Return annual interest rate as a decimal (e.g. 0.065 = 6.5%) based on loan type
            return loanType switch
            {
                LoanType.Home => 0.065m,
                LoanType.Vehicle => 0.08m,
                LoanType.Personal => 0.12m,
                LoanType.Education => 0.05m,
                _ => 0.10m
            };
        }
        private decimal CalculateMonthlyEMI(decimal loanAmount, int tenureInMonths, decimal interestRate)
        {
            if (tenureInMonths <= 0 || loanAmount <= 0)
                return 0m;

            // interestRate is annual (e.g., 0.06 for 6%). Convert to monthly rate as double for Math.Pow.
            double monthlyRate = (double)interestRate / 12.0;

            if (Math.Abs(monthlyRate) < double.Epsilon)
            {
                // No interest loan
                return decimal.Divide(loanAmount, tenureInMonths);
            }

            double p = (double)loanAmount;
            double factor = Math.Pow(1.0 + monthlyRate, tenureInMonths);
            double emi = p * monthlyRate * factor / (factor - 1.0);

            return (decimal)emi;
        }
        private LoanStatus CalculateLoanEligibility(LoanApplications loanApplication, RiskAssesment riskAssesment, Customer customer)
        {
            if (customer.AnnualIncome <= 0)
                return LoanStatus.Rejected;

            // Use enum values for employment type
            if (customer.EmploymentType == EmploymentType.Unemployed)
                return LoanStatus.Rejected;

            decimal baseThreshold = customer.EmploymentType switch
            {
                EmploymentType.Salaried => 25_000m,
                EmploymentType.SelfEmployed => 40_000m,
                _ => 30_000m
            };

            // Risk adjustments
            decimal riskMultiplier = riskAssesment switch
            {
                RiskAssesment.Low => 1.0m,
                RiskAssesment.Medium => 1.5m,
                RiskAssesment.High => 2.0m,
                RiskAssesment.VeryHigh => 3.0m,
                _ => 1.0m
            };

            // Loan type adjustments using enum values
            decimal loanMultiplier = loanApplication.LoanType switch
            {
                LoanType.Home => 2.0m,
                LoanType.Vehicle => 1.2m,
                LoanType.Personal => 1.0m,
                LoanType.Education => 0.5m,
                _ => 1.0m
            };

            // Factor in loan amount: larger loans require proportionally higher income, capped to avoid extreme values
            decimal loanAmountFactor = 1.0m;
            if (loanApplication.LoanAmount > 0)
            {
                // scale: every 100k increases factor by 1. Cap between 0.5 and 3.0
                var raw = loanApplication.LoanAmount / 100_000m;
                loanAmountFactor = Math.Min(Math.Max(raw, 0.5m), 3.0m);
            }

            // Compute required income factoring employment baseline, risk, loan type and loan amount
            decimal requiredIncome = baseThreshold * riskMultiplier * loanMultiplier * loanAmountFactor;

            // VeryHigh risk: require especially strong income; otherwise reject
            if (riskAssesment == RiskAssesment.VeryHigh)
                return customer.AnnualIncome >= requiredIncome ? LoanStatus.Approved : LoanStatus.Rejected;

            // For other risks: approve when income meets or exceeds required threshold
            return customer.AnnualIncome >= requiredIncome ? LoanStatus.Approved : LoanStatus.Rejected;
        }
        private async Task DispatchNotificationEvent(LoanApplicationMessage message, bool KYCVerified, string KYCRemarks, Data.BankingDb.Tables.User user)
        {
            KycNotification kycNotification = new()
            {
                EventId = message.EventId,
                EventType = "KYCVerificationCompleted",
                EventTime = DateTime.Now,
                NotificationType = "KYC",
                CustomerId = message.CustomerId,
                CustomerName = user.Customer!.FirstName + " " + user.Customer.LastName,
                Status = KYCVerified ? "KYCVerified" : "KYCRejected",
                Email = user.Customer.Email,
                MobileNumber = user.Customer.MobileNumber,
                Remarks = KYCRemarks,
                SourceSystem = "CustomerKycProcessorFunction"
            };
            var additionalProperties = new Dictionary<string, object>
                    {
                        { nameof(KycNotification.NotificationType), kycNotification.NotificationType },
                    };
            await _serviceBusHandler.SendMessageToQueueOrTopic(kycNotification,
                _configuration.GetValue<string>("NotificationTopicName")!,
                _configuration.GetValue<string>("ServiceBusWriter")!,
                additionalProperties);
        }

        private async Task CreateKycRecords(CustomerKYCMessage message)
        {
            var kycRecords = new List<KycDocument>();
            foreach (var kycdocument in message.Documents)
            {
                KycDocument document = new()
                {
                    Id = kycdocument.DocumentId,
                    CustomerId = message.CustomerId,
                    DocumentName = kycdocument.DocumentName,
                    BlobUrl = kycdocument.BlobUrl
                };
                kycRecords.Add(document);
            }
            await _kycDocumentsRepository.AddKycRecords(kycRecords);
        }

        private (bool, string) ValidateDocuments(List<BankingDocument> documents)
        {
            bool validated = false;
            string validationRemarks = string.Empty;
            if (documents is null || documents.Count != 2)
                return (validated, "Documents were not uploaded.");
            //TODO - fix below logic
            foreach (var document in documents)
            {
                if (document.DocumentName.Contains("PAN", StringComparison.OrdinalIgnoreCase))
                    validated = true;
                else
                {
                    validated = false;
                    validationRemarks = "PAN Verificaiton Failed.";
                }

                if (document.DocumentName.Contains("Aadhar", StringComparison.OrdinalIgnoreCase))
                    validated = true;
                else
                {
                    validated = false;
                    validationRemarks = "Aadhar Verificaiton Failed.";
                }
            }

            validationRemarks = validated ? "KYC verification completed successfully" : validationRemarks;
            return (validated, validationRemarks);
        }
    }
}
