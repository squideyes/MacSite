using System.Reflection;
using MacSite.Data;
using MacSite.Migrations;
using DbUp;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var connectionString = config.GetConnectionString("macsitedb")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__macsitedb");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("[migrations] No connection string 'macsitedb' was found.");
    return 1;
}

// Make sure the directory for the SQLite file exists (e.g. /data when mounted as a volume).
EnsureDatabaseDirectory(connectionString);

// 1) Schema migrations via DbUp (tracked in 'SchemaVersions'). SQLite creates the file on connect.
var upgrader = DeployChanges.To
    .SqliteDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.EndsWith(".sql"))
    .WithTransactionPerScript()
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();
if (!result.Successful)
{
    Console.Error.WriteLine($"[migrations] Schema upgrade failed: {result.Error}");
    return 2;
}
Console.WriteLine("[migrations] Schema is up to date.");

// 2) Idempotent content seed.
var services = new ServiceCollection();
services.AddMacSiteData(connectionString);
await using var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    await DataSeeder.SeedAsync(scope.ServiceProvider);
}
Console.WriteLine("[migrations] Seed complete.");
return 0;

static void EnsureDatabaseDirectory(string connectionString)
{
    var dataSource = new SqliteConnectionStringBuilder(connectionString).DataSource;
    var dir = Path.GetDirectoryName(Path.GetFullPath(dataSource));
    if (!string.IsNullOrEmpty(dir))
        Directory.CreateDirectory(dir);
}
