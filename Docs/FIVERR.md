# MacSite.com — Handoff Notes (full brief)

A work-in-progress attempt to build a marketing/portfolio website for theatrical/opera
set & costume designer **Marie Anne Chiment** from a supplied Photoshop design.
Stack: C# 14 / .NET 10 / Blazor / .NET Aspire / SQLite + Dapper.

> This is the detailed brief. For the short bid-solicitation intro, see the root
> [`README.md`](../README.md).

---

## ⚠️ Status disclaimer — read first

**Nothing in this repository has been reviewed, tested, or accepted by the owner. None of it
is "done" or signed off.** Treat the entire codebase as a **rough first-pass draft** that needs
independent review and, in places, rework.

- The **visual result does not match the design.** The owner reports *dozens* of visual errors,
  and the top priority of the project — **pixel-perfect fidelity** — is **not met**.
- There are very likely **functional gaps and bugs** as well; most flows have **not** been
  exercised or verified.
- Statements below describe **what was built/attempted** and **what I observed when running it
  in a limited way** — not guarantees of correctness or completeness. Verify everything
  yourself.

---

## What the project is meant to be (aspirations)

1. **Pixel-perfect reproduction of the supplied design** on desktop and mobile. This is the
   owner's explicit #1 requirement.
2. A public marketing/portfolio site: hero slider, filterable portfolio (slider <=> grid),
   biography, press "spotlight", "applause" (testimonials), downloadable publicity resources,
   newsletter signup, contact — matching the comps section-for-section.
3. The configurable sections (**Applause, Slider, Grid, hero-images**) should
   be **editable through a tool** — i.e. an admin/CMS app — without code changes.
4. Built to the owner's house standards: **.NET 10 / C# 14 / Blazor / .NET Aspire /
   SQLite + Dapper**, production-hosted on **DigitalOcean**/dev-hosted on Aspire.

---

## The design is the source of truth

All visuals must match the supplied design in [`Design/`](../Design/):

| Path | What it is |
|------|-----------|
| `Design/2 Desktop Version Source file/jpg/1 homepage.jpg` | Desktop homepage, **1920 × 7491 px** — primary target |
| `Design/2 Desktop Version Source file/jpg/2 Portfolio Expand_Page.jpg` | Desktop portfolio/grid page, 1920 × 7836 |
| `Design/3 Mobile Version Source file/jpg/*.jpg` | Mobile versions (in a phone frame) |
| `Design/2 Desktop Version Source file/psd/*.psd` | **Layered PSDs** — the real source of the decorative artwork (brush strokes, stacked paper, watermarks, ghosted figures, parchment texture). Open in Photoshop to export assets. |
| `Design/1 Fonts - Please install this first/*.otf` | Matita Informal font (5 weights) |
| `Design/Chiment Site Metadata.pdf` | Production metadata used to seed content |
| `Design/Hero Images`, `Portfolio Slider`, `Applause images` | Source production photos |
| `Design/4 All SVG icons` | Footer/banner SVG icons |

Design canvas is **1920 px wide**; content column appears to be ~**1468 px** (≈226 px margins).

---

## Decisions made along the way (as implemented — not endorsed/validated)

- **Fonts:** the design uses three families; only one was delivered.
  - Matita Informal (headlines) — delivered, self-hosted.
  - **Aeonik** (logo/nav/buttons) — commercial, not delivered → substituted with **Inter**.
  - **Myriad Pro** (body) — Adobe, not web-licensable → substituted with **Source Sans 3**.
  - Substitution is a known fidelity gap. See [`FONTS.md`](FONTS.md).
- **Data store:** SQLite + Dapper; content driven from a DB file and an admin app. (Originally
  Postgres; switched to SQLite to drop the database-server dependency for this low-write site —
  see [`DEPLOY.md`](DEPLOY.md). The repository layer is provider-agnostic if it ever needs Postgres.)
- **Rendering:** public site uses Blazor static SSR with small interactive islands; carousels
  were implemented in plain JS (`wwwroot/js/carousel.js`) to avoid a SignalR dependency.

---

## How to run

**Prerequisites:** .NET 10 SDK. (No Docker needed for local dev.)

```bash
dotnet run --project Aspire/MacSite.AppHost
```

