namespace Identity.Configuration;

internal sealed record CorsConfiguration
{
    public const string Position = "Cors";

    public ICollection<string> AllowedOrigins { get; set; } = [];
}