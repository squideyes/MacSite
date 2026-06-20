using MacSite.Core.Abstractions;
using MacSite.Core.Common;
using MacSite.Core.Enums;
using MacSite.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MacSite.Migrations;

/// <summary>
/// Idempotent content seed derived from "Chiment Site Metadata.pdf". Only inserts when a
/// table is empty, so it is safe to run on every deploy. Image URLs point at the media paths
/// produced by the asset pipeline (served from MacSite.Web/wwwroot/media).
/// </summary>
public static class DataSeeder
{
    // Title, Venue, Year, Director, Composer/Author, MusicDirector, Lights, Scenery, Choreography, Photographer, Category, SliderIndex
    private static readonly ProductionSeed[] Productions =
    [
        new("Arthur and the Tale of the Red Dragon", "People's Light and Theater", 2014, "Pete Pryor", "Michael Ogborn", "Ryan Touhey", "Lily Fossner", "James F. Pyne Jr.", "Samantha Bellomo", "Louis S. Berman", ProductionCategory.Musicals, 1),
        new("The Tragical History of Dr. Faustus", "Oregon Shakespeare Festival", 2005, "James Edmondson", "Todd Barton", null, "Robert Peterson", "Richard I. Hay", "John Sipes", "Charles Erickson", ProductionCategory.Plays, 2),
        new("Rough Crossing", "Oregon Shakespeare Festival", 1997, "James Edmondson", "Andre Previn", "Darcy Danielson", "Dennis Parichy", "Richard I. Hay", "Xedex", "Andree Lanthier", ProductionCategory.Plays, 3),
        new("The Magic Flute", "Santa Fe Opera", 1993, "Reto Nickler", "Wolfgang Amadeus Mozart", "George Manahan", "Craig Miller", "Marie Anne Chiment", null, "Hans Fahrmeyer", ProductionCategory.Opera, 4),
        new("Rigoletto", "Seattle Opera", 2004, "Linda Brovsky", "Giuseppe Verdi", "Edorardo Muller", "Thomas C. Hase", "Robert A. Dahlstrom", "Nicola Bowie", "Rozari Lynch", ProductionCategory.Opera, 5) { CaptionScene = "Maddalena's Seduction\nSparfucile's Aria" },
        new("Rappaccini's Daughter", "Manhattan School of Music", 1997, "Linda Brovsky", "Daniel Catan", "Eduardo Diazmunoz", "John Lasiter", "Maxine Klein", null, "Carol Rosegg", ProductionCategory.Opera, 6),
        new("Private Lives", "Berkeley Repertory Theater", 1977, "George Kovach", "Noel Coward", null, "Matthew Cohen", null, null, null, ProductionCategory.Plays, 7),
        new("Le Temple de la Gloire", "Philharmonia Baroque", 2017, "Catherine Turocy", "Jean-Philippe Rameau", "Nicholas McGegan", "Pierre Dupouey", "Scott Blake", "Catherine Turocy", "Frank Wing", ProductionCategory.Opera, 8),
        new("Twelfth Night", "Repertory Theater of St. Louis", 1985, "David Chambers", "Shakespeare", null, "Max Devolder", "Oliver Smith", null, "Scott Humbert", ProductionCategory.Plays, 9),
        new("Beauty and the Beast", "Opera Theater of St. Louis", 1987, "Colin Graham", "Stephen Oliver", "Hal France", "Peter Kaczorowski", "Marie Anne Chiment", null, "Ken Howard", ProductionCategory.Opera, 10),
        new("A Midsummer Night's Dream", "The Arena Stage", 1981, "David Chambers", "Shakespeare", null, "Arden Fingerhut", "Heidi Landesman", "Kathryn Posin", null, ProductionCategory.Plays, 11),
        new("Patience and Sarah", "Lincoln Center Festival", 1998, "Doug Moser", "Paula Kimper", "Steven Osgood", "Mimi Sherin", "Marie Anne Chiment", null, "Ken Howard", ProductionCategory.Opera, 12) { CaptionScene = "Wagon" },
        new("L'ajo nell Imbarazzo", "Kentucky Opera", 1989, null, "Gaetano Donizetti", null, null, "Marie Anne Chiment", null, null, ProductionCategory.Opera, 13),
        new("Macbeth", "Delaware Theater Company", 2000, "Fontaine Syer", "Shakespeare", null, "Rebecca Frederick", "Marie Anne Chiment", null, "Carol Freeley", ProductionCategory.Plays, 14),
        new("Le Devin du Village", "Alliance Francaise", 1986, null, "Jean-Jacques Rousseau", null, null, null, null, null, ProductionCategory.Opera, 15),
        new("Into the Woods", "Temple Theater", 2007, "Peter Reynolds", "Stephen Sondheim", "Maria Shaplin", null, "Marie Anne Chiment", null, null, ProductionCategory.Musicals, 16),
        new("The House of 7 Gables", "Manhattan School of Music", 2000, "Linda Brovsky", "Scott Eyerly", "David Gilbert", "Dennis Parichy", "Dipu Gupta", null, "Louis S. Berman", ProductionCategory.Opera, 17),
        new("Ariodante", "Spoleto Festival", 1985, "Catherine Turocy", "George Frideric Handel", "James Richman", null, "Jeffery Schneider", "Catherine Turocy", "Beatriz Schiller", ProductionCategory.Opera, 18),
        new("Under the Double Moon", "Opera Theater of St. Louis", 1989, "Rhonda Levine", "Anthony Davis", "Bill Mcglaughlin", "Robert Wierzel", "Marie Anne Chiment", null, "Ken Howard", ProductionCategory.Opera, 19),
        new("Carousel", "National Tour", 1997, "Dallett Norris", "Richard Rogers", "Helen Gregory", "Paul Miller", "James Fouchard", "Karen Azenberg", "Joan Marcus", ProductionCategory.Musicals, 20),
        new("Barber of Seville", "Opera Pacific", 2002, "Linda Brovsky", "Rossini", "Robert Tweten", "Dipu Gupta", null, null, null, ProductionCategory.Opera, 21),
        new("Pirates of Penzance", "Mauckingbird Theater Company", 2016, "Peter Reynolds", "Gilbert and Sullivan", "Michael Pacifico", null, null, null, null, ProductionCategory.Musicals, 22),
        new("Les Fetes de Thalie", "Opera Lafayette", 2024, "Catherine Turocy", "Jean-Joseph Mouret", "Christophe Rousset", "Christopher Brusberg", "Jeffery Martin", "Catherine Turocy", "Jennifer Packard", ProductionCategory.Opera, 23),
        new("Madame Butterfly", "Opera Theatre of St. Louis", 1992, "Colin Graham", "Giacomo Puccini", "Stephen Lord", "Christopher Akerlind", "Marie Anne Chiment", null, "Ken Howard", ProductionCategory.Opera, 24),
        new("Leoncavallo's Boheme", "Opera Theatre of St. Louis", 1991, "Rhonda Levine", "Ruggero Leoncavallo", "Andrew Litton", "Christopher Akerlind", "John Conklin", null, "Ken Howard", ProductionCategory.Opera, 25),
    ];

