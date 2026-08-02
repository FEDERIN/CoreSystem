# Why CoreSystem.Http?

Many ASP.NET Core applications need to inspect, capture or replay HTTP responses.

Features such as response caching, idempotency, auditing, logging, API gateways and custom middleware often require the same low-level HTTP infrastructure. Although ASP.NET Core provides the necessary primitives, implementing response capture correctly involves buffering the response body, preserving headers, restoring the original response stream, and handling HTTP-specific behaviors.

Without a reusable abstraction, every library ends up implementing its own response capture logic. This leads to duplicated code, inconsistent behavior, and components that are difficult to maintain and evolve.

CoreSystem.Http extracts these concerns into reusable infrastructure that can be shared across libraries and applications while keeping higher-level features focused on their own responsibilities.

## The Problem

Applications that need to inspect or reproduce HTTP responses typically have to solve the same challenges:

- Capturing the response body without breaking the ASP.NET Core pipeline.
- Preserving response headers and status codes.
- Restoring the original response stream.
- Supporting HTTP semantics such as `HEAD` requests.
- Avoiding duplicated response handling logic across middleware and libraries.

Implementing these behaviors repeatedly increases complexity and creates subtle bugs that are difficult to diagnose.

## The Solution

CoreSystem.Http provides reusable building blocks for capturing and replaying HTTP responses.

Instead of embedding HTTP plumbing into every feature, applications can rely on a small, focused API that separates response capture from response replay.

Core capabilities include:

- HTTP response capture.
- HTTP response replay.
- Response body buffering.
- Header preservation.
- Support for ASP.NET Core request pipelines.
- Lightweight dependency injection integration.
- Small and stable public API.

## Benefits

Using CoreSystem.Http provides several advantages:

- Eliminates duplicated HTTP response handling code.
- Separates HTTP infrastructure from business features.
- Simplifies development of middleware and reusable libraries.
- Promotes consistent behavior across applications.
- Provides a reusable foundation for higher-level components.
- Easy integration with ASP.NET Core Dependency Injection.

## When Should You Use It?

CoreSystem.Http is recommended whenever an application or library needs to capture or replay HTTP responses.

Typical scenarios include:

- Response caching.
- Idempotency.
- Audit logging.
- Response transformation.
- API gateways.
- Reverse proxies.
- Custom middleware.
- ASP.NET Core infrastructure libraries.

If your feature needs to inspect or reproduce an HTTP response, CoreSystem.Http provides the reusable infrastructure required to do it consistently.

## Next Steps

Continue with the **Architecture** section to understand how response capture and replay are separated and how CoreSystem.Http integrates with the ASP.NET Core request pipeline.