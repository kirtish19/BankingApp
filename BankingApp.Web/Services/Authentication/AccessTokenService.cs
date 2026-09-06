using Microsoft.Identity.Client;

namespace BankingApp.Web.Services.Authentication;

public class AccessTokenService(
    IConfiguration configuration) : IAccessTokenService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            var tenantId =
                 _configuration.GetValue<string>("AzureEntra:TenantId")
                 ?? throw new InvalidOperationException(
                     "Entra:TenantId is not configured.");

            var clientId =
                 _configuration.GetValue<string>("AzureEntra:CustomerAPI:ClientId")
               //_configuration["team1POC-customer-registration-clientid"]
               ?? throw new InvalidOperationException(
                   "Entra:ClientId is not configured.");
           

            var clientSecret =
                 _configuration.GetValue<string>("AzureEntra:CustomerAPI:ClientSecret")
                //_configuration["team1POC-customer-registration-clientsecret"]
                ?? throw new InvalidOperationException(
                    "Entra:ClientSecret is not configured.");

            var scope =
                _configuration["AzureEntra:CustomerAPI:Scope"]
                ?? throw new InvalidOperationException(
                    "Entra:Scope is not configured.");


            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(
                    $"https://login.microsoftonline.com/{tenantId}")
                .Build();

         
            var result = await app
                .AcquireTokenForClient(
                    new[] { scope })
                .ExecuteAsync();

         
            return result.AccessToken;
        }
        catch (MsalException ex)
        {
            Console.WriteLine(
                $"Failed to acquire Entra access token: {ex.Message}");

            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unexpected error while acquiring access token: {ex.Message}");

            throw;
        }
    }
}