namespace BFF.Configuration;

internal sealed record BackendConfiguration
{
    public const string Position = "Backend";

    public required string Url { get; set; }
}