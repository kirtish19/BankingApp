namespace BankingApp.Web.Models.Authentication
{
    public class PostLoginResponse
    {
        public string Token { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
