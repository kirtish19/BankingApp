using BankingApp.Web.Constants;
namespace BankingApp.Web.Models.Authentication;

public class LoginResult
{
    public bool IsAuthenticated { get; set; }

    public bool IsActive { get; set; }

    public UserType UserType { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}

