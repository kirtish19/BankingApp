namespace BankingApp.CustomerKycProcessorFunction.Services
{
    public class MetaDataProcessorService(IUnitOfWork unitOfWork, IKycDocumentsRepository kycDocumentsRepository, IServiceBusHandler serviceBusHandler, IConfiguration configuration) : IMetaDataProcessorService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IKycDocumentsRepository _kycDocumentsRepository = kycDocumentsRepository;
        private readonly IServiceBusHandler _serviceBusHandler = serviceBusHandler;
        private readonly IConfiguration _configuration = configuration;

        public async Task ProcessMetaData(CustomerKYCMessage message)
        {
            bool KYCVerified;
            string KYCRemarks;
            (KYCVerified, KYCRemarks) = ValidateDocuments(message.Documents);

            var user = await _unitOfWork.UserRepository.GetUserByCustomerId(message.CustomerId);
            user.IsActive = KYCVerified;
            user.Customer!.Status = KYCVerified ? CustomerStatus.Active : CustomerStatus.Rejected;
            await _unitOfWork.TransactionManager.SaveChangesAsync();
            await CreateKycRecords(message);

            KycNotification kycNotification = new()
            {
                EventId = message.EventId,
                EventType = "KYCVerificationCompleted",
                EventTime = DateTime.Now,
                NotificationType = "KYC",
                CustomerId = message.CustomerId,
                CustomerName = user.Customer.FirstName + " " + user.Customer.LastName,
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
                    DocumentId = kycdocument.DocumentId,
                    CustomerId = message.CustomerId,
                    DocumentName = kycdocument.DocumentName,
                    BlobUrl = kycdocument.BlobUrl
                };
                kycRecords.Add(document);
            }
            await _kycDocumentsRepository.AddKycRecords(kycRecords);
        }

        private (bool, string) ValidateDocuments(List<CustomerKYCDocument> documents)
        {
            bool validated = false;
            string validationRemarks = "KYC verification completed successfully";
            if (documents is null || documents.Count != 2)
                return (validated, "Documents were not uploaded.");

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
            return (validated, validationRemarks);
        }
    }
}
