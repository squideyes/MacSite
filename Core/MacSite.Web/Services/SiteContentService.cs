using MacSite.Core.Abstractions;
using MacSite.Core.Models;

namespace MacSite.Web.Services;

/// <summary>
/// Per-request cache for site-wide content (settings + section configs) so the
/// announcement bar, header, footer and sections don't each re-query the database.
/// </summary>
public sealed class SiteContentService(
    ISiteSettingsRepository settingsRepo,
    ISectionConfigRepository configRepo)
{
    private SiteSettings? _settings;
    private Dictionary<string, SectionConfig>? _configs;

    public async Task<SiteSettings> GetSettingsAsync(CancellationToken ct = default) =>
        _settings ??= await settingsRepo.GetAsync(ct);

    public async Task<SectionConfig> GetConfigAsync(string key, CancellationToken ct = default)
    {
        _configs ??= (await configRepo.GetAllAsync(ct)).ToDictionary(c => c.Key, StringComparer.OrdinalIgnoreCase);
        return _configs.TryGetValue(key, out var cfg) ? cfg : new SectionConfig { Key = key, DisplayName = key };
    }
    public static string SlickOptions(SectionConfig config)
    {
        // The hero is always one full-width slide, including when its settings row is absent.
        var isHero = config.Key == SectionConfig.Hero;
        var desktop = isHero ? 1 : Math.Clamp(config.SlidesVisibleDesktop, 1, 8);
        var mobile = isHero ? 1 : Math.Clamp(config.SlidesVisibleMobile, 1, 4);
        return System.Text.Json.JsonSerializer.Serialize(new
        {
            autoplay = config.Autoplay,
            autoplaySpeed = Math.Clamp(config.IntervalMs, 1000, 120000),
            dots = config.ShowDots,
            arrows = config.ShowArrows,
            slidesToShow = desktop,
            fade = isHero,
            responsive = new[] { new { breakpoint = 992, settings = new { slidesToShow = mobile } } }
        });
    }
}
