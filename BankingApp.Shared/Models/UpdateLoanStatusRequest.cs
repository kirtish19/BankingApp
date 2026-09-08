using BankingApp.Shared.Constants.Enums;

namespace BankingApp.Shared.Models;

public class UpdateLoanStatusRequest
{
    public Guid LoanId { get; set; }

    public LoanStatus Status { get; set; }

    public string StatusDescription { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string ReviewComments { get; set; } = string.Empty;
}