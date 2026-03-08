using Microsoft.Extensions.DependencyInjection;

namespace Users.Application;

public static class UsersModule
{
    extension(IServiceCollection services)
    {
        public void AddUsersApplication()
        {
            services.AddSingleton<UserMetrics, DiagnosticUserMetrics>();
        }
    }
}