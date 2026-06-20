using MacSite.Core.Abstractions;
using MacSite.Core.Models;
using MacSite.Data.Db;
using Dapper;

namespace MacSite.Data.Repositories;

internal sealed class SiteSettingsRepository(IDbConnectionFactory factory) : ISiteSettingsRepository
{
    private const string Cols = """
        id, brand_name, announcement_text, default_hero_headline, hero_video_url, hero_video_badge_text,
        bio_body, portrait_image_url, cv_file_url, newsletter_heading, phone, email,
        facebook_url, instagram_url, youtube_url, copyright_name, updated_at
        """;

    public async Task<SiteSettings> GetAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        var settings = await conn.QuerySingleOrDefaultAsync<SiteSettings>(new CommandDefinition(
            $"SELECT {Cols} FROM site_settings WHERE id = @id", new { id = SiteSettings.SingletonId }, cancellationToken: ct));
        return settings ?? new SiteSettings();
    }

    public async Task UpdateAsync(SiteSettings s, CancellationToken ct = default)
    {
        s.Id = SiteSettings.SingletonId;
        s.UpdatedAt = DateTimeOffset.UtcNow;
        const string sql = """
            INSERT INTO site_settings
                (id, brand_name, announcement_text, default_hero_headline, hero_video_url, hero_video_badge_text,
                 bio_body, portrait_image_url, cv_file_url, newsletter_heading, phone, email,
                 facebook_url, instagram_url, youtube_url, copyright_name, updated_at)
            VALUES
                (@Id, @BrandName, @AnnouncementText, @DefaultHeroHeadline, @HeroVideoUrl, @HeroVideoBadgeText,
                 @BioBody, @PortraitImageUrl, @CvFileUrl, @NewsletterHeading, @Phone, @Email,
                 @FacebookUrl, @InstagramUrl, @YoutubeUrl, @CopyrightName, @UpdatedAt)
            ON CONFLICT (id) DO UPDATE SET
                brand_name = EXCLUDED.brand_name, announcement_text = EXCLUDED.announcement_text,
                default_hero_headline = EXCLUDED.default_hero_headline, hero_video_url = EXCLUDED.hero_video_url,
                hero_video_badge_text = EXCLUDED.hero_video_badge_text, bio_body = EXCLUDED.bio_body,
                portrait_image_url = EXCLUDED.portrait_image_url, cv_file_url = EXCLUDED.cv_file_url,
                newsletter_heading = EXCLUDED.newsletter_heading, phone = EXCLUDED.phone, email = EXCLUDED.email,
                facebook_url = EXCLUDED.facebook_url, instagram_url = EXCLUDED.instagram_url,
                youtube_url = EXCLUDED.youtube_url, copyright_name = EXCLUDED.copyright_name,
                updated_at = EXCLUDED.updated_at;
            """;
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, s, cancellationToken: ct));
    }
}

internal sealed class SectionConfigRepository(IDbConnectionFactory factory) : ISectionConfigRepository
{
    private const string Cols = """
        key, display_name, autoplay, interval_ms, show_dots, show_arrows, show_captions,
        slides_visible_desktop, slides_visible_mobile, enabled_categories_csv, grid_initial_count, extra_json
        """;

    public async Task<SectionConfig?> GetAsync(string key, CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<SectionConfig>(new CommandDefinition(
            $"SELECT {Cols} FROM section_configs WHERE key = @key", new { key }, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<SectionConfig>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await factory.OpenAsync(ct);
        return (await conn.QueryAsync<SectionConfig>(new CommandDefinition(
            $"SELECT {Cols} FROM section_configs ORDER BY key", cancellationToken: ct))).ToList();
    }

    public async Task UpsertAsync(SectionConfig c, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO section_configs
                (key, display_name, autoplay, interval_ms, show_dots, show_arrows, show_captions,
                 slides_visible_desktop, slides_visible_mobile, enabled_categories_csv, grid_initial_count, extra_json)
            VALUES
                (@Key, @DisplayName, @Autoplay, @IntervalMs, @ShowDots, @ShowArrows, @ShowCaptions,
                 @SlidesVisibleDesktop, @SlidesVisibleMobile, @EnabledCategoriesCsv, @GridInitialCount, @ExtraJson)
            ON CONFLICT (key) DO UPDATE SET
                display_name = EXCLUDED.display_name, autoplay = EXCLUDED.autoplay, interval_ms = EXCLUDED.interval_ms,
                show_dots = EXCLUDED.show_dots, show_arrows = EXCLUDED.show_arrows, show_captions = EXCLUDED.show_captions,
                slides_visible_desktop = EXCLUDED.slides_visible_desktop, slides_visible_mobile = EXCLUDED.slides_visible_mobile,
                enabled_categories_csv = EXCLUDED.enabled_categories_csv, grid_initial_count = EXCLUDED.grid_initial_count,
                extra_json = EXCLUDED.extra_json;
            """;
        await using var conn = await factory.OpenAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, c, cancellationToken: ct));
    }
}
