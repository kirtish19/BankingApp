using BankingApp.Web.Models.Registration;

namespace BankingApp.Web.Services.Customer;

public interface ICustomerService
{
    Task<bool> RegisterAsync(RegistrationRequest request);
}
