namespace Users.Application.Configuration;

internal sealed record TokenConfiguration
{
    public const string Section = "Token";

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required int AccessTokenExpiration { get; init; }

    public required int RefreshTokenExpiration { get; init; }

    public required string SigningKey { get; init; }
}