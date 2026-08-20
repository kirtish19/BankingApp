namespace BankingApp.LoanApi.Extensions
{
    public static class ApplicationServiceExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationServices()
            {
                return services;
            }
        }
    }
}
