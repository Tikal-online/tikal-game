namespace Identity.Configuration;

internal sealed record ClientConfiguration
{
    public const string Position = "Client";

    public string Secret { get; set; } = string.Empty;

    public string Uri { get; set; } = string.Empty;
}