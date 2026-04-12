namespace BFF.Configuration;

internal sealed record FrontendConfiguration
{
    public const string Position = "Frontend";

    public required string Url { get; set; }
}