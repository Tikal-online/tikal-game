using System.Reflection;
using Azure.Identity;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Identity.Configuration;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Identity;

internal static class HostingExtensions
{
    private static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("identityDb");

        if (!string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }

        var options = new DatabaseConfiguration();
        configuration.GetSection(DatabaseConfiguration.Section).Bind(options);

        connectionString = $"Server={options.Host};" +
                           $"Port={options.Port};" +
                           $"Database={options.DatabaseName};" +
                           $"User ID={options.Username};" +
                           $"Password={options.Password};" +
                           "Ssl Mode=Require;";

        return connectionString;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.InitializeDatabase();

        app.UseForwardedHeaders();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "style-src 'self' https://cdn.jsdelivr.net; " +
                "script-src 'self' https://cdn.jsdelivr.net https://code.jquery.com; "
            );
            await next();
        });

        app.MapRazorPages()
            .RequireAuthorization();

        app.MapHealthChecks("/healthcheck");

        return app;
    }

    private static void InitializeDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        context.Database.Migrate();

        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

        var clientConfig = new ClientConfiguration();
        configuration.Bind(ClientConfiguration.Position, clientConfig);

        foreach (var client in Config.GetClients(clientConfig))
        {
            if (context.Clients.Any(c => c.ClientId == client.ClientId))
            {
                continue;
            }

            context.Clients.Add(client.ToEntity());
        }

        foreach (var resource in Config.IdentityResources)
        {
            if (context.IdentityResources.Any(r => r.Name == resource.Name))
            {
                continue;
            }

            context.IdentityResources.Add(resource.ToEntity());
        }

        foreach (var resource in Config.ApiScopes)
        {
            if (context.ApiScopes.Any(r => r.Name == resource.Name))
            {
                continue;
            }

            context.ApiScopes.Add(resource.ToEntity());
        }

        context.SaveChanges();
    }

    extension(ConfigurationManager configurationManager)
    {
        public void ConfigureKeyVault()
        {
            var keyVaultName = configurationManager.GetValue<string>("KeyVaultName") ?? string.Empty;

            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

            configurationManager.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
        }
    }

    extension(WebApplicationBuilder builder)
    {
        public WebApplication ConfigureServices()
        {
            builder.Services.AddRazorPages();

            builder.Services.AddHealthChecks();

            var connectionString = GetConnectionString(builder.Configuration);

            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            var clientConfig = new ClientConfiguration();
            builder.Configuration.Bind(ClientConfiguration.Position, clientConfig);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>()
                .SetApplicationName("Identity");

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddConfigurationStoreCache()
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                    options.EnableTokenCleanup = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.GetClients(clientConfig))
                .AddAspNetIdentity<ApplicationUser>();

            builder.Logging.ClearProviders();
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resourceBuilder => resourceBuilder.AddService(builder.Environment.ApplicationName))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddSource(IdentityServerConstants.Tracing.Basic)
                        .AddSource(IdentityServerConstants.Tracing.Cache)
                        .AddSource(IdentityServerConstants.Tracing.Services)
                        .AddSource(IdentityServerConstants.Tracing.Stores)
                        .AddSource(IdentityServerConstants.Tracing.Validation)
                        .AddAspNetCoreInstrumentation()
                        .AddNpgsql()
                        .AddOtlpExporter();
                })
                .WithMetrics(metrics =>
                    {
                        metrics
                            .AddMeter(Telemetry.ServiceName)
                            .AddMeter(Pages.Telemetry.ServiceName)
                            .AddAspNetCoreInstrumentation()
                            .AddNpgsqlInstrumentation()
                            .AddOtlpExporter();
                    }
                )
                .WithLogging(logging =>
                {
                    logging
                        .AddOtlpExporter();
                });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedHost |
                                           ForwardedHeaders.XForwardedProto;

                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });

            builder.Services.AddAuthentication();

            return builder.Build();
        }
    }
}