CREATE TABLE IF NOT EXISTS idempotency_keys
(
    key TEXT PRIMARY KEY,

    status_code INTEGER NOT NULL,

    content_type TEXT,

    body TEXT,

    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    expires_at TIMESTAMPTZ NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_idempotency_expires
    ON idempotency_keys (expires_at);