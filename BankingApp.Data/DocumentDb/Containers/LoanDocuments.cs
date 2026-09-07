using Azure.Core.Pipeline;
using Newtonsoft.Json;

namespace BankingApp.Data.DocumentDb.Containers
{
    public class LoanDocuments
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid LoanApplicationId { get; set; }
        public string DocumentName { get; set; } = null!;
        public string BlobUrl { get; set; } = null!;
    }
}
