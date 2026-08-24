using Microsoft.EntityFrameworkCore.Design;

namespace BankingApp.Data;

public class BankingDbContextFactory : IDesignTimeDbContextFactory<BankingDbContext>
{
    public BankingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BankingDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=BankingDb;User Id=sa;Password=Temp@12345;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True");

        return new BankingDbContext(optionsBuilder.Options);
    }
}