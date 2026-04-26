using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithPgAdmin(pgAdmin => pgAdmin
        .WithImageTag("latest")
        .WithHostPort(5050)
    );

// identity
var identityDb = postgres.AddDatabase("identityDb");

builder.AddProject<Identity>("identity")
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
    .WithReference(bffDb)
    .WaitFor(bffDb);

// tikal frontend
builder.AddJavaScriptApp("tikal-frontend", "../TikalFrontend")
    .WithReference(bff)
    .WaitFor(bff);

builder.Build().Run();