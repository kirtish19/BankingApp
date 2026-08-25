using Azure.Identity;
using Serilog;
using System.Text.Json.Serialization;

try
{


    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    // Keyvault Setup

    var credential = new DefaultAzureCredential(
    new DefaultAzureCredentialOptions
    {
        ExcludeEnvironmentCredential = true,
        ExcludeWorkloadIdentityCredential = true,
        ExcludeManagedIdentityCredential = true,
        ExcludeVisualStudioCodeCredential = true,
        ExcludeAzurePowerShellCredential = true,
        ExcludeAzureDeveloperCliCredential = true
    });
    var keyvaulturi = builder.Configuration.GetConnectionString("KeyVault")!;
    builder.Configuration.AddAzureKeyVault(new Uri(keyvaulturi), credential);


    builder.Host.UseSerilog();

    Log.Information("Starting the BankingApp Customer API");

    // Add services to the container.
    builder.Services.AddApplicationServices(builder.Configuration);

    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter());
        });

    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Loan API v1");
    });

    // Serilog request logging
    app.UseSerilogRequestLoggingWithClientAddress();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}