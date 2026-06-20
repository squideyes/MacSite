namespace MacSite.Core.Models;

/// <summary>
/// Site-wide editable settings (a single row). Edited in the Studio; read by the public site.
/// </summary>
public sealed class SiteSettings
{
    /// <summary>Fixed primary key for the singleton settings row.</summary>
    public static readonly Guid SingletonId = new("00000000-0000-0000-0000-0000000000a1");

    public Guid Id { get; set; } = SingletonId;

    public string BrandName { get; set; } = "MARIE ANNE CHIMENT";

    /// <summary>Top announcement bar. May contain limited inline markup (&lt;u&gt;).</summary>
    public string AnnouncementText { get; set; } =
        "AWARD-WINNING <u>SET DESIGNS</u> AND <u>COSTUME DESIGNS</u> THAT BRING STORIES TO LIFE.";

    public string DefaultHeroHeadline { get; set; } =
        "TRANSFORMING<br>STORYTELLING THROUGH SET<br>& COSTUME DESIGN";

    public string? HeroVideoUrl { get; set; }
    public string HeroVideoBadgeText { get; set; } = "THE STAGE COMES ALIVE IN VIDEO";

    // ── Bio / "A Life in Design" ───────────────────────────────────────────────
    public string BioBody { get; set; } = string.Empty;
    public string? PortraitImageUrl { get; set; }
    public string? CvFileUrl { get; set; }

    // ── Newsletter ─────────────────────────────────────────────────────────────
    public string NewsletterHeading { get; set; } = "BEHIND THE SCENES WITH MARIE ANNE CHIMENT";

    // ── Contact ────────────────────────────────────────────────────────────────
    public string Phone { get; set; } = "+1 (215) 279-8868";
    public string Email { get; set; } = "marie@macsite.com";

    // ── Social ─────────────────────────────────────────────────────────────────
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? YoutubeUrl { get; set; }

    public string CopyrightName { get; set; } = "MARIE ANNE CHIMENT";

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
