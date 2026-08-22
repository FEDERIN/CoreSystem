# CHANGELOG

## [Unreleased]

### Added

- **Declarative Caching Support**: Introduced `[Cacheable]` attribute in `Core.Cache.Attributes` for declarative caching at the method level.
- **Aspect-Oriented Caching**: Implemented infrastructure to support automatic caching via reflection-based interception, enabling seamless integration with services and repositories beyond just API controllers.
- **Default Attribute Behavior**: Configured default expiration (60 seconds) for `[Cacheable]` attribute when `expirationSeconds` is not explicitly provided.

### Changed

- **Architectural Refinement**: Enhanced the caching strategy to allow universal usage across service and repository layers using attribute-based resolution.
- **Documentation**: Updated README.md to include the new Declarative Caching feature and configuration guidelines.

## [2.0.1]

### Fixed

- **Dependency Injection**: Fixed service registration when `Core.Cache` is used without an external cache provider.
- **Fallback Registration**: `FallbackBehavior` is now registered only by providers that support cache fallback.
- **Memory-Only Configuration**: `Core.Cache` can now be used with the built-in Memory provider without requiring `IPrimaryHealthStateWriter`.

### Compatibility

- No Redis provider is required to use `Core.Cache` with Memory caching.