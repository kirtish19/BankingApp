using Microsoft.EntityFrameworkCore.Design;

namespace BankingApp.Data;

public class BankingDbContextFactory : IDesignTimeDbContextFactory<BankingDbContext>
{
    public BankingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BankingDbContext>();

        optionsBuilder.UseSqlite($"Data Source={SharedConstants.DatabaseName}");

        return new BankingDbContext(optionsBuilder.Options);
    }
}