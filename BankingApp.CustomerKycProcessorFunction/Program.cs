using Microsoft.Azure.Cosmos;
using Serilog;

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

var appInsightConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.ApplicationInsights(
            appInsightConnectionString,
            TelemetryConverter.Traces)
        .CreateLogger();


builder.Services.AddSerilog();

Log.Information("Azure function started.");

builder.Services.AddSingleton(s =>
{
    return new CosmosClient(builder.Configuration.GetValue<string>("CosmosDbConnectionString")!);
});

//if (!string.IsNullOrEmpty(appInsightConnectionString))
//{
//    builder.Services
//        .AddOpenTelemetry()
//        .UseFunctionsWorkerDefaults()
//        .WithLogging()
//        .UseAzureMonitorExporter();
//}
builder.Build().Run();
