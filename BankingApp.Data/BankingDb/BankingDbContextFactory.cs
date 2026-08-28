namespace BankingApp.Data.BankingDb;

public class BankingDbContextFactory : IDesignTimeDbContextFactory<BankingDbContext>
{
    public BankingDbContext CreateDbContext(string[] args)
    {
        var keyVaultUrl = "https://team1-customerapi-kv.vault.azure.net/";
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

        var secretClient = new SecretClient(new Uri(keyVaultUrl), credential);

        var secret = secretClient.GetSecret("DbConnectionString");
        var optionsBuilder = new DbContextOptionsBuilder<BankingDbContext>();

        optionsBuilder.UseSqlServer(secret.Value.Value);

        return new BankingDbContext(optionsBuilder.Options);
    }
}