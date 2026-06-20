namespace MacSite.Admin.Services;

/// <summary>Stores uploaded media and returns its public URL.</summary>
public interface IMediaStorage
{
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct = default);
}

/// <summary>
/// Dev implementation: writes uploads into the public Web app's wwwroot so they are
/// served at <c>/media/uploads/...</c>. In production this is replaced by a
/// DigitalOcean Spaces (S3) implementation. Configure with Media:LocalPath / Media:BaseUrl.
/// </summary>
public sealed class LocalMediaStorage : IMediaStorage
{
    private readonly string _root;
    private readonly string _baseUrl;

    public LocalMediaStorage(IConfiguration config, IWebHostEnvironment env)
    {
        _baseUrl = config["Media:BaseUrl"]?.TrimEnd('/') ?? "/media/uploads";

        var configured = config["Media:LocalPath"];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            _root = configured;
        }
        else
        {
            // Default to the sibling Web project's wwwroot so the public site can serve uploads in dev.
            var webPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "MacSite.Web", "wwwroot", "media", "uploads"));
            _root = Directory.Exists(Path.GetDirectoryName(webPath)!)
                ? webPath
                : Path.Combine(env.ContentRootPath, "wwwroot", "media", "uploads");
        }

        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".bin";
        var name = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
        var fullPath = Path.Combine(_root, name);

        await using (var fs = File.Create(fullPath))
        {
            await content.CopyToAsync(fs, ct);
        }

        return $"{_baseUrl}/{name}";
    }
}
