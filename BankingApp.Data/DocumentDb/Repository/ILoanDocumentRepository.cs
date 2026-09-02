using BankingApp.Data.DocumentDb.Containers;

namespace BankingApp.Data.DocumentDb.Repository
{
    public interface ILoanDocumentRepository
    {
        public Task AddLoanDocumentRecords(IEnumerable<LoanDocuments> loanDocuments);
    }
}
