using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Contexts;

namespace Shared.Application;

public static class SharedApplicationModule
{
    extension(IServiceCollection services)
    {
        public void AddSharedApplication()
        {
            services.AddScoped<AccountContext>();
        }
    }
}