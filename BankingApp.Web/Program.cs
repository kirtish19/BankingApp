using BankingApp.Web.Components;
using BankingApp.Web.Services.Authentication;
using BankingApp.Web.Services.Customer;
using BankingApp.Web.Validators.Registration;
using FluentValidation;

namespace BankingApp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            // Customer API
            // -----------------------------------------

            builder.Services.AddHttpClient<ICustomerService, CustomerService>(
                client =>
                {
                    client.BaseAddress =
                        new Uri("https://localhost:7174/");
                });
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

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

