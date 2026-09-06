
namespace BankingApp.Web.Models.Loan;
public class LoanDocument
{
    public string Name { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long Size { get; set; }

    public byte[] Content { get; set; } = [];

}
