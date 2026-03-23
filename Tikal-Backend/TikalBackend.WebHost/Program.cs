using RestApi.Controllers;
using Scalar.AspNetCore;
using TikalBackend.WebHost.Extensions;

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

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.MapScalarApiReference();
}

app.MapOpenApi();

app.UseExceptionHandler();

app.MapHealthChecks("/healthcheck");

app.MapControllers();

await app.RunAsync();