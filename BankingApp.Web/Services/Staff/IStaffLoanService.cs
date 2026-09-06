using BankingApp.Web.Models.Loan;

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
}