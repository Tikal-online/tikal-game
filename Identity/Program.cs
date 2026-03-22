using Identity;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureLogging()
    .ConfigureServices()
    .ConfigurePipeline();

SeedData.EnsureSeedData(app);

await app.RunAsync();