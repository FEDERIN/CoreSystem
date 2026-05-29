## 📋 Description

Comprehensive unit test suite for Core.Idempotency library.

## 🎯 Type of Change

- [x] Tests added
- [ ] Bug fix
- [ ] Feature
- [ ] Breaking change

## ✨ What's New

### Test Coverage

- **26 Unit Tests** across 6 test classes
- **Storage Tests**: Redis and PostgreSQL implementations
- **Middleware Tests**: Core idempotency logic and edge cases
- **Configuration Tests**: Options and DI extensions
- **Model Tests**: Response serialization and deserialization

### Test Classes

#### Storage
- ✅ `RedisIdempotencyStorageTests` (4 tests)
  - `GetAsync` - retrieve cached responses
  - `SaveAsync` - persist with expiration
  - Serialization validation

- ✅ `PostgresIdempotencyStorageTests` (4 tests)
  - Connection string validation
  - Schema initialization

#### Middleware
- ✅ `IdempotencyMiddlewareTests` (7 tests)
  - Disabled idempotency handling
  - HTTP method filtering
  - Idempotency key validation
  - Cache hit/miss scenarios
  - Error response handling
  - Response persistence

#### Configuration & Extensions
- ✅ `IdempotencyExtensionsTests` (5 tests)
  - Redis registration
  - PostgreSQL registration
  - Custom options configuration

- ✅ `IdempotencyOptionsTests` (3 tests)
  - Default values
  - Custom assignments

#### Models
- ✅ `IdempotencyResponseTests` (3 tests)
  - Property assignment
  - JSON serialization

## 🔧 Testing Stack

- **Framework**: xUnit 2.6.6
- **Mocking**: Moq 4.20.70
- **Assertions**: FluentAssertions 6.12.0
- **Target**: .NET 8.0

## 🚀 How to Run

```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test --filter FullyQualifiedName~Core.Idempotency.Tests.Storage.RedisIdempotencyStorageTests

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## 📊 Coverage Areas

| Component | Tests | Coverage |
|-----------|-------|----------|
| Redis Storage | 4 | ✅ High |
| PostgreSQL Storage | 4 | ✅ High |
| Middleware | 7 | ✅ High |
| Extensions | 5 | ✅ High |
| Options | 3 | ✅ High |
| Models | 3 | ✅ High |
| **Total** | **26** | **✅ Comprehensive** |

## 📝 Notes

- PostgreSQL tests are unit tests (no database required)
- Redis tests use mocks (no Redis instance required)
- All tests are isolated and can run independently
- Fixtures provided for middleware testing

## ✅ Checklist

- [x] Tests added/updated
- [x] Test project configured in solution
- [x] All tests passing
- [x] Documentation included (README.md)
- [x] No breaking changes

## 📚 References

- [Core.Idempotency](src/Core.Idempotency/)
- [Test Documentation](tests/Core.Idempotency.Tests/README.md)
