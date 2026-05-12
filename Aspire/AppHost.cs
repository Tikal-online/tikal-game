using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// ================================================
// Global parameters
// ================================================
var clientSecret = builder.AddParameter("ClientSecret", true);

var databasePassword = builder.AddParameter("DatabasePassword", true);

var duendeLicense = builder.AddParameter("DuendeLicense", true);

// ================================================
// Services
// ================================================

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
    .WithReference(identityDb)
    .WaitFor(identityDb);

// tikal backend
var backendDb = postgres.AddDatabase("backendDb");

var backend = builder.AddProject<TikalBackend_WebHost>("tikal-backend")
    .WithUrl("/scalar/v1", "Scalar")
    .WithReference(backendDb)
    .WaitFor(backendDb);

// bff
var bffDb = postgres.AddDatabase("bffDb");

var bff = builder.AddProject<BFF>("tikal-bff")
    .WithReference(bffDb)
    .WaitFor(bffDb);

// tikal frontend
var frontend = builder.AddJavaScriptApp("tikal-frontend", "../TikalFrontend")
    .WithNpm(installCommand: "ci")
    .WithHttpsEndpoint(4200, isProxied: false)
    .WithReference(bff)
    .WaitFor(bff);

// ================================================
// Configuration
// ================================================

// identity
identity.WithEnvironment("Client__Secret", clientSecret);
identity.WithEnvironment("Client__BffUrl", bff.GetEndpoint("https"));
identity.WithEnvironment("Client__BackendUrl", backend.GetEndpoint("https"));
identity.WithEnvironment("Duende__LicenseKey", duendeLicense);

// tikal backend
backend.WithEnvironment("Identity__Secret", clientSecret);
backend.WithEnvironment("Identity__Authority", identity.GetEndpoint("https"));

// bff
bff.WithEnvironment("Auth__Secret", clientSecret);
bff.WithEnvironment("Auth__Authority", identity.GetEndpoint("https"));
bff.WithEnvironment("Frontend__Url", frontend.GetEndpoint("https"));
bff.WithEnvironment("Duende__LicenseKey", duendeLicense);

await builder.Build().RunAsync();