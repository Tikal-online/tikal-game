using Identity;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.ConfigureKeyVault();
}

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

SeedData.EnsureSeedData(app);

await app.RunAsync();