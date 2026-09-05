namespace BankingApp.CustomerApi.Models
{
    public class PostLoginResponse
    {
        public string Token { get; set; } = null!;
        public Guid? CustomerId { get; set; }
    }
}