Aspire runs a one-shot migration/seed against a local SQLite file (`.data/macsite.db` under
the solution root), then the public site and the admin app. The dashboard URL is printed on
start and links to both.

- Other commands: `dotnet build MacSite.slnx`, `dotnet test Tests/MacSite.UnitTests`.
- An admin login is seeded (`admin@macsite.com` / `ChangeMe!123`) — change/verify as needed.
- **Stale-data gotcha:** the seeder only seeds an empty DB. After changing seed data, delete
  `.data/macsite.db` (and its `-wal`/`-shm` siblings) so the next run re-seeds. This
  repeatedly caused confusion (old content showing).
- A **dev-only design-overlay** exists on the public site (bottom-right control / press `o`)
  that ghosts the design JPG over the page for comparison.

---

## Repository layout

```
/Core
  /MacSite.Web      Public site (Blazor static SSR + JS) — most of the visual work is here
  /MacSite.Admin    Admin/"Studio" app (auth-gated Blazor Server)
  /MacSite.Core     Domain models, enums, Result<T>, repository interfaces
  /MacSite.Data     Dapper repositories over SQLite (Microsoft.Data.Sqlite)
/Aspire
  /MacSite.AppHost          Local orchestration
  /MacSite.ServiceDefaults  OpenTelemetry / health / discovery
/Tools
  /MacSite.Migrations  DbUp schema (Scripts/*.sql) + DataSeeder.cs (seeds from the PDF)
/Tests
  /MacSite.UnitTests   xUnit
/Docs
  FIVERR.md            This brief (full handoff notes)
  DEPLOY.md            Docker / compose / DigitalOcean notes
  FONTS.md             Fonts tech-debt note
_analysis/             Dev scratch (screenshots, crops, comparisons) — not part of the app
```

### Public-site components — `Core/MacSite.Web/Components/` (each has a colocated `.razor.css`)

| Design section | Component |
|---|---|
| Announcement bar | `Shared/AnnouncementBar.razor` |
| Header / nav / mobile menu | `Shared/SiteHeader.razor` |
| Footer | `Shared/SiteFooter.razor` |
| Hero slider + video medallion + brush edge | `Sections/HeroSlider.razor` |
| "THEATRICAL DESIGNS" slider | `Sections/PortfolioSlider.razor` |
| Portfolio grid + filter tabs (expand page) | `Sections/PortfolioGrid.razor` |
| Portfolio card (shared) | `Shared/PortfolioCard.razor` |
| "A LIFE IN DESIGN" bio + BIOGRAPHY watermark | `Sections/BioSection.razor` |
| "FEATURED IN THE SPOTLIGHT" press | `Sections/SpotlightSection.razor` |
| "FROM STAGE TO PAGE" applause carousel | `Sections/ApplauseSlider.razor` |
| "PUBLICITY RESOURCES" + paper panel | `Sections/DownloadsSection.razor` |
| Newsletter / contact forms | `Sections/NewsletterSignup.razor`, `Sections/ContactForm.razor` |
| Page banner (non-hero pages) | `Shared/PageBanner.razor` |

Global styling: `wwwroot/css/design-tokens.css` (colors, type scale, spacing, layout vars),
`site.css` (reset, container, buttons, form controls), `fonts.css`. JS: `wwwroot/js/carousel.js`,
`wwwroot/js/design-overlay.js`. Media: `wwwroot/media/*`, `wwwroot/fonts`, `wwwroot/icons`.

---

## What was built / attempted

This lists what exists in the repo. **None of it is validated or accepted; assume gaps and bugs.**

- A multi-project solution scaffolded to the intended layout above.
- **Domain models** (`MacSite.Core`) and **Dapper repositories** (`MacSite.Data`) for
  productions, images, hero slides, testimonials, press, downloads, site settings, section
  configs, subscribers, contact messages, admin user.
- **DbUp schema** + a **C# seeder** that maps the metadata PDF into the DB.
- An **admin app ("Studio")** that *attempts* cookie auth and CRUD/reorder/upload editors for
  the content types, plus a "Section Settings" screen for carousel/grid knobs. **Not reviewed,
  not signed off — likely incomplete and/or buggy.**
