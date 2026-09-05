namespace BankingApp.CustomerApi.Models
{
    public class LoginResponseDto
    {
        public bool LoginSuccess { get; set; }
        public User? User { get; set; }
    }
}
