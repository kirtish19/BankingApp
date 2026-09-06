namespace BankingApp.Web.Models.Staff;

public class UpdateLoanStatusRequest
{
    public Guid LoanId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string StatusDescription { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string ReviewComments { get; set; } = string.Empty;
}