using BankingApp.Web.Components;
using BankingApp.Web.Extentions;
using BankingApp.Web.Services.Authentication;
using BankingApp.Web.Services.Customer;
using BankingApp.Web.Services.Loan;
using BankingApp.Web.Services.Staff;
using BankingApp.Web.Validators.Registration;
using FluentValidation;

namespace BankingApp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var keyvaulturi = builder.Configuration.GetConnectionString("KeyVault")!;
            var runningLocal = builder.Configuration.GetValue<bool>("RunningLocal")!;
            var apimBaseUrl = builder.Configuration.GetConnectionString("ApimBaseUrl")!;

            builder.Configuration.AddCustomKeyVault(keyvaulturi, runningLocal);

            // -----------------------------------------
            // Razor Components
            // -----------------------------------------

            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents();


            // -----------------------------------------
            // FluentValidation
            // -----------------------------------------

            builder.Services.AddValidatorsFromAssemblyContaining<
                RegistrationRequestValidator>();
            // -----------------------------------------
            // Entra Authentication
            // -----------------------------------------

            builder.Services.AddScoped<
                IAccessTokenService,
                AccessTokenService>();



            // -----------------------------------------
            // Customer API
            // -----------------------------------------
            builder.Services.AddScoped<
                IAuthStorageService,
                AuthStorageService>();
            builder.Services.AddHttpClient<ICustomerService, CustomerService>(
                client =>
                {
                    client.BaseAddress =
                        new Uri(apimBaseUrl);
                });

            builder.Services.AddHttpClient<
                  IAuthenticationService,
                  AuthenticationService>(
                  client =>
                  {
                      client.BaseAddress =
                          new Uri(apimBaseUrl);
                  });
            builder.Services.AddHttpClient<ILoanService, LoanService>(
                    client =>
                    {
                        client.BaseAddress =
                            new Uri(apimBaseUrl);
                    });
            builder.Services.AddHttpClient<IStaffLoanService, StaffLoanService>(
                    client =>
                    {
                        client.BaseAddress =
                            new Uri(apimBaseUrl);
                    });

            //builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


            var app = builder.Build();


            // -----------------------------------------
            // HTTP Request Pipeline
            // -----------------------------------------

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");

                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute(
                "/not-found",
                createScopeForStatusCodePages: true);

            app.UseHttpsRedirection();

            app.UseAntiforgery();


            // -----------------------------------------
            // Razor Components
            // -----------------------------------------

            app.MapStaticAssets();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();


            app.Run();
        }
    }
}

