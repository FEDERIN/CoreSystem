# Core.Idempotency Unit Tests

This project contains comprehensive unit tests for the `Core.Idempotency` library.

## Test Coverage

### Storage Tests
- **RedisIdempotencyStorageTests**: Tests for Redis-based idempotency storage
  - `GetAsync` - retrieve cached responses
  - `SaveAsync` - persist responses with expiration
  - Serialization/deserialization

- **PostgresIdempotencyStorageTests**: Tests for PostgreSQL-based idempotency storage
  - Table/schema creation
  - Query execution

### Middleware Tests
- **IdempotencyMiddlewareTests**: Tests for the core middleware logic
  - Disabled idempotency handling
  - HTTP method filtering
  - Idempotency key validation
  - Cache hit/miss scenarios
  - Response caching and replay
  - Error response handling

### Extension Tests
- **IdempotencyExtensionsTests**: Tests for DI configuration
  - Redis registration
  - PostgreSQL registration
  - Custom options configuration

### Options Tests
- **IdempotencyOptionsTests**: Tests for configuration options
  - Default values validation
  - Custom value assignment

### Model Tests
- **IdempotencyResponseTests**: Tests for response models
  - Property assignment
  - Serialization/deserialization

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test --filter FullyQualifiedName~Core.Idempotency.Tests.Storage.RedisIdempotencyStorageTests

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage (requires coverlet)
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## Test Fixtures

### IdempotencyMiddlewareFixture
Provides pre-configured mocks and HTTP context for middleware testing.

```csharp
var fixture = new IdempotencyMiddlewareFixture();
var middleware = fixture.CreateMiddleware();
await middleware.InvokeAsync(fixture.HttpContext);
```

## Dependencies

- **xUnit**: Test framework
- **Moq**: Mocking library
- **FluentAssertions**: Assertion library
- **Microsoft.AspNetCore.Http**: HTTP context mocking
- **Microsoft.Extensions.DependencyInjection**: DI testing

## Notes

- PostgreSQL storage tests are limited to unit tests without a real database. For integration tests with a real PostgreSQL instance, see integration test documentation.
- Redis storage tests use mocks to avoid requiring a Redis instance during unit testing.
- Middleware tests use `DefaultHttpContext` for request/response testing.
