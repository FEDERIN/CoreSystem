CREATE TABLE IF NOT EXISTS idempotency_keys
(
    key                     VARCHAR(255) PRIMARY KEY,
    request_Fingerprint     TEXT NULL,
    hash_algorithm          VARCHAR(255),
    status_code             INTEGER NOT NULL,
    content_type            VARCHAR(255),
    headers                 BYTEA NOT NULL,
    body                    BYTEA,
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    expires_at              TIMESTAMPTZ NOT NULL
);


CREATE INDEX idx_idempotency_keys_expires_at
    ON idempotency_keys (expires_at);