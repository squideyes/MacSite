# MacSite.com — Run & Deploy

## Local development (Aspire)
```bash
dotnet run --project Aspire/MacSite.AppHost      # requires Docker running
```
Aspire runs the migration/seed one-shot against a local SQLite file (`.data/macsite.db`
under the solution root), then the public site and the Studio. The Aspire dashboard links
to both. Default Studio login (from the seeder): `admin@macsite.com` / `ChangeMe!123` —
**change it after first login** (override with `MacSite_ADMIN_EMAIL` /
`MacSite_ADMIN_PASSWORD` env vars at seed time). No Docker is required for local dev.

## Tests
```bash
dotnet test Tests/MacSite.UnitTests
```

## Production — DigitalOcean Droplet (Docker Compose)
1. Provision a Droplet with Docker + Compose.
2. Copy the repo (or pull images from a registry) and create a `.env`:
   ```
   MacSite_ADMIN_PASSWORD=<studio-admin-password>
   MacSite_ADMIN_EMAIL=marie@macsite.com
   PUBLIC_SITE_URL=https://macsite.com
   ```
3. Launch:
   ```bash
   docker compose up -d --build
   ```
   This runs migrations once — creating the SQLite file on the shared `appdata` volume —
   then `web` (:8080) and `admin` (:8081). There is no database server container.

### TLS & routing
Put a reverse proxy (Caddy or nginx) in front:
- `macsite.com` → `web:8080`
- `studio.macsite.com` → `admin:8081`

Caddy example:
```
macsite.com        { reverse_proxy localhost:8080 }
studio.macsite.com { reverse_proxy localhost:8081 }
```

### Database (SQLite)
All three services share one SQLite file on the `appdata` Docker volume
(`Data Source=/data/macsite.db`). WAL mode (set per-connection in `MacSite.Data`) lets the
read-mostly site and the write-occasional Studio share it without blocking. There is no
database server to manage.

- **Backups:** snapshot the file with the WAL applied, e.g.
  `docker compose exec web sh -c "sqlite3 /data/macsite.db '.backup /data/backup.db'"`,
  or just copy the volume while writers are idle.
- **Scaling out:** SQLite suits this single-Droplet, low-write workload. If the app ever
  needs multiple write replicas or a managed DB, the repository layer is provider-agnostic —
  swap `SqliteConnectionFactory` + the schema script for Postgres and point
  `ConnectionStrings__macsitedb` at a DigitalOcean Managed Postgres DSN.

### Media (DigitalOcean Spaces)
In dev, uploads land in the Web app's `wwwroot/media`. For production, implement an
`IMediaStorage` backed by Spaces (S3 API) and register it in `MacSite.Admin/Program.cs`,
then set `Media:BaseUrl` to the Spaces CDN origin. Existing seeded image paths
(`/media/...`) can be uploaded to Spaces under the same keys.

## CI/CD
`.github/workflows/ci.yml` builds, tests, and builds Docker images on push to `main`.
Uncomment the deploy block and add secrets (`DIGITALOCEAN_ACCESS_TOKEN`, `DROPLET_HOST`,
`DROPLET_SSH_KEY`) to push to the DO Container Registry and roll the Droplet.

## Fonts
See [docs/FONTS.md](docs/FONTS.md) — two design fonts are substituted (free) pending an
optional Aeonik purchase; swapping it in is a one-token change.
