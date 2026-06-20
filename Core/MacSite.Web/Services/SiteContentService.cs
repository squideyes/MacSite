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
}
