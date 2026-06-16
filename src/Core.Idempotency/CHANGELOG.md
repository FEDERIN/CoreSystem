# Changelog

All notable changes to this project will be documented in this file.

---

# [1.2.0] - 2026-06-15

## Fixed

### 204 No Content Response Handling

- Fixed an issue where `IdempotencyMiddleware` attempted to process empty response streams for HTTP 204 responses.
- Prevented persistence of empty response bodies.
- Improved middleware stability when handling responses without content.

## Changed

### Middleware Stability

- Refactored response stream handling.
- Improved restoration of the original response stream.
- Added safer handling for empty responses.

---

# [1.1.6] - 2026-06-10

## Fixed

### Provider Registration

- Fixed comparison issue in `AddIdempotencyProvider()` where `ToUpperInvariant()` values did not match switch cases.

---

# [1.1.3] - 2026-06-01

## Added

- Added `AddIdempotencyProvider(IConfiguration)` extension method.

## Changed

- Migrated provider registration logic to Core abstractions.
- Removed deprecated Http.Abstractions dependencies.

## Fixed

- Downgraded Npgsql to 8.0.3 to resolve `TypeLoadException`.

---