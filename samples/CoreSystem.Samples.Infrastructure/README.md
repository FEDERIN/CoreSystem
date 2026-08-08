# Core.Idempotency (Sample Integration)

This sample demonstrates the integration of **Core.Idempotency** into a real ASP.NET Core application.

It validates the provider-independent idempotency pipeline using two independent storage providers:

* **Core.Idempotency.Redis**
* **Core.Idempotency.PostgreSql**

The same API endpoints and idempotency configuration are used with both providers. The storage implementation is selected through application configuration.

---

## 🛠️ Infrastructure Setup

This sample can run with either **Redis** or **PostgreSQL** as the idempotency storage provider.

Both providers can be referenced by the sample application, but only the provider selected through configuration is registered at startup.

### Redis

Start the Redis container:

```bash
docker-compose up -d
```

The sample uses the configured Redis connection from:

```json
{
  "RedisConnections": {
    "MainRedis": {
      "Host": "localhost:6379",
      "Password": "foobared"
    }
  }
}
```

### PostgreSQL

PostgreSQL can be configured through:

```json
{
  "PostgreSqlConnections": {
    "MainPostgreSql": {
      "ConnectionString": "Host=localhost;Port=5433;Database=idempotency_db;Username=admin;Password=admin_password_123"
    }
  }
}
```

The required `idempotency_keys` table must exist before running the PostgreSQL sample.

---

## ⚙️ Provider Selection

The storage provider is selected through application configuration:

```json
{
  "Core": {
    "Idempotency": {
      "Enabled": true,
      "Provider": "Redis"
    }
  }
}
```

To use PostgreSQL:

```json
{
  "Core": {
    "Idempotency": {
      "Enabled": true,
      "Provider": "PostgreSql"
    }
  }
}
```

The API endpoints do not change when switching providers.

---

## 🔧 Idempotency Configuration

The sample configures the provider-independent Core.Idempotency options:

```json
{
  "Core": {
    "Idempotency": {
      "Enabled": true,
      "Provider": "Redis",
      "AllowedMethods": [
        "POST"
      ],
      "InstanceName": "CoreSystem:App01",
      "Expiration": "01:00:00",
      "Fingerprint": {
        "Enabled": true,
        "IncludeQueryString": true,
        "IncludeContentType": true,
        "IncludedHeaders": [
          "Accept",
          "X-Tenant-Id"
        ]
      }
    }
  }
}
```

The core library is responsible for the idempotency pipeline, request fingerprinting, response replay, and shared diagnostics.

The selected provider is responsible for persistence.

---

## 🚀 API

The sample exposes a real HTTP endpoint:

```text
POST http://localhost:5082/api/order/data
```

Example request:

```http
POST /api/order/data
Idempotency-Key: 15
Content-Type: application/json

{
  "id": 3,
  "description": "Test"
}
```

### First Request

The first request is processed normally by the API.

The response is persisted by the configured storage provider.

```text
HTTP 200
```

### Repeated Request

Sending the same request with the same `Idempotency-Key` causes the previously stored response to be replayed.

```http
POST /api/order/data
Idempotency-Key: 15
Content-Type: application/json

{
  "id": 3,
  "description": "Test"
}
```

The controller is not executed again and the stored response is returned.

---

## 🔐 Fingerprint Validation

The sample also validates request fingerprint protection.

Using the same `Idempotency-Key` with a different request produces a fingerprint mismatch.

Example:

```http
POST /api/order/data
Idempotency-Key: 15
Content-Type: application/json

{
  "id": 4,
  "description": "Different request"
}
```

Expected result:

```text
HTTP 409 Conflict
```

This prevents an existing idempotency key from being reused for a different request.

---

## 💾 Storage Providers

### Redis

When Redis is selected:

```text
Core.Idempotency
        │
        ▼
IIdempotencyStorage
        │
        ▼
Core.Idempotency.Redis
        │
        ▼
Redis
```

Example Redis key:

```text
CoreSystem:App01:Idempotency:15
```

The stored entry contains the request fingerprint and captured HTTP response.

### PostgreSQL

When PostgreSQL is selected:

```text
Core.Idempotency
        │
        ▼
IIdempotencyStorage
        │
        ▼
Core.Idempotency.PostgreSql
        │
        ▼
PostgreSQL
```

The response is persisted in:

```text
public.idempotency_keys
```

The sample validates persistence, replay, expiration, and reuse of an expired idempotency key.

---

## 🔄 Provider Switching

The same API can switch between Redis and PostgreSQL by changing:

```json
"Provider": "Redis"
```

to:

```json
"Provider": "PostgreSql"
```

No changes are required to the API endpoint or idempotency middleware.

This demonstrates the provider-independent architecture of **Core.Idempotency**.

---

## ✅ Validation

The sample validates:

* First request execution.
* Response persistence.
* Response replay.
* Idempotency key reuse.
* Request fingerprint validation.
* Fingerprint mismatch handling.
* Expired idempotency entries.
* Redis storage.
* PostgreSQL storage.
* Provider switching without modifying API endpoints.

---

## 📌 Architecture

```text
                         Core.Idempotency
                                │
                         IIdempotencyStorage
                                │
                  ┌─────────────┴─────────────┐
                  │                           │
       Core.Idempotency.Redis       Core.Idempotency.PostgreSql
                  │                           │
                Redis                    PostgreSQL
```

The sample application consumes the providers through their public registration APIs, keeping storage-specific implementation details outside the core idempotency library.
