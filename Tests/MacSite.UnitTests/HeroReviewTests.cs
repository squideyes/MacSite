using System.Text.Json;
using MacSite.Core.Abstractions;
using MacSite.Core.Common;
using MacSite.Core.Models;
using MacSite.Web.Components.Sections;
using MacSite.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MacSite.UnitTests;

public class HeroReviewTests
{
    [Theory]
    [InlineData("media/hero/x.jpg", "/media/hero/x.jpg")]
    [InlineData(" /media/hero/x.jpg ", "/media/hero/x.jpg")]
    [InlineData("https://example.com/image.jpg?v=1", "https://example.com/image.jpg?v=1")]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("javascript:alert(1)", null)]
    [InlineData("data:image/svg+xml,test", null)]
    [InlineData("//example.com/x.jpg", null)]
    [InlineData("/\\example.com/x.jpg", null)]
    [InlineData("../secret.jpg", null)]
    [InlineData("/%2e%2e/secret.jpg", null)]
    [InlineData("/%2fexample.com/x.jpg", null)]
    [InlineData("#image", null)]
    public void Image_paths_are_normalized_or_rejected(string? input, string? expected) =>
        Assert.Equal(expected, MediaUrl.Normalize(input));

    [Fact]
    public void Hero_always_shows_one_slide_but_other_carousels_keep_their_layout()
    {
        foreach (var desktop in new[] { 0, 2, 8 })
        {
            using var hero = JsonDocument.Parse(SiteContentService.SlickOptions(new SectionConfig
            {
                Key = SectionConfig.Hero, SlidesVisibleDesktop = desktop, SlidesVisibleMobile = 3,
                ShowArrows = true, ShowDots = false, Autoplay = false, IntervalMs = 3500
            }));
            var options = hero.RootElement;
            Assert.Equal(1, options.GetProperty("slidesToShow").GetInt32());
            Assert.Equal(1, options.GetProperty("responsive")[0].GetProperty("settings").GetProperty("slidesToShow").GetInt32());
            Assert.True(options.GetProperty("fade").GetBoolean());
            Assert.True(options.GetProperty("arrows").GetBoolean());
            Assert.False(options.GetProperty("dots").GetBoolean());
            Assert.False(options.GetProperty("autoplay").GetBoolean());
            Assert.Equal(3500, options.GetProperty("autoplaySpeed").GetInt32());
        }
        using var slider = JsonDocument.Parse(SiteContentService.SlickOptions(new SectionConfig
        {
            Key = SectionConfig.Slider, SlidesVisibleDesktop = 3, SlidesVisibleMobile = 2
        }));
        Assert.Equal(3, slider.RootElement.GetProperty("slidesToShow").GetInt32());
        Assert.False(slider.RootElement.GetProperty("fade").GetBoolean());
    }

    [Fact]
    public async Task No_active_slides_and_missing_config_render_a_bundled_fallback_without_writing_data()
    {
        await using var fixture = new HeroFixture();
        var html = await fixture.RenderAsync();
        Assert.Contains("src=\"/media/hero/hero-1.jpg\"", html);
        Assert.Contains("TRANSFORMING", html);
        Assert.Contains("ENTER PORTFOLIO", html);
        Assert.DoesNotContain("<a class=\"play-icon\"", html);
        Assert.DoesNotContain(fixture.Slides.ReceivedCalls(), c => c.GetMethodInfo().Name != nameof(IHeroSlideRepository.GetActiveAsync));
        Assert.All(fixture.Slides.ReceivedCalls(), c => Assert.True(((CancellationToken)c.GetArguments()[0]!).CanBeCanceled));
        Assert.All(fixture.Settings.ReceivedCalls(), c => Assert.True(((CancellationToken)c.GetArguments()[0]!).CanBeCanceled));
        Assert.All(fixture.Configs.ReceivedCalls(), c => Assert.True(((CancellationToken)c.GetArguments()[0]!).CanBeCanceled));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ", false)]
    [InlineData("https://youtu.be/dQw4w9WgXcQ?t=1", false)]
    [InlineData("https://youtu.be/ejKj_CQXCOU", true)]
    public async Task Badge_is_decorative_until_a_safe_nonplaceholder_video_is_configured(string? video, bool linked)
    {
        await using var fixture = new HeroFixture();
        fixture.SiteSettings.HeroVideoUrl = video;
        var html = await fixture.RenderAsync();
        Assert.Equal(linked, html.Contains("<a class=\"play-icon\""));
        if (linked)
        {
            Assert.Contains("target=\"_blank\"", html);
            Assert.Contains("rel=\"noopener\"", html);
        }
        else
            Assert.Contains("<div class=\"play-icon\" aria-hidden=\"true\"", html);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public async Task Portfolio_button_obeys_captions_and_page_context(bool captions, bool showLink, bool expected)
    {
        await using var fixture = new HeroFixture();
        fixture.Configs.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[] { new SectionConfig { Key = SectionConfig.Hero, ShowCaptions = captions } });
        var html = await fixture.RenderAsync(showLink);
        Assert.Equal(expected, html.Contains("ENTER PORTFOLIO"));
        Assert.Equal(captions, html.Contains("<h1>"));
    }

    [Fact]
    public async Task Existing_relative_images_and_editor_headlines_render_safely()
    {
        await using var fixture = new HeroFixture();
        fixture.Slides.GetActiveAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            new HeroSlide { ImageUrl = "media/hero/hero-2.jpg", Headline = "A<br><script>alert(1)</script>" }
        });
        var html = await fixture.RenderAsync();
        Assert.Contains("src=\"/media/hero/hero-2.jpg\"", html);
        Assert.Contains("A<br>&lt;script&gt;", html);
        Assert.DoesNotContain("<script>", html);
    }

    [Fact]
    public async Task Disposing_hero_cancels_its_database_call()
    {
        await using var fixture = new HeroFixture();
        CancellationToken token = default;
        fixture.Slides.GetActiveAsync(Arg.Any<CancellationToken>()).Returns(async call =>
        {
            token = call.Arg<CancellationToken>();
            await Task.Delay(Timeout.Infinite, token);
            return (IReadOnlyList<HeroSlide>)Array.Empty<HeroSlide>();
        });
        await fixture.Renderer.Dispatcher.InvokeAsync(() => fixture.Renderer.BeginRenderingComponent<ChimentHero>());
        Assert.True(token.CanBeCanceled);
        await fixture.Renderer.DisposeAsync();
        Assert.True(token.IsCancellationRequested);
    }

    private sealed class HeroFixture : IAsyncDisposable
    {
        public IHeroSlideRepository Slides { get; } = Substitute.For<IHeroSlideRepository>();
        public ISiteSettingsRepository Settings { get; } = Substitute.For<ISiteSettingsRepository>();
        public ISectionConfigRepository Configs { get; } = Substitute.For<ISectionConfigRepository>();
        public SiteSettings SiteSettings { get; } = new();
        private readonly ServiceProvider _provider;
        public HtmlRenderer Renderer { get; }

        public HeroFixture()
        {
            Slides.GetActiveAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<HeroSlide>());
            Settings.GetAsync(Arg.Any<CancellationToken>()).Returns(SiteSettings);
            Configs.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<SectionConfig>());
            _provider = new ServiceCollection().AddLogging()
                .AddSingleton(Slides).AddSingleton(Settings).AddSingleton(Configs)
                .AddScoped<SiteContentService>().BuildServiceProvider();
            Renderer = new HtmlRenderer(_provider, _provider.GetRequiredService<ILoggerFactory>());
        }

        public Task<string> RenderAsync(bool showLink = true) => Renderer.Dispatcher.InvokeAsync(async () =>
        {
            var rendered = await Renderer.RenderComponentAsync<ChimentHero>(ParameterView.FromDictionary(
                new Dictionary<string, object?> { [nameof(ChimentHero.ShowPortfolioLink)] = showLink }));
            return rendered.ToHtmlString();
        });

        public async ValueTask DisposeAsync()
        {
            await Renderer.DisposeAsync();
            await _provider.DisposeAsync();
        }
    }
}
