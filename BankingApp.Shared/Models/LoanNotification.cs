namespace BankingApp.Shared.Models
{
    public class LoanNotification
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = null!;
        public DateTimeOffset EventTime { get; set; }
        public string NotificationType { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string Remarks { get; set; } = null!;
        public string SourceSystem { get; set; } = null!;
    }
}
