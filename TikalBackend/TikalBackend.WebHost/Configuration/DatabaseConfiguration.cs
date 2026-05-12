namespace TikalBackend.WebHost.Configuration;

internal sealed record DatabaseConfiguration
{
    public const string Section = "Database";

    public required string Host { get; set; }

    public required int Port { get; set; }

    public required string DatabaseName { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }
}