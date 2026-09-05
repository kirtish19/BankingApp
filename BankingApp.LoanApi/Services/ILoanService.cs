namespace BankingApp.LoanApi.Services
{
    public interface ILoanService
    {
        public Task LoanApplicationSubmitAsync(PostLoanApplicationRequest request);
        public Task<IEnumerable<LoanApplicationsDto>> GetAllLoanApplications();
        public Task<LoanApplicationsDto?> GetLoanApplicationById(Guid id);
    }
}
