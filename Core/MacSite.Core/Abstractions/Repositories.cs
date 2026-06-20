using MacSite.Core.Enums;
using MacSite.Core.Models;

namespace MacSite.Core.Abstractions;

public interface IProductionRepository
{
    Task<IReadOnlyList<Production>> GetAllAsync(bool includeUnpublished = false, CancellationToken ct = default);
    Task<IReadOnlyList<Production>> GetForSliderAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Production>> GetForGridAsync(ProductionCategory? category = null, CancellationToken ct = default);
    Task<Production?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Production?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Guid> UpsertAsync(Production production, CancellationToken ct = default);
    Task ReplaceImagesAsync(Guid productionId, IEnumerable<ProductionImage> images, CancellationToken ct = default);
    Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IHeroSlideRepository
{
    Task<IReadOnlyList<HeroSlide>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<HeroSlide>> GetAllAsync(CancellationToken ct = default);
    Task<Guid> UpsertAsync(HeroSlide slide, CancellationToken ct = default);
    Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface ITestimonialRepository
{
    Task<IReadOnlyList<Testimonial>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Testimonial>> GetAllAsync(CancellationToken ct = default);
    Task<Guid> UpsertAsync(Testimonial testimonial, CancellationToken ct = default);
    Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IPressRepository
{
    Task<IReadOnlyList<PressItem>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PressItem>> GetAllAsync(CancellationToken ct = default);
    Task<Guid> UpsertAsync(PressItem item, CancellationToken ct = default);
    Task ReorderAsync(IReadOnlyList<Guid> orderedIds, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IDownloadRepository
{
    Task<IReadOnlyList<MediaDownload>> GetActiveAsync(DownloadKind? kind = null, CancellationToken ct = default);
    Task<IReadOnlyList<MediaDownload>> GetAllAsync(CancellationToken ct = default);
    Task<Guid> UpsertAsync(MediaDownload download, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface ISiteSettingsRepository
{
    Task<SiteSettings> GetAsync(CancellationToken ct = default);
    Task UpdateAsync(SiteSettings settings, CancellationToken ct = default);
}

public interface ISectionConfigRepository
{
    Task<SectionConfig?> GetAsync(string key, CancellationToken ct = default);
    Task<IReadOnlyList<SectionConfig>> GetAllAsync(CancellationToken ct = default);
    Task UpsertAsync(SectionConfig config, CancellationToken ct = default);
}

public interface ISubscriberRepository
{
    Task<bool> AddAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<Subscriber>> GetAllAsync(CancellationToken ct = default);
}

public interface IContactMessageRepository
{
    Task AddAsync(ContactMessage message, CancellationToken ct = default);
    Task<IReadOnlyList<ContactMessage>> GetAllAsync(CancellationToken ct = default);
    Task MarkReadAsync(Guid id, CancellationToken ct = default);
}

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<Guid> UpsertAsync(AdminUser user, CancellationToken ct = default);
    Task UpdateLastLoginAsync(Guid id, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
}
