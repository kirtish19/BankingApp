using BankingApp.Web.Components.Pages.Login;
using BankingApp.Web.Models.Authentication;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BankingApp.Web.Services.Authentication;

public class AuthenticationService(
    HttpClient httpClient,
    IAccessTokenService accessTokenService,
    IAuthStorageService authStorageService) : IAuthenticationService
{
    private readonly HttpClient _httpClient = httpClient;

    private readonly IAccessTokenService _accessTokenService =
        accessTokenService;

    private readonly IAuthStorageService _authStorageService =
        authStorageService;

    private bool _isAuthenticated;

    private string? _token;

    private Guid? _customerId;


    public bool IsAuthenticated =>
        _isAuthenticated;


    public string? Token =>
        _token;


    public Guid? CustomerId =>
        _customerId;


    public async Task<LoginResult> LoginAsync(
        LoginRequest request)
    {
        try
        {
            // Get Entra access token.
            var entraAccessToken =
                await _accessTokenService.GetAccessTokenAsync();


            // Set Entra token for Customer API call.
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    entraAccessToken);


            // Call Customer API Login through APIM.
            var response =
                await _httpClient.PostAsJsonAsync("user/api/User/Login", request);


            // Invalid username/password.
            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                return new LoginResult
                {
                    ErrorMessage =
                        "Invalid username or password."
                };
            }


            // Handle other API failures.
            if (!response.IsSuccessStatusCode)
            {
                return new LoginResult
                {
                    ErrorMessage =
                        "Unable to login. Please try again later."
                };
            }


            // Read successful login response.
            var loginResult =
                await response.Content
                    .ReadFromJsonAsync<LoginResult>();


            if (loginResult is null ||
                string.IsNullOrWhiteSpace(loginResult.Token))
            {
                return new LoginResult
                {
                    ErrorMessage =
                        "Login response did not contain a valid token."
                };
            }


            // Store authentication information in memory.
            _token =
                loginResult.Token;

            _customerId =
                loginResult.CustomerId;

            _isAuthenticated = true;


            // Frontend session = 10 minutes.
            var sessionExpiresAt =
                DateTime.UtcNow.AddMinutes(10);


            // Store authentication information
            // in browser localStorage.
            await _authStorageService.SaveAuthenticationAsync(
                _token,
                _customerId,
                sessionExpiresAt);


            Console.WriteLine(
                "========== LOGIN SUCCESS ==========");

            Console.WriteLine(
                $"Token received = " +
                $"{!string.IsNullOrWhiteSpace(_token)}");

            Console.WriteLine(
                $"CustomerId = {_customerId}");

            Console.WriteLine(
                $"Frontend session expires at UTC = " +
                $"{sessionExpiresAt}");

            Console.WriteLine(
                $"Stored in localStorage = True");

            Console.WriteLine(
                "===================================");


            return loginResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Authentication: Login exception = " +
                $"{ex.Message}");

            await ClearAuthenticationAsync();

            return new LoginResult
            {
                ErrorMessage =
                    "An unexpected error occurred while logging in."
            };
        }
    }


    public async Task LogoutAsync()
    {
        await ClearAuthenticationAsync();
    }


    private async Task ClearAuthenticationAsync()
    {
        _isAuthenticated = false;

        _token = null;

        _customerId = null;

        await _authStorageService.ClearAuthenticationAsync();
    }
}