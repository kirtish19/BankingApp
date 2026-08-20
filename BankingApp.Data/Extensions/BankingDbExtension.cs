namespace BankingApp.Data.Extensions
{
    public static class BankingDbExtension
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddInMemoryDatabase()
            {
                services.AddDbContext<BankingDbContext>(options =>
                {
                    options.UseInMemoryDatabase(SharedConstants.InMemoryDatabaseName);
                });
                return services;
            }

            public IServiceCollection AddSqlServerDatabase(string connectionString)
            {
                services.AddDbContext<BankingDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
                return services;
            }
        }
    }
}
