using BankingApp.Data.DocumentDb.Containers;

namespace BankingApp.Data.DocumentDb.Repository
{
    public interface IKycDocumentsRepository
    {
        public Task AddKycRecords(IEnumerable<KycDocument> kycDocuments);

        public Task<IEnumerable<KycDocument>> GetKycDocumentsByCustomerId(Guid customerId);
    }
}
