using Azure.Monitor.OpenTelemetry.Exporter;
using BankingApp.CustomerKycProcessorFunction.Services;
using BankingApp.Data.BankingDb.Extensions;
using BankingApp.Data.BankingDb.Repository;
using BankingApp.Data.DocumentDb.Extensions;
using BankingApp.Data.DocumentDb.Repository;
using BankingApp.Shared.Extensions;
using BankingApp.Shared.Helpers;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration.AddCustomKeyVault(builder.Configuration.GetValue<string>("KeyVaultUri")!);
builder.Services.AddScoped<IMetaDataProcessorService, MetaDataProcessorService>();
builder.Services.AddScoped<IKycDocumentsRepository, KycDocumentsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<IServiceBusHandler, ServiceBusHandler>();
builder.Services.AddSqlServerDatabase(builder.Configuration.GetValue<string>("DbConnectionString")!);
builder.Services.AddCosmosDatabase(builder.Configuration.GetValue<string>("CosmosDbConnectionString")!, builder.Configuration.GetValue<string>("CosmosDbName")!);

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
