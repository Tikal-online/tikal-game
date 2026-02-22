using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false);

// identity
var identityDb = postgres.AddDatabase("identityDb");

builder.AddProject<Identity_WebHost>("identity")
    .WithReference(identityDb)
    .WaitFor(identityDb);

builder.Build().Run();