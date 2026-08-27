namespace BankingApp.Data.DocumentDb.Container
{
    public class KycDocument
    {
        public Guid DocumentId { get; set; }
        public Guid CustomerId { get; set; }
        public string DocumentName { get; set; } = null!;
        public string BlobUrl { get; set; } = null!;
    }
}
