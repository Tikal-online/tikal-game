namespace BFF.Configuration;

internal sealed class DatabaseConfiguration
{
    public const string Position = "Database";

    public required string Host { get; set; }

    public int Port { get; set; }

    public required string DatabaseName { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }
}