    // Hero slide image index -> the production (by slider index) it depicts.
    private static readonly (int HeroIndex, int SliderIndex)[] HeroMap =
    [
        (1, 4), (2, 5), (3, 2), (4, 10), (5, 12), (6, 8)
    ];

    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        await SeedSiteSettingsAsync(sp, ct);
        await SeedSectionConfigsAsync(sp, ct);
        var bySlider = await SeedProductionsAsync(sp, ct);
        await SeedHeroSlidesAsync(sp, bySlider, ct);
        await SeedTestimonialsAsync(sp, ct);
        await SeedPressAsync(sp, ct);
        await SeedDownloadsAsync(sp, ct);
        await SeedAdminUserAsync(sp, ct);
    }

    private static async Task<Dictionary<int, Guid>> SeedProductionsAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<IProductionRepository>();
        var map = new Dictionary<int, Guid>();
        var existing = await repo.GetAllAsync(includeUnpublished: true, ct);
        if (existing.Count > 0)
            return Productions.ToDictionary(p => p.SliderIndex, p => Guid.Empty);

        var order = 0;
        foreach (var s in Productions)
        {
            var p = new Production
            {
                Slug = Slug.From($"{s.Title}-{s.Venue}-{s.Year}"),
                Title = s.Title, Venue = s.Venue, Year = s.Year,
                Director = s.Director, ComposerOrAuthor = s.ComposerOrAuthor, MusicDirector = s.MusicDirector,
                Lights = s.Lights, Scenery = s.Scenery, Choreography = s.Choreography, Photographer = s.Photographer,
                Category = s.Category, CaptionScene = s.CaptionScene,
                ShowInSlider = true, ShowInGrid = true, IsPublished = true, SortOrder = order++
            };
            var id = await repo.UpsertAsync(p, ct);
            await repo.ReplaceImagesAsync(id, [new ProductionImage
            {
                ProductionId = id, Url = $"/media/portfolio/slider-{s.SliderIndex:00}.jpg",
                Alt = $"{s.Title} — {s.Venue}", SortOrder = 0, IsPrimary = true
            }], ct);
            map[s.SliderIndex] = id;
        }
        return map;
    }

    private static async Task SeedHeroSlidesAsync(IServiceProvider sp, Dictionary<int, Guid> bySlider, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<IHeroSlideRepository>();
        if ((await repo.GetAllAsync(ct)).Count > 0) return;

        var order = 0;
        foreach (var (heroIndex, sliderIndex) in HeroMap)
        {
            bySlider.TryGetValue(sliderIndex, out var pid);
            await repo.UpsertAsync(new HeroSlide
            {
                ImageUrl = $"/media/hero/hero-{heroIndex}.jpg",
                Alt = Productions.First(p => p.SliderIndex == sliderIndex).Title,
                ProductionId = pid == Guid.Empty ? null : pid,
                SortOrder = order++, IsActive = true
            }, ct);
        }
    }

    private static async Task SeedTestimonialsAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<ITestimonialRepository>();
        if ((await repo.GetAllAsync(ct)).Count > 0) return;

        var seed = new (string Quote, string Source, int Img)[]
        {
            ("Dazzling costumes make the pirates a glittering, bright-colored, fantastical band…", "BROAD STREET REVIEW", 1),
            ("The lusty Amazons of the title are bedecked in sparkling, outrageously pneumatic minidresses designed by Marie Anne Chiment.", "NEW YORK TIMES", 2),
            ("A feast for the eyes — sumptuous, witty, and impeccably detailed.", "OPERA NEWS", 3),
            ("Chiment's designs conjure entire worlds — opulent, precise, and deeply theatrical.", "THE PHILADELPHIA INQUIRER", 4),
            ("Costumes of wit and ravishing beauty that tell the story before a word is sung.", "OPERA TODAY", 5),
        };
        var order = 0;
        foreach (var t in seed)
            await repo.UpsertAsync(new Testimonial
            {
                Quote = t.Quote, Source = t.Source,
                ImageUrl = $"/media/applause/applause-{t.Img:00}.jpg",
                SortOrder = order++, IsActive = true
            }, ct);
    }

    private static async Task SeedPressAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<IPressRepository>();
        if ((await repo.GetAllAsync(ct)).Count > 0) return;

        var seed = new (PressCategory Cat, string Desc, int Img)[]
        {
            (PressCategory.InterviewsAndProfiles, "A full-length feature exploring Marie's design process, inspirations, and her transformative work in opera and theater.", 1),
            (PressCategory.BooksAndAcademic, "Marie's set and costume renderings are featured in this widely used academic textbook, theater design fundamentals.", 2),
            (PressCategory.ReviewsAndAcclaim, "Marie Anne Chiment's designs have captivated audiences and critics alike, earning praise for their emotional depth and visual poetry.", 3),
        };
        var order = 0;
        foreach (var p in seed)
            await repo.UpsertAsync(new PressItem
            {
                Category = p.Cat, Description = p.Desc,
                ImageUrl = $"/media/press/press-{p.Img:00}.jpg",
                SortOrder = order++, IsActive = true
            }, ct);
    }

    private static async Task SeedDownloadsAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<IDownloadRepository>();
        if ((await repo.GetAllAsync(ct)).Count > 0) return;

        await repo.UpsertAsync(new MediaDownload
        {
            Kind = DownloadKind.PublicityResource, Title = "Publicity Resources",
            Description = "Production photos, renderings, and press-ready imagery.",
            FileUrl = "/media/downloads/publicity-resources.zip",
            ThumbnailUrls = "/media/downloads/publicity-1.jpg\n/media/downloads/publicity-2.jpg",
            SortOrder = 0, IsActive = true
        }, ct);
    }

    private static async Task SeedSiteSettingsAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<ISiteSettingsRepository>();
        var current = await repo.GetAsync(ct);
        if (!string.IsNullOrWhiteSpace(current.BioBody)) return;

        current.BioBody =
            "Marie Anne Chiment is an award-winning designer of costumes and sets for theater, opera, and ballet worldwide. " +
            "She received the Kevin Kline Award for ACE (costumes) and I AM MY OWN WIFE (sets and costumes), and the Helen " +
            "Hayes Award for outstanding design. Her work has appeared at the Oregon Shakespeare Festival, Santa Fe Opera, " +
            "Lyric Opera of Chicago, Kennedy Center, Lincoln Center, Arena Stage, and internationally in Spain, Germany, " +
            "England, Cuba, France, and Japan. Featured in Opera News, Entertainment Design, and The Costume Designer's " +
            "Handbook, her recent credits include LES FETES DE THALIE at the Kennedy Center.";
        current.PortraitImageUrl = "/media/bio/portrait.jpg";
        current.CvFileUrl = "/media/downloads/marie-anne-chiment-cv.pdf";
        current.HeroVideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
        current.FacebookUrl = "https://facebook.com/";
        current.InstagramUrl = "https://instagram.com/";
        current.YoutubeUrl = "https://youtube.com/";
        await repo.UpdateAsync(current, ct);
    }

    private static async Task SeedSectionConfigsAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<ISectionConfigRepository>();
        var defaults = new[]
        {
            new SectionConfig { Key = SectionConfig.Hero, DisplayName = "Hero Slider", Autoplay = true, IntervalMs = 6000, ShowDots = true, SlidesVisibleDesktop = 1, SlidesVisibleMobile = 1 },
            new SectionConfig { Key = SectionConfig.Slider, DisplayName = "Portfolio Slider", Autoplay = false, IntervalMs = 6000, ShowDots = false, SlidesVisibleDesktop = 2, SlidesVisibleMobile = 1 },
            new SectionConfig { Key = SectionConfig.Applause, DisplayName = "Applause / Testimonials", Autoplay = true, IntervalMs = 7000, ShowDots = true, SlidesVisibleDesktop = 2, SlidesVisibleMobile = 1 },
            new SectionConfig { Key = SectionConfig.Grid, DisplayName = "Portfolio Grid", GridInitialCount = 8, SlidesVisibleDesktop = 4, SlidesVisibleMobile = 2 },
        };
        foreach (var cfg in defaults)
        {
            if (await repo.GetAsync(cfg.Key, ct) is null)
                await repo.UpsertAsync(cfg, ct);
        }
    }

    private static async Task SeedAdminUserAsync(IServiceProvider sp, CancellationToken ct)
    {
        var repo = sp.GetRequiredService<IAdminUserRepository>();
        if (await repo.CountAsync(ct) > 0) return;

        var email = Environment.GetEnvironmentVariable("MacSite_ADMIN_EMAIL") ?? "admin@macsite.com";
        var password = Environment.GetEnvironmentVariable("MacSite_ADMIN_PASSWORD") ?? "ChangeMe!123";
        await repo.UpsertAsync(new AdminUser
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            DisplayName = "Marie Anne Chiment"
        }, ct);
        Console.WriteLine($"[seed] Created admin user '{email}'. Change the password after first login.");
    }

    private sealed record ProductionSeed(
        string Title, string Venue, int Year, string? Director, string? ComposerOrAuthor,
        string? MusicDirector, string? Lights, string? Scenery, string? Choreography,
        string? Photographer, ProductionCategory Category, int SliderIndex)
    {
        public string? CaptionScene { get; init; }
    }
}
