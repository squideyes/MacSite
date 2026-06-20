using MacSite.Core.Abstractions;
using MacSite.Core.Enums;
using MacSite.Core.Models;
using MacSite.Data.Db;
using Dapper;

namespace MacSite.Data.Repositories;

internal sealed class ProductionRepository(IDbConnectionFactory factory) : IProductionRepository
{
    private const string SelectColumns = """
        id, slug, title, venue, year, director, composer_or_author, music_director,
        lights, scenery, choreography, photographer, category, caption_scene,
        show_in_slider, show_in_grid, is_published, sort_order,
        is_deleted, deleted_at, created_at, updated_at
        """;

    public async Task<IReadOnlyList<Production>> GetAllAsync(bool includeUnpublished = false, CancellationToken ct = default)
    {
        var sql = $"""
            SELECT {SelectColumns} FROM productions
            WHERE is_deleted = FALSE {(includeUnpublished ? "" : "AND is_published = TRUE")}
            ORDER BY sort_order, title
            """;
        return await QueryWithImagesAsync(sql, null, ct);
    }

    public async Task<IReadOnlyList<Production>> GetForSliderAsync(CancellationToken ct = default)
    {
        const string sql = $"""
            SELECT {SelectColumns} FROM productions
            WHERE is_deleted = FALSE AND is_published = TRUE AND show_in_slider = TRUE
            ORDER BY sort_order, title
            """;
        return await QueryWithImagesAsync(sql, null, ct);
    }

    public async Task<IReadOnlyList<Production>> GetForGridAsync(ProductionCategory? category = null, CancellationToken ct = default)
    {
        var sql = $"""
            SELECT {SelectColumns} FROM productions
            WHERE is_deleted = FALSE AND is_published = TRUE AND show_in_grid = TRUE
            {(category is null ? "" : "AND category = @Category")}
            ORDER BY sort_order, title
            """;
        return await QueryWithImagesAsync(sql, new { Category = (int?)category }, ct);
    }

    public async Task<Production?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = $"SELECT {SelectColumns} FROM productions WHERE id = @id AND is_deleted = FALSE";
        var list = await QueryWithImagesAsync(sql, new { id }, ct);
        return list.FirstOrDefault();
    }

    public async Task<Production?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        const string sql = $"SELECT {SelectColumns} FROM productions WHERE slug = @slug AND is_deleted = FALSE";
        var list = await QueryWithImagesAsync(sql, new { slug }, ct);
        return list.FirstOrDefault();
    }

    public async Task<Guid> UpsertAsync(Production p, CancellationToken ct = default)
    {
        p.UpdatedAt = DateTimeOffset.UtcNow;
        const string sql = """
            INSERT INTO productions
                (id, slug, title, venue, year, director, composer_or_author, music_director,
                 lights, scenery, choreography, photographer, category, caption_scene,
                 show_in_slider, show_in_grid, is_published, sort_order,
                 is_deleted, deleted_at, created_at, updated_at)
            VALUES
                (@Id, @Slug, @Title, @Venue, @Year, @Director, @ComposerOrAuthor, @MusicDirector,
                 @Lights, @Scenery, @Choreography, @Photographer, @Category, @CaptionScene,
                 @ShowInSlider, @ShowInGrid, @IsPublished, @SortOrder,
                 @IsDeleted, @DeletedAt, @CreatedAt, @UpdatedAt)
            ON CONFLICT (id) DO UPDATE SET
                slug = EXCLUDED.slug, title = EXCLUDED.title, venue = EXCLUDED.venue, year = EXCLUDED.year,
                director = EXCLUDED.director, composer_or_author = EXCLUDED.composer_or_author,
                music_director = EXCLUDED.music_director, lights = EXCLUDED.lights, scenery = EXCLUDED.scenery,
                choreography = EXCLUDED.choreography, photographer = EXCLUDED.photographer,
                category = EXCLUDED.category, caption_scene = EXCLUDED.caption_scene,
                show_in_slider = EXCLUDED.show_in_slider, show_in_grid = EXCLUDED.show_in_grid,
                is_published = EXCLUDED.is_published, sort_order = EXCLUDED.sort_order,
                updated_at = EXCLUDED.updated_at
            RETURNING id;
            """;
        await using var conn = await factory.OpenAsync(ct);
        return await conn.ExecuteScalarAsync<Guid>(new CommandDefinition(sql, Param(p), cancellationToken: ct));
    }

    public async Task ReplaceImagesAsync(Guid productionId, IEnumerable<ProductionImage> images, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "DELETE FROM production_images WHERE production_id = @productionId",
            new { productionId }, tx, cancellationToken: ct));
        const string insert = """
            INSERT INTO production_images (id, production_id, url, alt, sort_order, is_primary)
            VALUES (@Id, @ProductionId, @Url, @Alt, @SortOrder, @IsPrimary)
            """;
        foreach (var img in images)
        {
            img.ProductionId = productionId;
            await conn.ExecuteAsync(new CommandDefinition(insert, img, tx, cancellationToken: ct));
        }
        await tx.CommitAsync(ct);
    }

    public async Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        await using var conn = await factory.OpenAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        for (var i = 0; i < orderedIds.Count; i++)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                "UPDATE productions SET sort_order = @order, updated_at = @now WHERE id = @id",
                new { order = i, now, id = orderedIds[i] }, tx, cancellationToken: ct));
        }
        await tx.CommitAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "UPDATE productions SET is_deleted = TRUE, deleted_at = @now, updated_at = @now WHERE id = @id",
            new { now = DateTimeOffset.UtcNow, id }, cancellationToken: ct));
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task<IReadOnlyList<Production>> QueryWithImagesAsync(string sql, object? param, CancellationToken ct)
    {
        await using var conn = await factory.OpenAsync(ct);
        var productions = (await conn.QueryAsync<Production>(new CommandDefinition(sql, param, cancellationToken: ct))).ToList();
        if (productions.Count == 0) return productions;

        var ids = productions.Select(p => p.Id).ToArray();
        const string imgSql = """
            SELECT id, production_id, url, alt, sort_order, is_primary
            FROM production_images WHERE production_id IN @ids ORDER BY sort_order
            """;
        var images = await conn.QueryAsync<ProductionImage>(new CommandDefinition(imgSql, new { ids }, cancellationToken: ct));
        var byProduction = images.GroupBy(i => i.ProductionId).ToDictionary(g => g.Key, g => (IReadOnlyList<ProductionImage>)g.ToList());

        foreach (var p in productions)
            p.Images = byProduction.TryGetValue(p.Id, out var imgs) ? imgs : [];

        return productions;
    }

    private static object Param(Production p) => new
    {
        p.Id, p.Slug, p.Title, p.Venue, p.Year, p.Director, p.ComposerOrAuthor, p.MusicDirector,
        p.Lights, p.Scenery, p.Choreography, p.Photographer, Category = (int)p.Category, p.CaptionScene,
        p.ShowInSlider, p.ShowInGrid, p.IsPublished, p.SortOrder,
        p.IsDeleted, p.DeletedAt, p.CreatedAt, p.UpdatedAt
    };
}
