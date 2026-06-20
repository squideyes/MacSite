using MacSite.Core.Enums;
using MacSite.Core.Models;
using FluentAssertions;

namespace MacSite.UnitTests;

public class EnumDisplayTests
{
    [Theory]
    [InlineData(ProductionCategory.Opera, "OPERA")]
    [InlineData(ProductionCategory.FilmAndMedia, "FILM & MEDIA")]
    [InlineData(ProductionCategory.Academic, "ACADEMIC")]
    public void Category_display_matches_design(ProductionCategory category, string expected)
    {
        category.ToDisplay().Should().Be(expected);
    }

    [Theory]
    [InlineData(PressCategory.InterviewsAndProfiles, "INTERVIEWS & PROFILES")]
    [InlineData(PressCategory.BooksAndAcademic, "BOOKS & ACADEMIC")]
    [InlineData(PressCategory.ReviewsAndAcclaim, "REVIEWS & ACCLAIM")]
    public void Press_display_matches_design(PressCategory category, string expected)
    {
        category.ToDisplay().Should().Be(expected);
    }

    [Fact]
    public void CategoryOrder_lists_all_six_in_order()
    {
        EnumDisplay.CategoryOrder.Should().HaveCount(6);
        EnumDisplay.CategoryOrder[0].Should().Be(ProductionCategory.Opera);
        EnumDisplay.CategoryOrder[^1].Should().Be(ProductionCategory.Academic);
    }
}

public class ProductionTests
{
    [Fact]
    public void CaptionTitle_combines_title_and_venue()
    {
        var p = new Production { Title = "Rigoletto", Venue = "Seattle Opera" };
        p.CaptionTitle.Should().Be("Rigoletto Seattle Opera");
    }

    [Fact]
    public void PrimaryImageUrl_prefers_primary_then_first()
    {
        var p = new Production
        {
            Images =
            [
                new ProductionImage { Url = "/a.jpg", IsPrimary = false, SortOrder = 0 },
                new ProductionImage { Url = "/b.jpg", IsPrimary = true, SortOrder = 1 }
            ]
        };
        p.PrimaryImageUrl.Should().Be("/b.jpg");
    }

    [Fact]
    public void PrimaryImageUrl_is_null_when_no_images()
    {
        new Production().PrimaryImageUrl.Should().BeNull();
    }
}

public class MediaDownloadTests
{
    [Fact]
    public void Thumbnails_splits_newline_separated_urls()
    {
        var d = new MediaDownload { ThumbnailUrls = "/one.jpg\n/two.jpg\n" };
        d.Thumbnails.Should().BeEquivalentTo("/one.jpg", "/two.jpg");
    }

    [Fact]
    public void Thumbnails_empty_when_null()
    {
        new MediaDownload().Thumbnails.Should().BeEmpty();
    }
}

public class SiteSettingsTests
{
    [Fact]
    public void Singleton_id_is_fixed()
    {
        new SiteSettings().Id.Should().Be(SiteSettings.SingletonId);
    }
}
