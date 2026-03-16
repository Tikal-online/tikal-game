using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Configuration;
using Users.Application.Services;

namespace Users.Application;

public static class UsersApplicationModule
{
    extension(IServiceCollection services)
    {
        public void AddUsersApplication(IConfiguration configuration)
        {
            services.Configure<TokenConfiguration>(configuration.GetSection(TokenConfiguration.Section));

            services.AddSingleton<SecurityTokenHandler, JwtSecurityTokenHandler>();

            services.AddScoped<TokenService, JwtTokenService>();
        }
    }
}