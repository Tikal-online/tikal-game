using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Configuration;

namespace Users.Application;

public static class UsersApplicationModule
{
    extension(IServiceCollection services)
    {
        public void AddUsersApplication(IConfiguration configuration)
        {
            services.Configure<TokenConfiguration>(configuration.GetSection(TokenConfiguration.Section));
        }
    }
}