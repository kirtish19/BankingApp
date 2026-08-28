namespace BankingApp.Shared.Extensions
{
    public static class CustomKeyVaultExtensions
    {
        extension(IConfigurationBuilder configuration)
        {
            public IConfigurationBuilder AddCustomKeyVault(string keyVaultUri)
            {
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
                configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential);
                return configuration;
            }
        }

    }
}
