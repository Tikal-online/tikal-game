using RestApi.Controllers;
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
    app.UseScalarUi();
}

app.MapOpenApi().AllowAnonymous();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthChecks("/healthcheck");

app.MapControllers();

await app.RunAsync();