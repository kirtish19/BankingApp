using BankingApp.Web.Models.Loan;

namespace BankingApp.Web.Services.Loan;

public interface ILoanService
{
    Task<bool> SubmitLoanApplicationAsync(
        LoanApplicationRequest request);

    Task<List<LoanApplicationsDto>> GetLoansForCustomerAsync(
        Guid customerId);
}