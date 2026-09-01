using BankingApp.LoanApi.Models;

namespace BankingApp.LoanApi.Services
{
    public interface ILoanService
    {
        public Task LoanApplicationSubmitAsync(PostLoanApplicationRequest request);
    }
}
