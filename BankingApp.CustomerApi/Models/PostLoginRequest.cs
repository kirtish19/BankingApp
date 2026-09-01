namespace BankingApp.CustomerApi.Models
{
    public class PostLoginRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}