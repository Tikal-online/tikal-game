using Azure.Identity;

namespace Identity.WebHost.Extensions;

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
    }
}