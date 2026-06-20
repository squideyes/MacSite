-- MacSite.com schema (SQLite). Idempotent.
-- GUIDs are stored as TEXT (36-char canonical form) and timestamps as ISO-8601 TEXT;
-- the app's Dapper type handlers round-trip both. Booleans are INTEGER 0/1.

CREATE TABLE IF NOT EXISTS productions (
    id                 TEXT PRIMARY KEY,
    slug               TEXT NOT NULL UNIQUE,
    title              TEXT NOT NULL,
    venue              TEXT NOT NULL DEFAULT '',
    year               INTEGER NOT NULL DEFAULT 0,
    director           TEXT,
    composer_or_author TEXT,
    music_director     TEXT,
    lights             TEXT,
    scenery            TEXT,
    choreography       TEXT,
    photographer       TEXT,
    category           INTEGER NOT NULL DEFAULT 0,
    caption_scene      TEXT,
    show_in_slider     INTEGER NOT NULL DEFAULT 0,
    show_in_grid       INTEGER NOT NULL DEFAULT 1,
    is_published       INTEGER NOT NULL DEFAULT 1,
    sort_order         INTEGER NOT NULL DEFAULT 0,
    is_deleted         INTEGER NOT NULL DEFAULT 0,
    deleted_at         TEXT,
    created_at         TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    updated_at         TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);
CREATE INDEX IF NOT EXISTS ix_productions_category ON productions (category) WHERE is_deleted = 0;

CREATE TABLE IF NOT EXISTS production_images (
    id            TEXT PRIMARY KEY,
    production_id TEXT NOT NULL REFERENCES productions (id) ON DELETE CASCADE,
    url           TEXT NOT NULL,
    alt           TEXT,
    sort_order    INTEGER NOT NULL DEFAULT 0,
    is_primary    INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS ix_production_images_pid ON production_images (production_id);

CREATE TABLE IF NOT EXISTS hero_slides (
    id            TEXT PRIMARY KEY,
    image_url     TEXT NOT NULL,
    alt           TEXT,
    headline      TEXT,
    production_id TEXT REFERENCES productions (id) ON DELETE SET NULL,
    sort_order    INTEGER NOT NULL DEFAULT 0,
    is_active     INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS testimonials (
    id            TEXT PRIMARY KEY,
    quote         TEXT NOT NULL,
    source        TEXT NOT NULL DEFAULT '',
    image_url     TEXT,
    production_id TEXT REFERENCES productions (id) ON DELETE SET NULL,
    sort_order    INTEGER NOT NULL DEFAULT 0,
    is_active     INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS press_items (
    id          TEXT PRIMARY KEY,
    category    INTEGER NOT NULL DEFAULT 0,
    image_url   TEXT NOT NULL,
    title       TEXT,
    description TEXT NOT NULL DEFAULT '',
    url         TEXT,
    sort_order  INTEGER NOT NULL DEFAULT 0,
    is_active   INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS media_downloads (
    id             TEXT PRIMARY KEY,
    kind           INTEGER NOT NULL DEFAULT 0,
    title          TEXT NOT NULL,
    description    TEXT,
    file_url       TEXT NOT NULL DEFAULT '',
    thumbnail_urls TEXT,
    sort_order     INTEGER NOT NULL DEFAULT 0,
    is_active      INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS site_settings (
    id                    TEXT PRIMARY KEY,
    brand_name            TEXT NOT NULL DEFAULT 'MARIE ANNE CHIMENT',
    announcement_text     TEXT NOT NULL DEFAULT '',
    default_hero_headline TEXT NOT NULL DEFAULT '',
    hero_video_url        TEXT,
    hero_video_badge_text TEXT NOT NULL DEFAULT '',
    bio_body              TEXT NOT NULL DEFAULT '',
    portrait_image_url    TEXT,
    cv_file_url           TEXT,
    newsletter_heading    TEXT NOT NULL DEFAULT '',
    phone                 TEXT NOT NULL DEFAULT '',
    email                 TEXT NOT NULL DEFAULT '',
    facebook_url          TEXT,
    instagram_url         TEXT,
    youtube_url           TEXT,
    copyright_name        TEXT NOT NULL DEFAULT '',
    updated_at            TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE IF NOT EXISTS section_configs (
    key                    TEXT PRIMARY KEY,
    display_name           TEXT NOT NULL DEFAULT '',
    autoplay               INTEGER NOT NULL DEFAULT 1,
    interval_ms            INTEGER NOT NULL DEFAULT 6000,
    show_dots              INTEGER NOT NULL DEFAULT 1,
    show_arrows            INTEGER NOT NULL DEFAULT 0,
    show_captions          INTEGER NOT NULL DEFAULT 1,
    slides_visible_desktop INTEGER NOT NULL DEFAULT 2,
    slides_visible_mobile  INTEGER NOT NULL DEFAULT 1,
    enabled_categories_csv TEXT,
    grid_initial_count     INTEGER NOT NULL DEFAULT 8,
    extra_json             TEXT
);

CREATE TABLE IF NOT EXISTS subscribers (
    id           TEXT PRIMARY KEY,
    email        TEXT NOT NULL UNIQUE,
    is_confirmed INTEGER NOT NULL DEFAULT 0,
    created_at   TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE IF NOT EXISTS contact_messages (
    id         TEXT PRIMARY KEY,
    name       TEXT NOT NULL,
    email      TEXT NOT NULL,
    subject    TEXT,
    message    TEXT NOT NULL,
    is_read    INTEGER NOT NULL DEFAULT 0,
    created_at TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE IF NOT EXISTS admin_users (
    id            TEXT PRIMARY KEY,
    email         TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    display_name  TEXT NOT NULL DEFAULT '',
    created_at    TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    last_login_at TEXT
);
