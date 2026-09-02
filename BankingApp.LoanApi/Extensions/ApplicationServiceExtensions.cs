namespace BankingApp.LoanApi.Extensions
{
    public static class ApplicationServiceExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationServices(IConfiguration configuration)
            {
                services.AddSqlServerDatabase(configuration.GetValue<string>("DbConnectionString")!);
                services.AddScoped<ITransactionManager, TransactionManager>();
                services.AddScoped<IStorageHandler, StorageHandler>();
                services.AddScoped<IServiceBusHandler, ServiceBusHandler>();
                services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
                services.AddScoped<ICustomerRepository, CustomerRepository>();
                services.AddScoped<ILoanService, LoanService>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                return services;
            }
        }
    }
}
