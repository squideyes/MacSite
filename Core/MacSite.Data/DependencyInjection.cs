using MacSite.Core.Abstractions;
using MacSite.Data.Db;
using MacSite.Data.Repositories;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace MacSite.Data;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the Dapper SQLite connection factory and all repositories.
    /// <paramref name="connectionString"/> is a SQLite connection string, e.g.
    /// <c>Data Source=/data/macsite.db;Foreign Keys=True</c>.
    /// </summary>
    public static IServiceCollection AddMacSiteData(this IServiceCollection services, string connectionString)
    {
        // Map snake_case columns to PascalCase properties.
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        // SQLite stores GUIDs and timestamps as TEXT; teach Dapper how to round-trip them.
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.RemoveTypeMap(typeof(Guid?));
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.AddTypeHandler(new DateTimeOffsetTypeHandler());

        services.AddScoped<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));

        services.AddScoped<IProductionRepository, ProductionRepository>();
        services.AddScoped<IHeroSlideRepository, HeroSlideRepository>();
        services.AddScoped<ITestimonialRepository, TestimonialRepository>();
        services.AddScoped<IPressRepository, PressRepository>();
        services.AddScoped<IDownloadRepository, DownloadRepository>();
        services.AddScoped<ISiteSettingsRepository, SiteSettingsRepository>();
        services.AddScoped<ISectionConfigRepository, SectionConfigRepository>();
        services.AddScoped<ISubscriberRepository, SubscriberRepository>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();

        return services;
    }
}
