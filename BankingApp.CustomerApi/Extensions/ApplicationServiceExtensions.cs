using BankingApp.Data.Extensions;
using BankingApp.Shared.Helpers;

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
                services.AddScoped( typeof(IEntityRepository<>), typeof(EntityRepository<>));
                services.AddScoped<ITransactionManager, TransactionManager>();
                services.AddScoped<IStorageHandler, StorageHandler>();
                services.AddScoped(typeof(IServiceBusHandler<>), typeof(ServiceBusHandler<>));
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                return services;
            }
        }
    }
}
