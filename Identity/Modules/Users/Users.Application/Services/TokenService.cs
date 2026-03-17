using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Configuration;
using Users.Domain.Entities;

namespace Users.Application.Services;

public interface TokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);

    string GenerateRefreshToken(User user, IEnumerable<string> roles);
}

internal sealed class JwtTokenService : TokenService
{
    private readonly SecurityTokenHandler securityTokenHandler;

    private readonly TokenConfiguration tokenConfiguration;

    private readonly SymmetricSecurityKey securityKey;

    public JwtTokenService(
        SecurityTokenHandler securityTokenHandler,
        IOptions<TokenConfiguration> tokenConfigurationOptions
    )
    {
        this.securityTokenHandler = securityTokenHandler;
        tokenConfiguration = tokenConfigurationOptions.Value;

        securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.SigningKey));
    }

    public string GenerateAccessToken(User user, IEnumerable<string> roles)
    {
        return GenerateToken(user, roles, tokenConfiguration.AccessTokenExpiration);
    }

    public string GenerateRefreshToken(User user, IEnumerable<string> roles)
    {
        return GenerateToken(user, roles, tokenConfiguration.RefreshTokenExpiration);
    }

    private string GenerateToken(User user, IEnumerable<string> roles, int secondsValid)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.UserName!),
            ..roles.Select(role => new Claim(ClaimTypes.Role, role))
        ];

        var now = DateTime.UtcNow;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddSeconds(secondsValid),
            IssuedAt = now,
            Issuer = tokenConfiguration.Issuer,
            Audience = tokenConfiguration.Audience,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = securityTokenHandler.CreateToken(tokenDescriptor);

        return securityTokenHandler.WriteToken(token);
    }
}