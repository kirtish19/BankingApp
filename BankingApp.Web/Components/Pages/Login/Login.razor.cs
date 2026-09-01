using BankingApp.Web.Constants;
using BankingApp.Web.Models.Authentication;
using BankingApp.Web.Services.Authentication;
using Microsoft.AspNetCore.Components;

namespace BankingApp.Web.Components.Pages.Login
{
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

                // Authentication failed

                if (!result.IsAuthenticated)
                {
                    ErrorMessage =
                        result.ErrorMessage ??
                        "Invalid username or password.";

                    return;
                }

                // Account inactive

                if (!result.IsActive)
                {
                    ErrorMessage =
                        "Your account is not active yet.";

                    return;
                }

                // Staff

                if (result.UserType == UserType.Staff)
                {
                    Navigation.NavigateTo(
                        "/staff-dashboard");

                    return;
                }

                // Customer

                Navigation.NavigateTo(
                    "/customer-dashboard");
            }
            finally
            {
                IsLoggingIn = false;
            }
        }
}
}
