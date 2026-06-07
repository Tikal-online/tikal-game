using System.Reflection.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace SignalRApi.Hubs;

public static class SignalRModule
{
    extension(IServiceCollection services)
    {
        public void AddSignalRModule()
        {
            services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

            services.AddSignalR();
        }
    }
}