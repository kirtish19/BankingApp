namespace BankingApp.Shared.Models
{
    public class LoanApplicationMessage
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = null!;
        public DateTimeOffset EventTime { get; set; }
        public string DocumentType { get; set; } = null!;
        public Guid LoanApplicationId { get; set; }
        public Guid CustomerId { get; set; }
        public List<BankingDocument> Documents { get; set; } = [];
        public string UploadedBy { get; set; } = null!;
        public string SourceSystem { get; set; } = null!;
    }
}
