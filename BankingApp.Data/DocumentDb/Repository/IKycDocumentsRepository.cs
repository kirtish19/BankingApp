using BankingApp.Data.DocumentDb.Container;

namespace BankingApp.Data.DocumentDb.Repository
{
    public interface IKycDocumentsRepository
    {
        public Task AddKycRecords(IEnumerable<KycDocument> kycDocuments);
    }
}
