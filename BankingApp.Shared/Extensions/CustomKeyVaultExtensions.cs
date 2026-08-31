namespace BankingApp.Shared.Extensions
{
    public static class CustomKeyVaultExtensions
    {
        extension(IConfigurationBuilder configuration)
        {
            public IConfigurationBuilder AddCustomKeyVault(string keyVaultUri, bool runningLocal = false)
            {
                if (runningLocal)
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
                else
                {
                    var credential = new DefaultAzureCredential();
                    configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential);
                    return configuration;
                }
            }
        }

    }
}
