using Microsoft.Extensions.DependencyInjection;
using Shared.Application.DataAccess;

namespace Shared.Infrastructure;

public static class SharedInfrastructureModule
{
    extension(IServiceCollection services)
    {
        public void AddSharedInfrastructure()
        {
            services.AddTransient<TransactionHandler, DbTransactionHandler>();
        }
    }
}