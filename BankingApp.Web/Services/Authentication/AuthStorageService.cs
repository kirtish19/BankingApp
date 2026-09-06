using Microsoft.JSInterop;

namespace BankingApp.Web.Services.Authentication;

public class AuthStorageService(
    IJSRuntime jsRuntime) : IAuthStorageService
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    private const string TokenKey =
        "bankingapp_token";

    private const string CustomerIdKey =
        "bankingapp_customer_id";

    private const string ExpirationKey =
        "bankingapp_session_expiration";


    public async Task SaveAuthenticationAsync(
        string token,
        Guid? customerId,
        DateTime expiresAt)
    {
        await _jsRuntime.InvokeVoidAsync(
            "sessionStorage.setItem",
            TokenKey,
            token);

        await _jsRuntime.InvokeVoidAsync(
            "sessionStorage.setItem",
            CustomerIdKey,
            customerId?.ToString() ?? string.Empty);

        await _jsRuntime.InvokeVoidAsync(
            "sessionStorage.setItem",
            ExpirationKey,
            expiresAt.ToString("O"));
    }


    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>(
            "sessionStorage.getItem",
            TokenKey);
    }


    public async Task<Guid?> GetCustomerIdAsync()
    {
        var value =
            await _jsRuntime.InvokeAsync<string?>(
                "sessionStorage.getItem",
                CustomerIdKey);

        if (Guid.TryParse(value, out var customerId))
        {
            return customerId;
        }

        return null;
    }


    public async Task<bool> IsSessionValidAsync()
    {
        var expiration =
            await _jsRuntime.InvokeAsync<string?>(
                "sessionStorage.getItem",
                ExpirationKey);

        if (!DateTime.TryParse(
                expiration,
                out var expiresAt))
        {
            return false;
        }

        return DateTime.UtcNow < expiresAt;
    }


    public async Task ClearAuthenticationAsync()
    {
        await _jsRuntime.InvokeVoidAsync(
            "sessionStorage.removeItem",
            TokenKey);

        await _jsRuntime.InvokeVoidAsync(
            "sessionStorage.removeItem",
            CustomerIdKey);

        await _jsRuntime.InvokeVoidAsync(
            "sessionStorage.removeItem",
            ExpirationKey);
    }
}