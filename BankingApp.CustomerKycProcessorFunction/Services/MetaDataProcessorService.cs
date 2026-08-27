using BankingApp.Data.BankingDb.Repository;
using BankingApp.Data.DocumentDb.Container;
using BankingApp.Data.DocumentDb.Repository;
using BankingApp.Shared.Constants.Enums;
using BankingApp.Shared.Models;

namespace BankingApp.CustomerKycProcessorFunction.Services
{
    public class MetaDataProcessorService(IUnitOfWork unitOfWork, IKycDocumentsRepository kycDocumentsRepository) : IMetaDataProcessorService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IKycDocumentsRepository _kycDocumentsRepository = kycDocumentsRepository;
        public async Task ProcessMetaData(CustomerKYCMessage message)
        {
            bool KYCVerified = ValidateDocuments(message.Documents);

            var user = await _unitOfWork.Users.GetUserByCustomerId(message.CustomerId);
            user.IsActive = KYCVerified;
            user.Customer!.Status = KYCVerified ? CustomerStatus.Active : CustomerStatus.Rejected;
            await _unitOfWork.TransactionManager.SaveChangesAsync();

            var kycRecords = new List<KycDocument>();
            foreach(var kycdocument in message.Documents)
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
        private bool ValidateDocuments(List<CustomerKYCDocument> documents)
        {
            bool validated = false;
            if (documents is null || documents.Count != 2)
                return validated;

            foreach (var document in documents)
            {
               if (document.DocumentName.Contains("PAN", StringComparison.OrdinalIgnoreCase) 
                    || document.DocumentName.Contains("Aadhar", StringComparison.OrdinalIgnoreCase))
                    validated = true;
               else validated = false;
            }

            return validated;
        }
    }
}
