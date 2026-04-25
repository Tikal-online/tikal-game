namespace BFF.Configuration;

internal sealed record DuendeConfiguration
{
    public const string Position = "Duende";

    public required string LicenseKey { get; set; }
}