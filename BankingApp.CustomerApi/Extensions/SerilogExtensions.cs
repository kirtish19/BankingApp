namespace BankingApp.CustomerApi.Extensions
{
    public static class SerilogExtensions
    {
        public static IApplicationBuilder
            UseSerilogRequestLoggingWithClientAddress(
                this IApplicationBuilder app)
        {
            return app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext =
                    (diagnosticContext, httpContext) =>
                    {
                        var clientAddress =
                            httpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "Unknown";

                        diagnosticContext.Set(
                            "ClientAddress",
                            clientAddress);
                    };

                options.MessageTemplate =
                    "HTTP {ClientAddress} {RequestMethod} {RequestPath} " +
                    "responded {StatusCode} in {Elapsed:0.0000} ms";
            });
        }
    }
}
