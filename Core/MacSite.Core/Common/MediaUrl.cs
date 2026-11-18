namespace MacSite.Core.Common;

/// <summary>Validates Studio image URLs and makes local paths site-relative.</summary>
public static class MediaUrl
{
    public static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        if (value.Any(char.IsControl) || value.Contains('\\') || value.StartsWith("//", StringComparison.Ordinal))
            return null;
        if (Uri.TryCreate(value, UriKind.Absolute, out var absolute) && absolute.Scheme is "https" or "http")
            return value;
        // Reject schemes, fragment/query-only values, and traversal rather than turning them into local paths.
        var path = value.Split('?', '#')[0];
        if (path.Length == 0 || path == "/" || path.Contains(':')) return null;
        var decoded = Uri.UnescapeDataString(path);
        if (decoded.Contains('\\') || decoded.Any(char.IsControl) || decoded.StartsWith("//", StringComparison.Ordinal)
            || decoded.Split('/').Any(segment => segment is "." or "..")) return null;
        return value.StartsWith('/') ? value : "/" + value;
    }
}
