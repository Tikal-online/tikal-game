using RestApi.Controllers;
using SignalRApi.Hubs.Lobbies.GlobalChat;
using TikalBackend.WebHost.Extensions;
using TikalBackend.WebHost.Middleware;
using TikalBackend.WebHost.SchemaTransformers;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.ConfigureKeyVault();
}

builder.Logging.ClearProviders();

builder.Services.ConfigureOpenTelemetry();

builder.Services.AddControllers().AddApplicationPart(AssemblyReference.Assembly);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandlers();

builder.Services.AddOpenApi(options => { options.AddDocumentTransformer<SecuritySchemeTransformer>(); });

builder.Services.AddHealthChecks();

builder.Services.ConfigureAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.UseScalarUi();
}

app.MapOpenApi().AllowAnonymous();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<AccountMiddleware>();

app.MapHealthChecks("/healthcheck");

app.MapControllers();

app.MapHub<GlobalChatHub>("/globalChat");

await app.RunAsync();