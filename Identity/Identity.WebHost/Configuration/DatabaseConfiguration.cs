namespace Identity.WebHost.Configuration;

internal sealed class DatabaseConfiguration
{
    public const string Section = "Database";

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string DatabaseName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}