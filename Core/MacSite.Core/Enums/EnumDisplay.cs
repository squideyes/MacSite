namespace MacSite.Core.Enums;

/// <summary>Human-facing display labels for domain enums, matching the design exactly.</summary>
public static class EnumDisplay
{
    public static string ToDisplay(this ProductionCategory category) => category switch
    {
        ProductionCategory.Opera => "OPERA",
        ProductionCategory.Musicals => "MUSICALS",
        ProductionCategory.Dance => "DANCE",
        ProductionCategory.Plays => "PLAYS",
        ProductionCategory.FilmAndMedia => "FILM & MEDIA",
        ProductionCategory.Academic => "ACADEMIC",
        _ => category.ToString().ToUpperInvariant()
    };

    public static string ToDisplay(this PressCategory category) => category switch
    {
        PressCategory.InterviewsAndProfiles => "INTERVIEWS & PROFILES",
        PressCategory.BooksAndAcademic => "BOOKS & ACADEMIC",
        PressCategory.ReviewsAndAcclaim => "REVIEWS & ACCLAIM",
        _ => category.ToString().ToUpperInvariant()
    };

    /// <summary>All categories in display order (for rendering the filter tab row).</summary>
    public static readonly IReadOnlyList<ProductionCategory> CategoryOrder =
    [
        ProductionCategory.Opera,
        ProductionCategory.Musicals,
        ProductionCategory.Dance,
        ProductionCategory.Plays,
        ProductionCategory.FilmAndMedia,
        ProductionCategory.Academic
    ];
}
