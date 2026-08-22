namespace BankingApp.CustomerApi.Extensions
{
    public static class ApplicationServiceExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationServices(IConfiguration configuration)
            {
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<ICustomerRepository, CustomerRepository>();

                return services;
            }
        }
    }
}
