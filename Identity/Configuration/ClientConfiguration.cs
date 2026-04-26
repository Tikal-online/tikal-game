namespace Identity.Configuration;

internal sealed record ClientConfiguration
{
    public const string Position = "Client";

    public required string Secret { get; set; }

    public ICollection<string> RedirectUris { get; set; } = [];

    public ICollection<string> PostLogoutRedirectUris { get; set; } = [];

    public required string FrontendChannelLogoutUri { get; set; }
}