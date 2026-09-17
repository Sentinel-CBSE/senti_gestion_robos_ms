var builder = DistributedApplication.CreateBuilder(args);

// Local-only SQL Server container. In the deployed environment this
// resource isn't provisioned by Aspire — ConnectionStrings:sentirobosdb is
// supplied externally (Azure SQL, via Managed Identity), matching the
// design decision that this project doesn't touch Azure infrastructure.
var sentiRobosDb = builder.AddSqlServer("sentirobos-sql")
    .WithDataVolume()
    .AddDatabase("sentirobosdb");

builder.AddProject<Projects.senti_robos_Host>("senti-robos-host")
    .WithReference(sentiRobosDb)
    .WaitFor(sentiRobosDb);

builder.Build().Run();
