namespace BankingApp.LoanApi.Services
{
    public interface ILoanService
    {
        public Task LoanApplicationSubmitAsync(PostLoanApplicationRequest request);
        public Task<IEnumerable<LoanApplicationsDto>> GetAllLoanApplications();
        public Task<LoanApplicationsDto?> GetLoanApplicationById(Guid id);
        public Task<IEnumerable<LoanApplicationsDto>> GetLoanApplicationsForCustomerAsync(Guid customerId);
        public Task<IEnumerable<LoanApplicationsDto>> GetPendingLoanApplicationsAsync();
    }
}
