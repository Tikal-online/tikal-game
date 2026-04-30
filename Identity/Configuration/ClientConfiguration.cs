namespace Identity.Configuration;

internal sealed record ClientConfiguration
{
    public const string Position = "Client";

    public required string Secret { get; set; }

    public required string BffUrl { get; set; }
}