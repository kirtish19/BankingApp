namespace BankingApp.Shared.Models
{
    public class BankingDocument
    {
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; } = null!;
        public string BlobUrl { get; set; } = null!;
    }
}