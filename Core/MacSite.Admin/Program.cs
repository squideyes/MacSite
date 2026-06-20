using System.Security.Claims;
using MacSite.Admin.Components;
using MacSite.Admin.Services;
using MacSite.Core.Abstractions;
using MacSite.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Aspire service defaults (OpenTelemetry, health checks, service discovery).
builder.AddServiceDefaults();

// SQLite (Dapper repositories). The connection string is injected by Aspire locally and by
// docker-compose in production (ConnectionStrings__macsitedb).
var connectionString = builder.Configuration.GetConnectionString("macsitedb")
    ?? throw new InvalidOperationException("Connection string 'macsitedb' is not configured.");
builder.Services.AddMacSiteData(connectionString);

// Media storage (local filesystem in dev; swap for DigitalOcean Spaces in prod).
builder.Services.AddSingleton<IMediaStorage, LocalMediaStorage>();

// Cookie authentication for the Studio.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "MacSiteStudio";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

// Dev: serve the public Web app's media folder so Studio previews/uploads resolve.
// (In production media is served from DigitalOcean Spaces via absolute URLs.)
var webMedia = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "MacSite.Web", "wwwroot", "media"));
if (Directory.Exists(webMedia))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(webMedia),
        RequestPath = "/media"
    });
}

// ── Auth endpoints (form posts; not Blazor-interactive) ─────────────────────
app.MapPost("/auth/login", async (
    HttpContext ctx, IAdminUserRepository users,
    [FromForm] string email, [FromForm] string password, [FromForm] string? returnUrl) =>
{
    var user = await users.GetByEmailAsync(email ?? string.Empty);
    if (user is null || !BCrypt.Net.BCrypt.Verify(password ?? string.Empty, user.PasswordHash))
    {
        var back = Uri.EscapeDataString(returnUrl ?? "/");
        return Results.Redirect($"/login?error=1&returnUrl={back}");
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.DisplayName) ? user.Email : user.DisplayName),
        new Claim(ClaimTypes.Email, user.Email)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true });
    await users.UpdateLastLoginAsync(user.Id);

    return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
}).AllowAnonymous().DisableAntiforgery();

app.MapPost("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
