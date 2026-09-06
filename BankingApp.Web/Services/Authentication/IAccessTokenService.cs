namespace BankingApp.Web.Services.Authentication;

public interface IAccessTokenService
{
    Task<string> GetAccessTokenAsync();
}