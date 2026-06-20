using MacSite.Core.Enums;

namespace MacSite.Core.Models;

/// <summary>A full-bleed slide in the homepage hero carousel.</summary>
public sealed class HeroSlide
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ImageUrl { get; set; } = string.Empty;
    public string? Alt { get; set; }
    /// <summary>Optional per-slide headline; falls back to the site default when null.</summary>
    public string? Headline { get; set; }
    public Guid? ProductionId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>A testimonial/press quote shown in the "Applause / From Stage to Page" carousel.</summary>
public sealed class Testimonial
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Quote { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;     // e.g. "NEW YORK TIMES"
    public string? ImageUrl { get; set; }
    public Guid? ProductionId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>A press clipping shown under "Featured in the Spotlight".</summary>
public sealed class PressItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public PressCategory Category { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Url { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>A downloadable resource (publicity pack, CV, etc.) stored on Spaces.</summary>
public sealed class MediaDownload
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DownloadKind Kind { get; set; } = DownloadKind.PublicityResource;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    /// <summary>Newline-separated thumbnail URLs (the taped polaroid images).</summary>
    public string? ThumbnailUrls { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public IEnumerable<string> Thumbnails =>
        (ThumbnailUrls ?? string.Empty)
        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

/// <summary>A newsletter subscriber captured by the "Sign Up" form.</summary>
public sealed class Subscriber
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>A message submitted via the Contacts page form.</summary>
public sealed class ContactMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>A Studio administrator (cookie-auth, BCrypt-hashed password).</summary>
public sealed class AdminUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; set; }
}
