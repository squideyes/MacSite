# MacSite.com — finishing & pixel-perfect QA (taking bids)

Hi, and thanks for looking. I'm hiring someone to **finish** a portfolio/marketing website for
theatrical & opera set-and-costume designer **Marie Anne Chiment**, built from a supplied
Photoshop design. A working first-pass already exists and **builds and runs** — the job is to
take it from "rough draft" to **pixel-perfect and production-ready**.

If that's your kind of work, I'd love your bid. A short skim of this page should be enough to
scope one; the full brief is in [`Docs/FIVERR.md`](Docs/FIVERR.md).

## The project in 30 seconds

- A public marketing/portfolio site: hero slider, filterable portfolio (slider ⇄ grid),
  biography, press "spotlight", testimonials, downloadable publicity resources, newsletter
  signup, and contact — matching the design section-for-section.
- Plus a small admin/CMS ("Studio") so the owner can edit content **without touching code**.
- **Stack:** C# 14 / .NET 10 / **Blazor** / .NET Aspire / SQLite + Dapper. Hosted on
  DigitalOcean (Docker Compose provided). You'll need to be comfortable in **.NET + Blazor + CSS**.
- The Photoshop design in [`Design/`](Design/) is the **single source of truth** (desktop
  homepage comp is 1920 × 7491 px).

## What needs doing (the scope)

1. **Pixel-perfect visual fidelity — the #1 priority and the bulk of the work.** The site does
   **not** yet match the comps; the owner reports dozens of visual errors. Much of the
   decorative artwork (brush strokes, torn-paper panels, ghosted figures, watermarks, parchment
   texture) is faked in CSS and **should be exported from the layered PSDs in Photoshop** instead.
2. **Per-section polish on desktop *and* mobile**, matched to the mockups.
3. **Fonts** — see below.
4. **Review & finish the data layer, the admin app, and the forms.** They were scaffolded but
   are largely **unverified** — assume bugs.
5. **Replace placeholder media** (portrait, press images, CV, publicity pack) with real assets.
6. **Deploy to DigitalOcean** (Dockerfiles + compose are in the repo, not yet deployed).

## Fonts (please read — it affects your bid)

The design uses three type families; only one shipped:

- **Matita Informal** (headlines) — delivered, exact. ✅
- **Aeonik** (logo/nav/buttons) — commercial, not delivered. Currently substituted with **Inter**.
  Licensing Aeonik is ~**$210** for 3 web styles and it's a **one-line swap** in the code.
- **Myriad Pro** (body) — not licensable for self-hosted web use, so **permanently** substituted
  with **Source Sans 3** (a near-identical successor; difference is negligible).

Details and the exact swap steps: [`Docs/FONTS.md`](Docs/FONTS.md). **In your bid, tell me**
whether you'd license Aeonik or keep the Inter substitute.

## How to bid

Please include:

- Your rough **cost and timeline** (hours or fixed price).
- Whether you'll do **side-by-side pixel QA** against the design JPGs (the owner's own method).
- Your **approach to the PSD decorative assets** (export vs. approximate).
- Your **font decision** (license Aeonik or not).
- Whether you're quoting the **whole job** or just a slice (e.g. front-end visual fidelity only) —
  either is fine, just be clear about what's in scope.

## Before you bid, please look at

- **Full brief & honest status:** [`Docs/FIVERR.md`](Docs/FIVERR.md)
- **The design (source of truth):** [`Design/`](Design/)
- **Fonts:** [`Docs/FONTS.md`](Docs/FONTS.md) · **Deploy notes:** [`Docs/DEPLOY.md`](Docs/DEPLOY.md)
- **Run it yourself:** install the .NET 10 SDK (no Docker needed for dev), then
  `dotnet run --project Aspire/MacSite.AppHost`. The site comes up pre-seeded with sample content.

## Honest status, up front

It **builds clean and runs**, but **visual fidelity is not close** and most user flows are
**unverified** — assume bugs. I'd rather be straight with you now than surprise you later. The
good news: the structure is in place, so this is a **finishing-and-fidelity** job, not a rewrite.
Full detail is in [`Docs/FIVERR.md`](Docs/FIVERR.md). Looking forward to your bid!
