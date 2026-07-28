# PostgreSQL Provider

The PostgreSQL provider stores idempotency records in a relational database using **Dapper**.

It is recommended for applications that already use PostgreSQL or require durable persistence across application restarts.

---

# Requirements

- PostgreSQL 16 or later
- A database dedicated to idempotency or shared with your application
- A valid connection string

---

# Database Setup

CoreSystem.Idempotency automatically creates the required table and indexes when the application starts.

> **Important**
>
> The target database must already exist. The library does **not** create databases.

Example:

```sql
CREATE DATABASE idempotency_db;
```

---

# Connection String

Configure the PostgreSQL connection string.

```json
{
  "ConnectionStrings": {
    "Idempotency": "Host=localhost;Port=5432;Database=idempotency_db;Username=admin;Password=admin"
  }
}
```

---

# Schema

The provider creates the following table automatically.

```sql
CREATE TABLE idempotency_keys
(
    key                 VARCHAR(255) PRIMARY KEY,
    request_fingerprint TEXT NULL,
    hash_algorithm      VARCHAR(255),
    status_code         INTEGER NOT NULL,
    content_type        VARCHAR(255),
    headers             BYTEA NOT NULL,
    body                BYTEA,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    expires_at          TIMESTAMPTZ NOT NULL
);

CREATE INDEX idx_idempotency_keys_expires_at
ON idempotency_keys (expires_at);
```

---

# Expired Records

Expired records are not removed automatically.

Schedule a periodic cleanup job.

Example:

```sql
DELETE
FROM idempotency_keys
WHERE expires_at <= NOW();
```

The cleanup interval depends on your retention policy and expected request volume.

---

# Recommended Maintenance

For production environments, consider scheduling:

- Periodic cleanup of expired records
- VACUUM
- ANALYZE

These operations help maintain query performance as the table grows.

---

# How It Works

When a request reaches the middleware:

1. The idempotency key is extracted.
2. A request fingerprint is generated.
3. PostgreSQL is queried for an existing record.
4. If found, the stored response is returned.
5. Otherwise, the request executes normally.
6. The response is persisted for future requests.

---

# When to Use PostgreSQL

PostgreSQL is recommended when:

- Your application already uses PostgreSQL.
- Strong durability is required.
- You prefer relational storage.
- Redis is not available.

---

# Limitations

Compared to Redis:

- Higher latency
- Disk I/O
- Requires periodic cleanup of expired records