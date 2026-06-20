using MacSite.Core.Abstractions;
using MacSite.Core.Enums;
using MacSite.Core.Models;
using MacSite.Data.Db;
using Dapper;

namespace MacSite.Data.Repositories;

internal sealed class HeroSlideRepository(IDbConnectionFactory factory) : IHeroSlideRepository
{
    private const string Cols = "id, image_url, alt, headline, production_id, sort_order, is_active";

    public async Task<IReadOnlyList<HeroSlide>> GetActiveAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<HeroSlide>(new CommandDefinition(
            $"SELECT {Cols} FROM hero_slides WHERE is_active = TRUE ORDER BY sort_order", cancellationToken: ct))).ToList();
    }

    public async Task<IReadOnlyList<HeroSlide>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<HeroSlide>(new CommandDefinition(
            $"SELECT {Cols} FROM hero_slides ORDER BY sort_order", cancellationToken: ct))).ToList();
    }

    public async Task<Guid> UpsertAsync(HeroSlide s, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO hero_slides (id, image_url, alt, headline, production_id, sort_order, is_active)
            VALUES (@Id, @ImageUrl, @Alt, @Headline, @ProductionId, @SortOrder, @IsActive)
            ON CONFLICT (id) DO UPDATE SET
                image_url = EXCLUDED.image_url, alt = EXCLUDED.alt, headline = EXCLUDED.headline,
                production_id = EXCLUDED.production_id, sort_order = EXCLUDED.sort_order, is_active = EXCLUDED.is_active
            RETURNING id;
            """;
        await using var conn = await factory.OpenAsync(ct);
        return await conn.ExecuteScalarAsync<Guid>(new CommandDefinition(sql, s, cancellationToken: ct));
    }

    public Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken ct = default) =>
        RepoHelpers.ReorderAsync(factory, "hero_slides", orderedIds, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition("DELETE FROM hero_slides WHERE id = @id", new { id }, cancellationToken: ct));
    }
}

internal sealed class TestimonialRepository(IDbConnectionFactory factory) : ITestimonialRepository
{
    private const string Cols = "id, quote, source, image_url, production_id, sort_order, is_active";

    public async Task<IReadOnlyList<Testimonial>> GetActiveAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<Testimonial>(new CommandDefinition(
            $"SELECT {Cols} FROM testimonials WHERE is_active = TRUE ORDER BY sort_order", cancellationToken: ct))).ToList();
    }

    public async Task<IReadOnlyList<Testimonial>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<Testimonial>(new CommandDefinition(
            $"SELECT {Cols} FROM testimonials ORDER BY sort_order", cancellationToken: ct))).ToList();
    }

    public async Task<Guid> UpsertAsync(Testimonial t, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO testimonials (id, quote, source, image_url, production_id, sort_order, is_active)
            VALUES (@Id, @Quote, @Source, @ImageUrl, @ProductionId, @SortOrder, @IsActive)
            ON CONFLICT (id) DO UPDATE SET
                quote = EXCLUDED.quote, source = EXCLUDED.source, image_url = EXCLUDED.image_url,
                production_id = EXCLUDED.production_id, sort_order = EXCLUDED.sort_order, is_active = EXCLUDED.is_active
            RETURNING id;
            """;
        await using var conn = await factory.OpenAsync(ct);
        return await conn.ExecuteScalarAsync<Guid>(new CommandDefinition(sql, t, cancellationToken: ct));
    }

    public Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken ct = default) =>
        RepoHelpers.ReorderAsync(factory, "testimonials", orderedIds, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition("DELETE FROM testimonials WHERE id = @id", new { id }, cancellationToken: ct));
    }
}

internal sealed class PressRepository(IDbConnectionFactory factory) : IPressRepository
{
    private const string Cols = "id, category, image_url, title, description, url, sort_order, is_active";

