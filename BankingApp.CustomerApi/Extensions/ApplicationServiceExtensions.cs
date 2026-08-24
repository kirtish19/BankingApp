using BankingApp.Data.Extensions;

namespace BankingApp.CustomerApi.Extensions
{
    public static class ApplicationServiceExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationServices(IConfiguration configuration)
            {
                services.AddSqlServerDatabase(configuration.GetConnectionString("Default")!);
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<ICustomerRepository, CustomerRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                return services;
            }
        }
    }
}
