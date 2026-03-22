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

builder.Build().Run();