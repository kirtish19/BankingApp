namespace BankingApp.CustomerApi.Models
{
    public class CustomerKYCDocument
    {
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; } = null!;
        public string BlobUrl { get; set; } = null!;
    }
}