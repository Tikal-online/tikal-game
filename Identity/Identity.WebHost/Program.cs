using Identity.WebHost.Extensions;
using RestApi.Controllers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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
}

app.MapOpenApi();

app.MapScalarApiReference();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapHealthChecks("/healthcheck");

app.MapControllers();

app.Run();