using Identity.WebHost.Extensions;
using RestApi.Controllers;
using Scalar.AspNetCore;

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
else
{
    app.MapScalarApiReference(options => { options.AddServer("https://auth.tikalonline.com"); });
}

app.MapOpenApi();


app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapHealthChecks("/healthcheck");

app.MapControllers();

app.Run();