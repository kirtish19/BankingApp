namespace BankingApp.Web.Models.Customer
{
    public class KycDocument
    {
        public string Name { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long Size { get; set; }

        public byte[] Content { get; set; } = [];
    }
}
