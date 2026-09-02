var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration.AddCustomKeyVault(builder.Configuration.GetValue<string>("KeyVaultUri")!);
builder.Services.AddScoped<ILoanAssessmentService, LoanAssessmentService>();
builder.Services.AddScoped<ILoanDocumentRepository, LoanDocumentRepository>();
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

builder.Build().Run();
