namespace BankingApp.CustomerApi.Models
{
	public class CustomerKYCEvent
	{
		public Guid EventId { get; set; }
		public string EventType { get; set; } = null!;
		public DateTimeOffset EventTime { get; set; }
		public string DocumentType { get; set; } = null!;
		public Guid CustomerId { get; set; }
		public List<CustomerKYCDocument> Documents { get; set; } = [];
		public string UploadedBy { get; set; } = null!;
		public string SourceSystem { get; set; } = null!;
	}
}
