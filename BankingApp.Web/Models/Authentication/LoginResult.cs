namespace BankingApp.Web.Models.Authentication;

public class LoginResult
{
    public string? Token { get; set; }

    public Guid? CustomerId { get; set; }

    public string? ErrorMessage { get; set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(Token);
}