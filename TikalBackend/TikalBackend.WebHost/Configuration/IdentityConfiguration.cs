namespace TikalBackend.WebHost.Configuration;

internal sealed record IdentityConfiguration
{
    public const string Position = "Identity";

    public string Authority { get; set; } = string.Empty;
}