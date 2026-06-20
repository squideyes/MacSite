namespace MacSite.Core.Models;

/// <summary>
/// Layout/behaviour knobs for a configurable section (hero, slider, applause, grid),
/// editable in the Studio. Keyed by <see cref="Key"/>. These let the owner tune the
/// Applause, Slider, and Grid sections without code changes.
/// </summary>
public sealed class SectionConfig
{
    // Well-known keys.
    public const string Hero = "hero";
    public const string Slider = "slider";       // homepage "Theatrical Designs" carousel
    public const string Applause = "applause";   // "From Stage to Page" testimonial carousel
    public const string Grid = "grid";           // Portfolio Expand filterable grid

    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public bool Autoplay { get; set; } = true;
    public int IntervalMs { get; set; } = 6000;
    public bool ShowDots { get; set; } = true;
    public bool ShowArrows { get; set; }
    public bool ShowCaptions { get; set; } = true;

    public int SlidesVisibleDesktop { get; set; } = 2;
    public int SlidesVisibleMobile { get; set; } = 1;

    /// <summary>Grid only: which category tabs are enabled, as a CSV of enum names.
    /// Empty/null means "all".</summary>
    public string? EnabledCategoriesCsv { get; set; }

    /// <summary>Grid only: how many items to show before "View More".</summary>
    public int GridInitialCount { get; set; } = 8;

    /// <summary>Escape hatch for future per-section options without a schema change.</summary>
    public string? ExtraJson { get; set; }
}
