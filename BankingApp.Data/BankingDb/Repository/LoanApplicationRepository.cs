namespace BankingApp.Data.BankingDb.Repository
{
    public class LoanApplicationRepository : EntityRepository<LoanApplications>, ILoanApplicationRepository
    {
        public LoanApplicationRepository(BankingDbContext context) : base(context)
        {
        }
    }
}
