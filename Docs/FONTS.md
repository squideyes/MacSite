# Fonts — current state & how to restore full fidelity

The design's Photoshop files use **three** type families. Only one was delivered.

| Role | Design font | Status | Currently using |
|------|-------------|--------|-----------------|
| Headlines / display | **Matita Informal** (Bold, Medium) | ✅ delivered (OTF) | self-hosted Matita — **exact** |
| Logo / nav / buttons / labels | **Aeonik** (Black, Bold, Regular) | ❌ commercial, not delivered | **Inter** (substitute) |
| Body / captions / quotes | **Myriad Pro** (Regular) | ❌ Adobe Original, not web-self-hostable | **Source Sans 3** (substitute, permanent) |

## Why these substitutes
- **Inter** is a close neo-grotesque stand-in for Aeonik for UI text.
- **Source Sans 3** is Myriad's screen-optimized successor by the same designer (Robert Slimbach); the difference in body copy is negligible. This substitution is intended to be **permanent** (Myriad Pro can't be cleanly licensed for self-hosted web use).

## The swap seam
Fonts are referenced **only** through CSS custom properties in
`Core/MacSite.Web/wwwroot/css/design-tokens.css`:

```css
--font-display: 'Matita Informal', ...;   /* exact */
--font-ui:      'Inter', ...;             /* Aeonik substitute */
--font-body:    'Source Sans 3', ...;     /* Myriad substitute */
```

## To restore Aeonik (the only one worth buying — ~$210 for 3 web styles)
1. Purchase the **Aeonik** web license (Regular, Bold, Black) from CoType Foundry.
2. Drop the `woff2` files into `Core/MacSite.Web/wwwroot/fonts/`.
3. Add `@font-face` blocks in `wwwroot/css/fonts.css` (family name `Aeonik`).
4. Change one token: `--font-ui: 'Aeonik', ...;`.
5. Remove Inter from the Google Fonts `<link>` in `Components/App.razor` (keep Source Sans 3).

No other code changes are required — the logo, nav, buttons, eyebrows, filter tabs and labels all read `--font-ui`.

## Also queued (polish)
- Convert the Matita OTFs to `woff2` for smaller payloads (currently served as OTF).
- Self-host Inter + Source Sans 3 (currently via Google Fonts) for offline/perf.
