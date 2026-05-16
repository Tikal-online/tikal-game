namespace BFF.Configuration;

internal sealed record IdentityConfiguration
{
    public const string Position = "Identity";

    public required string Authority { get; set; }

    public required string Secret { get; set; }
}