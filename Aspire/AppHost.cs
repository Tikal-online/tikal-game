using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// parameters
var clientSecret = builder.AddParameter("ClientSecret", true);

var databasePassword = builder.AddParameter("DatabasePassword", true);

var duendeLicense = builder.AddParameter("DuendeLicense", true);

// database server
var postgres = builder.AddPostgres("postgres")
    .WithPassword(databasePassword)
    .WithDataVolume(isReadOnly: false)
    .WithPgAdmin(pgAdmin => pgAdmin
        .WithImageTag("latest")
        .WithHostPort(5050)
    );

// identity
var identityDb = postgres.AddDatabase("identityDb");

builder.AddProject<Identity>("identity")
    .WithEnvironment("Client__Secret", clientSecret)
    .WithEnvironment("Duende__LicenseKey", duendeLicense)
    .WithReference(identityDb)
    .WaitFor(identityDb);

// tikal backend
var backendDb = postgres.AddDatabase("backendDb");

builder.AddProject<TikalBackend_WebHost>("tikal-backend")
    .WithReference(backendDb)
    .WaitFor(backendDb);

// bff
var bffDb = postgres.AddDatabase("bffDb");

var bff = builder.AddProject<BFF>("tikal-bff")
    .WithEnvironment("Auth__Secret", clientSecret)
    .WithEnvironment("Duende__LicenseKey", duendeLicense)
    .WithReference(bffDb)
    .WaitFor(bffDb);

// tikal frontend
builder.AddJavaScriptApp("tikal-frontend", "../TikalFrontend")
    .WithReference(bff)
    .WaitFor(bff);

builder.Build().Run();