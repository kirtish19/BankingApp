using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankingApp.Data.DocumentDb.Extensions
{
    public static class DocumentDbExtension
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddCosmosDatabase(
                string connectionString,
                string databaseName)
            {
                services.AddDbContext<DocumentDbContext>(options =>
                {
                    options.UseCosmos(
                        connectionString,
                        databaseName);
                });

                return services;
            }
        }
    }
}