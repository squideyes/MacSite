using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace MacSite.Data.Db;

/// <summary>Opens connections to the SQLite database file.</summary>
public interface IDbConnectionFactory
{
    Task<DbConnection> OpenAsync(CancellationToken ct = default);
}

/// <summary>
/// Opens pooled <see cref="SqliteConnection"/>s and applies the per-connection PRAGMAs the app
/// relies on: WAL (lets the read-mostly public site and the write-occasional Studio share one file
/// without blocking each other), a busy timeout (waits out brief write locks instead of throwing),
/// and foreign-key enforcement (off by default in SQLite).
/// </summary>
internal sealed class SqliteConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<DbConnection> OpenAsync(CancellationToken ct = default)
    {
        var conn = new SqliteConnection(connectionString);
        await conn.OpenAsync(ct).ConfigureAwait(false);

        await using var pragma = conn.CreateCommand();
        pragma.CommandText = "PRAGMA journal_mode=WAL; PRAGMA busy_timeout=5000; PRAGMA foreign_keys=ON;";
        await pragma.ExecuteNonQueryAsync(ct).ConfigureAwait(false);

        return conn;
    }
}
