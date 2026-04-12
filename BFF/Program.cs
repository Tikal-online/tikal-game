using BFF;
using BFF.Configuration;
using BFF.Extensions;
using Duende.Bff;
using Duende.Bff.Endpoints;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.ConfigureKeyVault();
}

builder.Services.AddControllers();

var frontendConfiguration =
    builder.Configuration.GetSection(FrontendConfiguration.Position).Get<FrontendConfiguration>()
    ?? throw new InvalidOperationException("Frontend configuration is required");

var authConfiguration =
    builder.Configuration.GetSection(AuthConfiguration.Position).Get<AuthConfiguration>()
    ?? throw new InvalidOperationException("Authentication configuration is required");

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(frontendConfiguration.Url)
            .WithHeaders("x-csrf", "content-type")
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddBff()
    .AddRemoteApis()
    .ConfigureOpenIdConnect(options =>
    {
        options.Authority = authConfiguration.Authority;
        options.ClientId = "interactive.confidential";
        options.ClientSecret = authConfiguration.Secret;
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        options.SaveTokens = true;
        options.DisableTelemetry = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("tikal-api");
        options.Scope.Add("offline_access");

        options.Events = new OpenIdConnectEvents
        {
            OnAccessDenied = ctx =>
            {
                ctx.Response.Redirect(frontendConfiguration.Url);
                ctx.HandleResponse();
                return Task.CompletedTask;
            }
        };
    })
    .ConfigureCookies(options =>
    {
        options.Cookie.Name = "__Host-bff";
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddDataProtection()
    .SetApplicationName("BFF");

builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IReturnUrlValidator, FrontendHostReturnUrlValidator>();

var app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.MapHealthChecks("/healthcheck");

await app.RunAsync();