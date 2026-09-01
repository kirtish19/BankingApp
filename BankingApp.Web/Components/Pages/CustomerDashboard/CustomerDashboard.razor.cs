using BankingApp.Web.Services.Authentication;
using Microsoft.AspNetCore.Components;

namespace BankingApp.Web.Components.Pages.CustomerDashboard
{
    public partial class CustomerDashboard
    {
        [Inject]
        private IAuthenticationService AuthenticationService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;


        private void GoToLoans()
        {
            Navigation.NavigateTo("/apply-loan");
        }


        private async Task Logout()
        {
            await AuthenticationService.LogoutAsync();

            Navigation.NavigateTo("/login");
        }

    }
}
