try
{
    var builder = WebApplication.CreateBuilder(args);

    var keyvaulturi = builder.Configuration.GetConnectionString("KeyVault")!;
    var runningLocal = builder.Configuration.GetValue<bool>("RunningLocal")!;

    builder.Configuration.AddCustomKeyVault(keyvaulturi, runningLocal);

    var appInsightConnectionString = builder.Configuration.GetValue<string>("LoanApiAppInsightConnectionString")!;

    Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.ApplicationInsights(
                appInsightConnectionString,
                TelemetryConverter.Traces)
            .CreateLogger();

    builder.Host.UseSerilog();

    Log.Information("Starting the BankingApp Loan API");

    // Add services to the container.
    builder.Services.AddApplicationServices(builder.Configuration);


    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter());

            options.JsonSerializerOptions.NumberHandling =
            JsonNumberHandling.Strict;
        });

    builder.Services.AddOpenApi(options =>
    {
        options.AddSchemaTransformer((schema, context, cancellationToken) =>
        {
            // Convert number|string -> number
            if (schema.Type.HasValue &&
                schema.Type.Value.HasFlag(JsonSchemaType.Number) &&
                schema.Type.Value.HasFlag(JsonSchemaType.String))
            {
                schema.Type = JsonSchemaType.Number;
                schema.Pattern = null;
            }

            // Convert integer|string -> integer
            if (schema.Type.HasValue &&
                schema.Type.Value.HasFlag(JsonSchemaType.Integer) &&
                schema.Type.Value.HasFlag(JsonSchemaType.String))
            {
                schema.Type = JsonSchemaType.Integer;
                schema.Pattern = null;
            }

            return Task.CompletedTask;
        });
    });

    var app = builder.Build();

    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Loan API v1");
    });

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}