using FluentValidation;
using Identity.WebHost.ExceptionHandlers;
using MediatR;
using Shared.Application.Pipelines;
using Users.Application;
using Users.Infrastructure;

namespace Identity.WebHost.Extensions;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddMediatR()
        {
            services.AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblies(AssemblyReference.Assembly);

                c.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));
            });
        }

        public void AddValidators()
        {
            services.AddValidatorsFromAssemblies([AssemblyReference.Assembly]);
        }

        public void AddInfrastructure()
        {
            services.AddSingleton<UserRepository, IdentityUserRepository>();
        }

        public void AddExceptionHandlers()
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();

            services.AddProblemDetails();
        }
    }
}