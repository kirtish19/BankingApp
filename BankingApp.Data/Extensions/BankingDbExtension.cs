namespace BankingApp.Data.Extensions
{
    public static class BankingDbExtension
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddSqlite()
            {
                services.AddDbContext<BankingDbContext>(options =>
                {
                    options.UseSqlite($"Data Source={SharedConstants.DatabaseName}");
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
