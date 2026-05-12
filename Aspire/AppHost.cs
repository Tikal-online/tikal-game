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

var identity = builder.AddProject<Identity>("identity")
    .WithEnvironment("Client__Secret", clientSecret)
    .WithEnvironment("Duende__LicenseKey", duendeLicense)
    .WithReference(identityDb)
    .WaitFor(identityDb);

// tikal backend
var backendDb = postgres.AddDatabase("backendDb");

var backend = builder.AddProject<TikalBackend_WebHost>("tikal-backend")
    .WithEnvironment("Identity__Secret", clientSecret)
    .WithEnvironment("Identity__Authority", identity.GetEndpoint("https"))
    .WithUrl("/scalar/v1", "Scalar")
    .WithReference(backendDb)
    .WaitFor(backendDb);

identity.WithEnvironment("Client__BackendUrl", backend.GetEndpoint("https"));

// bff
var bffDb = postgres.AddDatabase("bffDb");

var bff = builder.AddProject<BFF>("tikal-bff")
    .WithEnvironment("Auth__Secret", clientSecret)
    .WithEnvironment("Auth__Authority", identity.GetEndpoint("https"))
    .WithEnvironment("Duende__LicenseKey", duendeLicense)
    .WithReference(bffDb)
    .WaitFor(bffDb)
    .WaitFor(identity);

identity.WithEnvironment("Client__BffUrl", bff.GetEndpoint("https"));

// tikal frontend
var frontend = builder.AddJavaScriptApp("tikal-frontend", "../TikalFrontend")
    .WithNpm(installCommand: "ci")
    .WithHttpsEndpoint(4200, isProxied: false)
    .WithReference(bff)
    .WaitFor(bff);

bff.WithEnvironment("Frontend__Url", frontend.GetEndpoint("https"));

builder.Build().Run();