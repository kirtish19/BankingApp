namespace BankingApp.Data.BankingDb.Repository
{
    public class LoanApplicationRepository : EntityRepository<LoanApplications>, ILoanApplicationRepository
    {
        private readonly BankingDbContext _context;

        public LoanApplicationRepository(BankingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoanApplications>> GetLoanApplicationsByStatusAsync(LoanStatus loanStatus)
        {
            var loanApplications = await _context.LoanApplications.Where(l => l.Status == loanStatus).ToListAsync();
            return loanApplications;
        }

        public async Task<IEnumerable<LoanApplications>> GetLoanApplicationsForCustomerAsync(Guid customerId)
        {
            var loanApplications = await _context.LoanApplications.Where(l => l.CustomerId == customerId).ToListAsync();
            return loanApplications;
        }
    }
}
