using RestApi.Controllers;
using Scalar.AspNetCore;
using TikalBackend.WebHost.Extensions;
using TikalBackend.WebHost.SchemaTransformers;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.ConfigureKeyVault();
}

builder.Logging.ClearProviders();
builder.Services.ConfigureOpenTelemetry();

builder.Services.AddControllers().AddApplicationPart(AssemblyReference.Assembly);

builder.Services.AddMediatR();

builder.Services.AddValidators();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandlers();

builder.Services.AddOpenApi(options => { options.AddDocumentTransformer<SecuritySchemeTransformer>(); });

builder.Services.AddHealthChecks();

builder.Services.ConfigureAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("OAuth2")
            .AddAuthorizationCodeFlow("OAuth2",
                flow =>
                {
                    flow.ClientId = "interactive";
                    flow.Pkce = Pkce.Sha256;
                    flow.SelectedScopes = ["openid", "profile", "tikal-backend"];
                });
    }).AllowAnonymous();
}

app.MapOpenApi().AllowAnonymous();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthChecks("/healthcheck");

app.MapControllers();

await app.RunAsync();