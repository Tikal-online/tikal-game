namespace Identity.WebHost.Configuration;

public class DatabaseConfiguration
{
    public const string Position = "Database";

    public required string Host { get; init; }

    public required int Port { get; init; }

    public required string DatabaseName { get; init; }

    public required string Username { get; init; }

    public required string Password { get; init; }
}