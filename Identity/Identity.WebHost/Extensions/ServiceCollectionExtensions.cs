using FluentValidation;
using Identity.WebHost.Configuration;
using Identity.WebHost.ExceptionHandlers;
using MediatR;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using Shared.Application.Pipelines;
using Users.Application;
using Users.Infrastructure;

namespace Identity.WebHost.Extensions;

internal static class ServiceCollectionExtensions
{
    private static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("identityDb");

        if (!string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }

        var options = new DatabaseOptions();
        configuration.GetSection(DatabaseOptions.Position).Bind(options);

        connectionString = $"Server={options.Host};" +
                           $"Port={options.Port};" +
                           $"Database={options.DatabaseName};" +
                           $"User ID={options.Username};" +
                           $"Password={options.Password};" +
                           "Ssl Mode=Require;";

        return connectionString;
    }

    extension(IServiceCollection services)
    {
        public void ConfigureOpenTelemetry()
        {
            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddNpgsql()
                        .AddOtlpExporter();
                })
                .WithLogging(logging =>
                {
                    logging
                        .AddOtlpExporter();
                });
        }

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

        public void AddInfrastructure(IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);

            services.AddUsersInfrastructure(connectionString);
        }

        public void AddExceptionHandlers()
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();

            services.AddProblemDetails();
        }
    }
}