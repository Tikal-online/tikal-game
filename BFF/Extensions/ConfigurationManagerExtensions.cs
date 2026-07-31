using BFF.Configuration;

namespace BFF.Extensions;

internal static class ConfigurationManagerExtensions
{
    extension(ConfigurationManager configurationManager)
    {
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