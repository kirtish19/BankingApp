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
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(message.CustomerId)
                    ?? throw new InvalidDataException("Customer not found.");
                var loanApplication = await _unitOfWork.LoanApplicationRepository.GetByIdAsync(message.LoanApplicationId)
                    ?? throw new InvalidDataException("Loan application not found.");

                var (isValid, remarks) = ValidateDocuments(message.Documents);

                if (isValid)
                {
                    await CreateLoanDocumentRecords(message);
                    var riskAssesmentScore = CreditRiskAssesmentHelper.CalculateCustomerRisk(customer.CreditScore);
                    loanApplication.Status = CalculateLoanEligibility(loanApplication, riskAssesmentScore, customer, ref remarks);
                    if (loanApplication.Status == LoanStatus.Approved)
                    {
                        var interestRate = CalculateInterestRate(loanApplication.LoanType);
                        var monthlyEMI = CalculateMonthlyEMI(loanApplication.LoanAmount, loanApplication.TenureMonths, interestRate);
                        loanApplication.InterestRate = interestRate;
                        loanApplication.MonthlyEMI = monthlyEMI;
                    }
                    loanApplication.UpdatedDate = DateTime.Now;
                    await _unitOfWork.TransactionManager.SaveChangesAsync();
                }

                await DispatchNotificationEvent(message, loanApplication, remarks);

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
        private LoanStatus CalculateLoanEligibility(LoanApplications loanApplication, RiskAssesment riskAssesment, Customer customer, ref string remarks)
        {
            if (customer.AnnualIncome <= 0)
            {
                remarks = "Income criteria not met.";
                return LoanStatus.Rejected;
            }

            // Use enum values for employment type
            if (customer.EmploymentType == EmploymentType.Unemployed)
            {
                remarks = "Employment criteria not met.";
                return LoanStatus.Rejected;
            }

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
                if (customer.AnnualIncome >= requiredIncome)
                    return LoanStatus.Approved;
                else
                {
                    remarks = "Income criteria not met for Very High risk assessment.";
                    return LoanStatus.Rejected;
                }

            // For other risks: approve when income meets or exceeds required threshold
            if (customer.AnnualIncome >= requiredIncome)
                return LoanStatus.Approved;
            else
            {
                remarks = "Income criteria not met.";
                return LoanStatus.Rejected;
            }
        }
        private async Task DispatchNotificationEvent(LoanApplicationMessage message, LoanApplications loanApplication, string remarks)
        {
            LoanNotification loanNotification = new()
            {
                EventId = message.EventId,
                EventType = "LoanApplicationProcessed",
                EventTime = DateTime.Now,
                NotificationType = "LOAN",
                CustomerId = message.CustomerId,
                CustomerName = loanApplication.Customer!.FirstName + " " + loanApplication.Customer.LastName,
                Status = loanApplication.Status == LoanStatus.Approved ? nameof(LoanStatus.Approved) : nameof(LoanStatus.Rejected),
                Email = loanApplication.Customer.Email,
                MobileNumber = loanApplication.Customer.MobileNumber,
                Remarks = remarks,
                SourceSystem = "CustomerLoanProcessorFunction"
            };
            var additionalProperties = new Dictionary<string, object>
                    {
                        { nameof(LoanNotification.NotificationType), loanNotification.NotificationType },
                    };
            await _serviceBusHandler.SendMessageToQueueOrTopic(loanNotification,
                _configuration.GetValue<string>("NotificationTopicName")!,
                _configuration.GetValue<string>("ServiceBusWriter")!,
                additionalProperties);
        }
        private (bool, string) ValidateDocuments(List<BankingDocument> documents)
        {
            if (documents is null || documents.Count == 0)
                return (false, "Documents were not uploaded.");

            bool hasBankStatement = false;
            bool hasEmploymentLetter = false;
            bool hasSalarySlip = false;

            foreach (var document in documents)
            {
                var name = document?.DocumentName ?? string.Empty;
                if (name.Contains("BankStatement", StringComparison.OrdinalIgnoreCase))
                    hasBankStatement = true;
                if (name.Contains("EmploymentLetter", StringComparison.OrdinalIgnoreCase))
                    hasEmploymentLetter = true;
                if (name.Contains("SalarySlip", StringComparison.OrdinalIgnoreCase))
                    hasSalarySlip = true;
            }

            var missing = new List<string>();
            if (!hasBankStatement) missing.Add("BankStatement is not uploaded.");
            if (!hasEmploymentLetter) missing.Add("Employment Letter is not uploaded.");
            if (!hasSalarySlip) missing.Add("Salary Slip is not uploaded.");

            if (missing.Count == 0)
                return (true, "Loan application processed successfully.");

            return (false, string.Join(" ", missing));
        }

        private async Task CreateLoanDocumentRecords(LoanApplicationMessage message)
        {
            var loanDocuments = new List<LoanDocuments>();
            foreach (var document in message.Documents)
            {
                LoanDocuments loanDocument = new()
                {
                    Id = document.DocumentId,
                    CustomerId = message.CustomerId,
                    DocumentName = document.DocumentName,
                    BlobUrl = document.BlobUrl
                };
                loanDocuments.Add(loanDocument);
            }
            await _loanDocumentRepository.AddLoanDocumentRecords(loanDocuments);
        }
    }
}