- **Public pages/components** that *attempt* each design section (home, portfolio + grid,
  production detail, applause, bio, downloads, contacts) — **far from matching the design.**
- **Carousels** (hero/slider/applause) implemented in JS.
- **Newsletter and contact forms** wired to repositories.
- `/sitemap.xml`, `/robots.txt`, basic Open Graph tags.
- **xUnit tests** (small, mostly pure-domain).
- **Dockerfiles, `docker-compose.yml`, a GitHub Actions workflow** (not deployed or validated).

---

## Current state, as best I can tell

- **Build:** `dotnet build` reported 0 errors/0 warnings in my runs.
- **Visual fidelity: not close.** This is the dominant outstanding problem (see next section).
- **Functional state:** I only exercised a few things in a limited, mostly-automated way (ran
  the site, seeded a dev SQLite DB, took screenshots, did one scripted admin login). The vast
  majority of user flows — admin editing end-to-end, uploads, form submissions, the grid
  filters, mobile, error cases — were **not** thoroughly tested. **Assume bugs.**
- **My own limitation:** I could capture screenshots but clearly **failed to catch many visual
  errors the owner can see at a glance.** A human comparing the running site to the JPGs in a
  browser is required; do not rely on the state of this code being visually correct.

---

## Known issues / gaps (non-exhaustive)

**Visual (the main work):**
- **Decorative artwork** is mostly missing or faked in CSS: ghosted figure images (e.g. the
  Rigoletto/production-photo ghosts, theatre masks), brush-stroke section transitions, the
  stacked torn-paper panel, the parchment background texture, and the BIOGRAPHY/APPLAUSE
  watermarks. A couple of these were stop-gapped by slicing the design JPG (leaves color
  seams). **The right fix is to export the real layers from the PSDs in Photoshop.**
- **Spacing / vertical rhythm** is approximate and wrong in many places; the page renders
  shorter than the comp.
- **Typography** sizes (headings, body, eyebrows, captions) are approximate; the caption/quote
  font may even be a serif that wasn't identified — confirm from the PSD text layers.
- **Per-section details** unverified: card sizes/aspect, caption overlays, filter controls,
  taped/polaroid frames, hero headline size/line-breaks, medallion size/position, dots,
  buttons, header logo position.
- **Mobile** has only a rough pass; not matched to the mobile mockups.
- **Fonts** substituted (Inter / Source Sans 3) — not the design's Aeonik / Myriad Pro.

**Content/assets:**
- Placeholder media in use: bio portrait (a production photo, not Marie's headshot), press
  images (reused applause photos), publicity zip, CV PDF, hero video URL.

**General:**
- Admin flows, uploads, validation, auth hardening, and the full data round-trip are unverified.
- No production deployment has been done.

---

## Suggested approach for whoever finishes this

1. Run it (above) and open the site in a browser **side-by-side with the design JPGs** — the
   owner's own QA method.
2. **Export the real decorative assets from the PSDs** rather than approximating in CSS.
3. Work top-down, section by section; globals live in `design-tokens.css`, specifics in each
   component's `.razor.css`. The dev overlay (`o`) helps with alignment.
4. Independently review the data layer, the admin app, and the forms — don't assume they're
   correct.
5. Match mobile to the mobile mockups.
6. Decide on fonts (license Aeonik or accept substitutes; [`FONTS.md`](FONTS.md)).
7. Replace placeholder assets with real ones.

---

## Tech debt / open decisions
- Buy & wire **Aeonik** webfont ([`FONTS.md`](FONTS.md)); convert Matita to woff2; self-host Inter/Source Sans 3.
- Implement a **DigitalOcean Spaces** `IMediaStorage` for production uploads (dev writes to the
  Web app's `wwwroot/media`); see `MacSite.Admin/Services/IMediaStorage.cs`.
- Assign each production's **Category** (absent from the source PDF).
- Replace placeholder media; supply hero video, CV, publicity pack.

## Pointers
- Deploy notes: [`DEPLOY.md`](DEPLOY.md) · Fonts: [`FONTS.md`](FONTS.md)
- Dev screenshots/comparisons: `_analysis/`
- Extracted decorative assets so far: `Core/MacSite.Web/wwwroot/media/decor/`
