using MacSite.Core.Abstractions;
using MacSite.Core.Models;
using MacSite.Data.Db;
using Dapper;

namespace MacSite.Data.Repositories;

internal sealed class SubscriberRepository(IDbConnectionFactory factory) : ISubscriberRepository
{
    public async Task<bool> AddAsync(string email, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        // Idempotent: returns false if the email already exists.
        var rows = await conn.ExecuteAsync(new CommandDefinition("""
            INSERT INTO subscribers (id, email, is_confirmed, created_at)
            VALUES (@id, @email, FALSE, @createdAt)
            ON CONFLICT (email) DO NOTHING
            """, new { id = Guid.NewGuid(), email = email.Trim().ToLowerInvariant(), createdAt = DateTimeOffset.UtcNow }, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<IReadOnlyList<Subscriber>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<Subscriber>(new CommandDefinition(
            "SELECT id, email, is_confirmed, created_at FROM subscribers ORDER BY created_at DESC", cancellationToken: ct))).ToList();
    }
}

internal sealed class ContactMessageRepository(IDbConnectionFactory factory) : IContactMessageRepository
{
    public async Task AddAsync(ContactMessage m, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition("""
            INSERT INTO contact_messages (id, name, email, subject, message, is_read, created_at)
            VALUES (@Id, @Name, @Email, @Subject, @Message, @IsRead, @CreatedAt)
            """, m, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<ContactMessage>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<ContactMessage>(new CommandDefinition(
            "SELECT id, name, email, subject, message, is_read, created_at FROM contact_messages ORDER BY created_at DESC",
            cancellationToken: ct))).ToList();
    }

    public async Task MarkReadAsync(Guid id, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "UPDATE contact_messages SET is_read = TRUE WHERE id = @id", new { id }, cancellationToken: ct));
    }
}

internal sealed class AdminUserRepository(IDbConnectionFactory factory) : IAdminUserRepository
{
    private const string Cols = "id, email, password_hash, display_name, created_at, last_login_at";

    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<AdminUser>(new CommandDefinition(
            $"SELECT {Cols} FROM admin_users WHERE lower(email) = lower(@email)", new { email }, cancellationToken: ct));
    }

    public async Task<Guid> UpsertAsync(AdminUser u, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO admin_users (id, email, password_hash, display_name, created_at, last_login_at)
            VALUES (@Id, @Email, @PasswordHash, @DisplayName, @CreatedAt, @LastLoginAt)
            ON CONFLICT (id) DO UPDATE SET
                email = EXCLUDED.email, password_hash = EXCLUDED.password_hash, display_name = EXCLUDED.display_name
            RETURNING id;
            """;
        await using var conn = await factory.OpenAsync(ct);
        return await conn.ExecuteScalarAsync<Guid>(new CommandDefinition(sql, u, cancellationToken: ct));
    }

    public async Task UpdateLastLoginAsync(Guid id, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "UPDATE admin_users SET last_login_at = @now WHERE id = @id", new { now = DateTimeOffset.UtcNow, id }, cancellationToken: ct));
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return await conn.ExecuteScalarAsync<int>(new CommandDefinition("SELECT count(*) FROM admin_users", cancellationToken: ct));
    }
}
