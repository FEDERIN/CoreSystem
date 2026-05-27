# Release Notes - 1.1.3

- Refactor: Migrated agnostic registration to Core.
- Fix: Downgraded Npgsql to 8.0.3 to resolve TypeLoadException.
- Feature: Added AddIdempotencyProvider(IConfiguration) extension.
- Clean: Removed deprecated Http.Abstractions dependencies.

## [1.1.6] - 2026-05-26
### Fixed
- Correct Error: Fixed comparison error in AddIdempotencyProvider where ToUpperInvariant() did not match switch cases.