    public async Task<IReadOnlyList<PressItem>> GetActiveAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<PressItem>(new CommandDefinition(
            $"SELECT {Cols} FROM press_items WHERE is_active = TRUE ORDER BY sort_order", cancellationToken: ct))).ToList();
    }

    public async Task<IReadOnlyList<PressItem>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<PressItem>(new CommandDefinition(
            $"SELECT {Cols} FROM press_items ORDER BY sort_order", cancellationToken: ct))).ToList();
    }

    public async Task<Guid> UpsertAsync(PressItem p, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO press_items (id, category, image_url, title, description, url, sort_order, is_active)
            VALUES (@Id, @Category, @ImageUrl, @Title, @Description, @Url, @SortOrder, @IsActive)
            ON CONFLICT (id) DO UPDATE SET
                category = EXCLUDED.category, image_url = EXCLUDED.image_url, title = EXCLUDED.title,
                description = EXCLUDED.description, url = EXCLUDED.url, sort_order = EXCLUDED.sort_order, is_active = EXCLUDED.is_active
            RETURNING id;
            """;
        await using var conn = await factory.OpenAsync(ct);
        return await conn.ExecuteScalarAsync<Guid>(new CommandDefinition(
            sql, new { p.Id, Category = (int)p.Category, p.ImageUrl, p.Title, p.Description, p.Url, p.SortOrder, p.IsActive },
            cancellationToken: ct));
    }

    public Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken ct = default) =>
        RepoHelpers.ReorderAsync(factory, "press_items", orderedIds, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition("DELETE FROM press_items WHERE id = @id", new { id }, cancellationToken: ct));
    }
}

internal sealed class DownloadRepository(IDbConnectionFactory factory) : IDownloadRepository
{
    private const string Cols = "id, kind, title, description, file_url, thumbnail_urls, sort_order, is_active";

    public async Task<IReadOnlyList<MediaDownload>> GetActiveAsync(DownloadKind? kind = null, CancellationToken ct = default)
    {
        var sql = $"SELECT {Cols} FROM media_downloads WHERE is_active = TRUE {(kind is null ? "" : "AND kind = @kind")} ORDER BY sort_order";
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<MediaDownload>(new CommandDefinition(sql, new { kind = (int?)kind }, cancellationToken: ct))).ToList();
    }

    public async Task<IReadOnlyList<MediaDownload>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<MediaDownload>(new CommandDefinition(
            $"SELECT {Cols} FROM media_downloads ORDER BY sort_order", cancellationToken: ct))).ToList();
    }

    public async Task<Guid> UpsertAsync(MediaDownload d, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO media_downloads (id, kind, title, description, file_url, thumbnail_urls, sort_order, is_active)
            VALUES (@Id, @Kind, @Title, @Description, @FileUrl, @ThumbnailUrls, @SortOrder, @IsActive)
            ON CONFLICT (id) DO UPDATE SET
                kind = EXCLUDED.kind, title = EXCLUDED.title, description = EXCLUDED.description,
                file_url = EXCLUDED.file_url, thumbnail_urls = EXCLUDED.thumbnail_urls,
                sort_order = EXCLUDED.sort_order, is_active = EXCLUDED.is_active
            RETURNING id;
            """;
        await using var conn = await factory.OpenAsync(ct);
        return await conn.ExecuteScalarAsync<Guid>(new CommandDefinition(
            sql, new { d.Id, Kind = (int)d.Kind, d.Title, d.Description, d.FileUrl, d.ThumbnailUrls, d.SortOrder, d.IsActive },
            cancellationToken: ct));
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition("DELETE FROM media_downloads WHERE id = @id", new { id }, cancellationToken: ct));
    }
}

/// <summary>Shared repository helpers.</summary>
internal static class RepoHelpers
{
    public static async Task ReorderAsync(IDbConnectionFactory factory, string table, IReadOnlyList<Guid> orderedIds, CancellationToken ct)
    {
        await using var conn = await factory.OpenAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        for (var i = 0; i < orderedIds.Count; i++)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                $"UPDATE {table} SET sort_order = @order WHERE id = @id",
                new { order = i, id = orderedIds[i] }, tx, cancellationToken: ct));
        }
        await tx.CommitAsync(ct);
    }
}
