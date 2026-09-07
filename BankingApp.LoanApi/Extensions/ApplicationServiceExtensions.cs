using BankingApp.Data;
using BankingApp.Data.DocumentDb.Repository;
using Microsoft.Azure.Cosmos;

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
                services.AddScoped<ILoanDocumentRepository, LoanDocumentRepository>();
                services.AddSingleton(s =>
                {
                    return new CosmosClient(configuration.GetValue<string>("CosmosDbConnectionString")!);
                });
                return services;
            }
        }
    }
}
