using MacSite.Core.Enums;

namespace MacSite.Core.Models;

/// <summary>
/// A theatrical/opera production in Marie's portfolio. Sourced from the site metadata sheet.
/// Drives the homepage Portfolio Slider and the Portfolio Expand grid.
/// </summary>
public sealed class Production
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>URL-safe identifier used for /portfolio/{slug} detail pages.</summary>
    public string Slug { get; set; } = string.Empty;

    // ── Metadata (from Chiment Site Metadata.pdf) ──────────────────────────────
    public string Title { get; set; } = string.Empty;          // "Show"
    public string Venue { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Director { get; set; }
    public string? ComposerOrAuthor { get; set; }
    public string? MusicDirector { get; set; }
    public string? Lights { get; set; }
    public string? Scenery { get; set; }
    public string? Choreography { get; set; }
    public string? Photographer { get; set; }                  // "Photos"

    // ── Presentation ───────────────────────────────────────────────────────────
    public ProductionCategory Category { get; set; } = ProductionCategory.Opera;

    /// <summary>Optional scene/aria lines shown under the title on the card overlay
    /// (e.g. "Maddalena's Seduction / Sparfucile's Aria"). Newline-separated.</summary>
    public string? CaptionScene { get; set; }

    public bool ShowInSlider { get; set; }
    public bool ShowInGrid { get; set; } = true;
    public bool IsPublished { get; set; } = true;
    public int SortOrder { get; set; }

    // ── Auditing / soft-delete ─────────────────────────────────────────────────
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Ordered images for this production (hydrated by the repository).</summary>
    public IReadOnlyList<ProductionImage> Images { get; set; } = [];

    /// <summary>The card overlay heading, e.g. "RIGOLETTO SEATTLE OPERA".</summary>
    public string CaptionTitle => $"{Title} {Venue}".Trim();

    public string? PrimaryImageUrl => Images.FirstOrDefault(i => i.IsPrimary)?.Url
                                      ?? Images.FirstOrDefault()?.Url;
}

/// <summary>An image attached to a <see cref="Production"/> (stored on DigitalOcean Spaces).</summary>
public sealed class ProductionImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductionId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Alt { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
