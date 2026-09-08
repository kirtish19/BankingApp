using BankingApp.Web.Models.Loan;
using BankingApp.Web.Models.Staff;
namespace BankingApp.Web.Services.Staff;

public interface IStaffLoanService
{
    Task<List<LoanApplicationsDto>> GetAllLoansAsync();

    Task<List<LoanApplicationsDto>> GetPendingLoansAsync();

    Task<LoanApplicationsDto?> GetLoanByIdAsync(
        Guid loanId);

    Task<bool> UpdateLoanStatusAsync(
        Guid loanId,
        Guid customerId,
        string status,
        string statusDescription,
        string reviewComments);
    Task<CustomerDetailsDto?> GetCustomerDetailsAsync(Guid customerId);

}