using Azure.Identity;
using BFF.Configuration;

namespace BFF.Extensions;

internal static class ConfigurationManagerExtensions
{
    extension(ConfigurationManager configurationManager)
    {
        public void ConfigureKeyVault()
        {
            var keyVaultName = configurationManager.GetValue<string>("KeyVaultName") ?? string.Empty;

            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

            configurationManager.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
        }

        public string GetConnectionString()
        {
            var connectionString = configurationManager.GetConnectionString("bffDb");

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            var options =
                configurationManager.GetSection(DatabaseConfiguration.Position).Get<DatabaseConfiguration>()
                ?? throw new InvalidOperationException("Database Configuration is required");

            connectionString = $"Server={options.Host};" +
                               $"Port={options.Port};" +
                               $"Database={options.DatabaseName};" +
                               $"User ID={options.Username};" +
                               $"Password={options.Password};" +
                               "Ssl Mode=Require;";

            return connectionString;
        }
    }
}