using BankingApp.Data.DocumentDb.Containers;

namespace BankingApp.CustomerKycProcessorFunction.Services
{
    public class MetaDataProcessorService(IUnitOfWork unitOfWork, IKycDocumentsRepository kycDocumentsRepository, IServiceBusHandler serviceBusHandler, IConfiguration configuration, ILogger<MetaDataProcessorService> logger) : IMetaDataProcessorService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IKycDocumentsRepository _kycDocumentsRepository = kycDocumentsRepository;
        private readonly IServiceBusHandler _serviceBusHandler = serviceBusHandler;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<MetaDataProcessorService> _logger = logger;

        public async Task ProcessMetaData(CustomerKYCMessage message)
        {
            bool KYCVerified;
            string KYCRemarks;
            try
            {
                (KYCVerified, KYCRemarks) = ValidateDocuments(message.Documents);

                var user = await _unitOfWork.UserRepository.GetUserByCustomerId(message.CustomerId);
                user.IsActive = KYCVerified;
                user.Customer!.Status = KYCVerified ? CustomerStatus.Active : CustomerStatus.Rejected;

                _logger.LogInformation("Writing to cosmos");

                await CreateKycRecords(message);

                _logger.LogInformation("Completed writting to cosmos");
                
                _logger.LogInformation("Calling method to send event to service bus");

                await DispatchNotificationEvent(message, KYCVerified, KYCRemarks, user);

                _logger.LogInformation("Message sent to service bus");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task DispatchNotificationEvent(CustomerKYCMessage message, bool KYCVerified, string KYCRemarks, Data.BankingDb.Tables.User user)
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
