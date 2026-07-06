# CHANGELOG

## [Unreleased]

### Added
- **Declarative Caching Support**: Introduced `[Cacheable]` attribute in `Core.Cache.Attributes` for declarative caching at the method level.
- **Aspect-Oriented Caching**: Implemented infrastructure to support automatic caching via reflection-based interception, enabling seamless integration with services and repositories beyond just API controllers.
- **Default Attribute Behavior**: Configured default expiration (60 seconds) for `[Cacheable]` attribute when `expirationSeconds` is not explicitly provided.

### Changed
- **Architectural Refinement**: Enhanced the caching strategy to allow universal usage across service and repository layers using attribute-based resolution.
- **Documentation**: Updated README.md to include the new Declarative Caching feature and configuration guidelines.
