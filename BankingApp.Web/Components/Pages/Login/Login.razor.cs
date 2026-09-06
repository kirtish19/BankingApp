using BankingApp.Web.Models.Authentication;
using BankingApp.Web.Services.Authentication;
using Microsoft.AspNetCore.Components;

namespace BankingApp.Web.Components.Pages.Login;

public partial class Login
{
    [Inject]
    private IAuthenticationService AuthenticationService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private readonly LoginRequest loginRequest = new();

    private bool IsLoggingIn;

    private string? ErrorMessage;


    private async Task HandleLogin()
    {
        ErrorMessage = null;

        IsLoggingIn = true;

        try
        {
            var result =
                await AuthenticationService.LoginAsync(
                    loginRequest);

            if (!result.IsAuthenticated)
            {
                ErrorMessage =
                    result.ErrorMessage ??
                    "Invalid username or password.";

                return;
            }

            // CustomerId is available for customers.
            // CustomerId is null for staff.
            if (result.CustomerId is null)
            {
                Navigation.NavigateTo(
                    "/staff-dashboard");

                return;
            }

            Navigation.NavigateTo(
                "/customer-dashboard");
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}

