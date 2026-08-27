using BankingApp.Data.DocumentDb.Container;

namespace BankingApp.Data.DocumentDb.Repository
{
    public class KycDocumentsRepository : IKycDocumentsRepository
    {
        private readonly DocumentDbContext _documentDbContext;

        public KycDocumentsRepository(DocumentDbContext documentDbContext)
        {
            _documentDbContext = documentDbContext;
        }
        public async Task AddKycRecords(IEnumerable<KycDocument> kycDocuments)
        {
            await _documentDbContext.KycDocuments.AddRangeAsync(kycDocuments);
            await _documentDbContext.SaveChangesAsync();
        }
    }
}
