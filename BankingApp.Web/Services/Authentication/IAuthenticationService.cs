using BankingApp.Web.Models.Authentication;

namespace BankingApp.Web.Services.Authentication;

public interface IAuthenticationService
{
    Task<LoginResult> LoginAsync(LoginRequest request);

    Task LogoutAsync();

    bool IsAuthenticated { get; }

    string? Token { get; }

    Guid? CustomerId { get; }
}

