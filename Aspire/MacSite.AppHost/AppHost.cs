var builder = DistributedApplication.CreateBuilder(args);

// Single SQLite database file shared by migrations + web + admin (kept under the solution root
// so it survives restarts and is easy to find/inspect locally). WAL mode (set per-connection in
// the data layer) lets the read-mostly site and the write-occasional Studio share it.
var dbPath = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, "..", "..", ".data", "macsite.db"));
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
var connectionString = $"Data Source={dbPath};Foreign Keys=True";

// One-shot schema migration + content seed. Web/Admin wait for it to finish.
var migrations = builder.AddProject<Projects.MacSite_Migrations>("migrations")
    .WithEnvironment("ConnectionStrings__macsitedb", connectionString);

// Public site (static SSR + interactive islands).
builder.AddProject<Projects.MacSite_Web>("web")
    .WithEnvironment("ConnectionStrings__macsitedb", connectionString)
    .WaitForCompletion(migrations)
    .WithExternalHttpEndpoints();

// Studio admin tool (fully interactive).
builder.AddProject<Projects.MacSite_Admin>("admin")
    .WithEnvironment("ConnectionStrings__macsitedb", connectionString)
    .WaitForCompletion(migrations)
    .WithExternalHttpEndpoints();

builder.Build().Run();
