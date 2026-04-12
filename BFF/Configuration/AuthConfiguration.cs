namespace BFF.Configuration;

internal sealed record AuthConfiguration
{
    public const string Position = "Auth";

    public required string Authority { get; set; }

    public required string Secret { get; set; }
}