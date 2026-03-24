namespace Identity.Configuration;

internal sealed record ClientConfiguration
{
    public const string Position = "Client";

    public string Secret { get; set; } = string.Empty;

    public ICollection<string> RedirectUris { get; set; } = [];

    public ICollection<string> PostLogoutRedirectUris { get; set; } = [];

    public string FrontendChannelLogoutUri { get; set; } = string.Empty;
}