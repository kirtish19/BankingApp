namespace BankingApp.Data.BankingDb.Repository
{
    public interface ILoanApplicationRepository : IEntityRepository<LoanApplications>
    {
        public Task<IEnumerable<LoanApplications>> GetLoanApplicationsForCustomerAsync(Guid customerId);
        public Task<IEnumerable<LoanApplications>> GetLoanApplicationsByStatusAsync(LoanStatus loanStatus);
    }
}
