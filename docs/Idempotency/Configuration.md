# ⚙️ Configuration

This guide describes every configuration option available in **CoreSystem.Idempotency**.

You'll learn how to configure:

- Framework options
- Storage providers
- Request fingerprinting
- Supported HTTP methods
- Response expiration
- OpenTelemetry
- `appsettings.json` integration

---

# Configuration Overview

Configure the framework using the `AddCoreIdempotency()` extension.

```csharp
builder.Services.AddCoreIdempotency(options =>
{
    // Configure the framework here
});
```

The framework can also be configured using `appsettings.json`.

---

# Configuration Options

| Option | Description | Default |
|----------|-------------|---------|
| Enabled | Enables or disables the middleware | `true` |
| Provider | Storage provider | `Redis` |
| HeaderName | HTTP header containing the idempotency key | `X-Idempotency-Key` |
| Expiration | Lifetime of stored responses | `24 hours` |
| AllowedMethods | HTTP methods protected by the middleware | `POST`, `PUT` |
| Fingerprint | Request fingerprint generation options | Default configuration |
| MeterName | OpenTelemetry meter name | `CoreSystem.Idempotency` |

---

# Enable or Disable the Framework

Enable the middleware.

```csharp
options.Enabled = true;
```

Disable the middleware without removing it from the dependency injection container.

```csharp
options.Enabled = false;
```

---

# Storage Provider

Select the persistence provider used to store idempotency records.

```csharp
options.Provider =
    StorageProviderType.Redis;
```

or

```csharp
options.Provider =
    StorageProviderType.PostgreSql;
```

Provider-specific configuration is described in the **Storage Providers** guide.

---

# Header Name

Configure the HTTP header that contains the idempotency key.

```csharp
options.HeaderName =
    "X-Idempotency-Key";
```

Example request:

```http
POST /orders HTTP/1.1
X-Idempotency-Key: 9f0c52d5-aaf9-4f3d-a64b-3f8f21d4a71b
```

---

# Allowed HTTP Methods

By default, the middleware protects:

- POST
- PUT

Add additional methods:

```csharp
options.AddAllowedMethods(
    "PATCH",
    "DELETE");
```

Remove methods:

```csharp
options.RemoveAllowedMethods(
    "PUT");
```

Requests using methods that are not included bypass the middleware.

---

# Response Expiration

Configure how long an idempotent response remains available for replay.

```csharp
options.Expiration =
    TimeSpan.FromHours(24);
```

Once the expiration time has elapsed, the request is treated as a new operation.

---

# Request Fingerprinting

Request fingerprinting prevents an idempotency key from being reused with a different request.

Example:

```csharp
options.Fingerprint.IncludedHeaders.Add(
    "X-Tenant-Id");

options.Fingerprint.IncludedHeaders.Add(
    "X-Region");
```

See **Fingerprinting** for a complete description of all available options.

---

# OpenTelemetry

Configure the meter name used by OpenTelemetry metrics.

```csharp
options.MeterName =
    "Orders.Api.Idempotency";
```

---

# Using appsettings.json

```json
{
  "Core": {
    "Idempotency": {
      "Enabled": true,
      "Provider": "Redis",
      "HeaderName": "X-Idempotency-Key",
      "Expiration": "1.00:00:00",
      "AllowedMethods": [
        "POST",
        "PUT"
      ],
      "MeterName": "Orders.Api.Idempotency",
      "Fingerprint": {
        "Algorithm": "SHA256",
        "IncludeQueryString": true,
        "IncludeBody": true,
        "IncludedHeaders": [
          "X-Tenant-Id",
          "X-Region"
        ]
      }
    }
  }
}
```

Bind the configuration:

```csharp
builder.Services.AddCoreIdempotency(options =>
{
    builder.Configuration
        .GetSection("Core:Idempotency")
        .Bind(options);
});
```

!!! note

    The `Fingerprint` section customizes how request fingerprints are generated.
    See the **Fingerprinting** guide for a detailed explanation of each option.

---

# Recommended Configurations

## Development

| Setting | Value |
|----------|-------|
| Provider | Redis |
| Expiration | 15 minutes |
| Fingerprint | Default |
| Meter Name | CoreSystem.Idempotency |

---

## Production

| Setting | Value |
|----------|-------|
| Provider | Redis or PostgreSQL |
| Expiration | 24 hours |
| Fingerprint | Default + required headers |
| Meter Name | Application-specific |

---

# Best Practices

- Use UUIDs for idempotency keys.
- Configure expiration according to your business requirements.
- Protect only operations that modify application state.
- Never reuse an idempotency key for different requests.
- Include only stable headers when customizing request fingerprinting.
- Use application-specific meter names when multiple services publish metrics.