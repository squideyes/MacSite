using System.Globalization;
using System.Text;

namespace MacSite.Core.Common;

public static class Slug
{
    /// <summary>Produces a lowercase, URL-safe slug from arbitrary text.</summary>
    public static string From(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        // Strip diacritics (é -> e), then keep alphanumerics, turn the rest into single hyphens.
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        var lastWasHyphen = false;

        foreach (var ch in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category == UnicodeCategory.NonSpacingMark) continue;

            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(char.ToLowerInvariant(ch));
                lastWasHyphen = false;
            }
            else if (!lastWasHyphen && sb.Length > 0)
            {
                sb.Append('-');
                lastWasHyphen = true;
            }
        }

        return sb.ToString().Trim('-');
    }
}
