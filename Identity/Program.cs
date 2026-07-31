using Identity;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

if (args.Contains("migrate", StringComparer.OrdinalIgnoreCase))
{
    app.MigrateDatabase();
    return;
}

await app.RunAsync();