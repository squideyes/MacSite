using System.Text;
using MacSite.Core.Abstractions;
using MacSite.Data;
using MacSite.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Aspire service defaults (OpenTelemetry, health checks, service discovery).
builder.AddServiceDefaults();

// SQLite (Dapper repositories). The connection string is injected by Aspire locally and by
// docker-compose in production (ConnectionStrings__macsitedb).
var connectionString = builder.Configuration.GetConnectionString("macsitedb")
    ?? throw new InvalidOperationException("Connection string 'macsitedb' is not configured.");
builder.Services.AddMacSiteData(connectionString);
builder.Services.AddScoped<MacSite.Web.Services.SiteContentService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

// Dynamic sitemap covering static routes + published production detail pages.
app.MapGet("/sitemap.xml", async (IProductionRepository repo, HttpContext ctx) =>
{
    var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
    var routes = new List<string> { "/", "/portfolio", "/applause", "/bio", "/downloads", "/contacts" };
    routes.AddRange((await repo.GetAllAsync()).Select(p => $"/portfolio/{p.Slug}"));

    var sb = new StringBuilder();
    sb.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
    sb.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");
    foreach (var r in routes)
        sb.AppendLine($"  <url><loc>{baseUrl}{r}</loc></url>");
    sb.AppendLine("</urlset>");

    return Results.Content(sb.ToString(), "application/xml");
});

app.Run();
