namespace BankingApp.Web.Services.Authentication;

public interface IAuthStorageService
{
    Task SaveAuthenticationAsync(
        string token,
        Guid? customerId,
        DateTime expiresAt);

    Task<string?> GetTokenAsync();

    Task<Guid?> GetCustomerIdAsync();

    Task<bool> IsSessionValidAsync();

    Task ClearAuthenticationAsync();
}