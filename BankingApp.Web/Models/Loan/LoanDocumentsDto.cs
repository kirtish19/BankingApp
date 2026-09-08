namespace BankingApp.Web.Models.Loan;

public class LoanDocumentsDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid LoanApplicationId { get; set; }

    public string DocumentName { get; set; } = null!;

    public string BlobUrl { get; set; } = null!;
}