using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;

namespace MacSite.Web.Services;

internal static class ContentLinks
{
    public static string? SafeUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        if (value.Any(char.IsControl) || value.Contains('\\')) return null;
        if (value.StartsWith('/') && !value.StartsWith("//", StringComparison.Ordinal)) return value;
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme is "https" or "http"
            ? value : null;
    }

    public static string? HeroVideoUrl(string? value)
    {
        var url = SafeUrl(value);
        // Older databases may still contain the original seed. Do not rewrite existing settings.
        return url is not null && !url.Contains("dQw4w9WgXcQ", StringComparison.OrdinalIgnoreCase) ? url : null;
    }

    // Editors may underline an announcement or insert headline breaks. Encode all other markup.
    public static MarkupString InlineText(string? value)
    {
        var encoded = HtmlEncoder.Default.Encode(value ?? "");
        foreach (var tag in new[] { "br", "br/", "br /", "u", "/u" })
            encoded = encoded.Replace($"&lt;{tag}&gt;", $"<{tag}>", StringComparison.OrdinalIgnoreCase);
        return new MarkupString(encoded);
    }
}
