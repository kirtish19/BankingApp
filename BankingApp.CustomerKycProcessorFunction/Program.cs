using Microsoft.Azure.Cosmos;

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
//builder.Services.AddCosmosDatabase(builder.Configuration.GetValue<string>("CosmosDbConnectionString")!, builder.Configuration.GetValue<string>("CosmosDbName")!);

builder.Services.AddSingleton(s =>
{
    return new CosmosClient(builder.Configuration.GetValue<string>("CosmosDbConnectionString")!);
});

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
