namespace BankingApp.Web.Models.Staff;

public class KycDocumentDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string DocumentName { get; set; } = null!;

    public string BlobUrl { get; set; } = null!;
}