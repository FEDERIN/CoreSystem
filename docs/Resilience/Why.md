# Why CoreSystem.Resilience?

Modern distributed applications rarely fail because of business logic alone. They fail because of unreliable networks, overloaded services, temporary infrastructure issues, or downstream dependencies that become unavailable.

These failures are often **transient**, meaning that retrying the operation after a short delay is enough to recover successfully. Others require protecting the application by limiting requests or failing fast until the dependency becomes healthy again.

Without a consistent resilience strategy, every application ends up implementing its own retry loops, timeout handling, and circuit breaker logic. This leads to duplicated code, inconsistent behavior, and systems that become difficult to maintain and observe.

CoreSystem.Resilience provides a unified and extensible way to apply resilience policies across your application while keeping business logic clean and focused.

## The Problem

Applications communicating with external systems commonly experience situations such as:

- Temporary network interruptions.
- Slow or unresponsive services.
- Rate limiting from third-party APIs.
- Database connectivity issues.
- Short-lived cloud infrastructure failures.
- Cascading failures between dependent services.

Handling these scenarios manually quickly becomes repetitive and error-prone.

## The Solution

CoreSystem.Resilience centralizes resilience concerns into configurable pipelines that can be reused throughout your application.

Instead of scattering retry logic across the codebase, resilience policies are defined once and applied consistently.

Typical capabilities include:

- Retry for transient failures.
- Timeout policies.
- Circuit breakers.
- Extensible resilience pipelines.
- Built-in diagnostics and metrics.
- Seamless integration with Dependency Injection.

## Benefits

Using CoreSystem.Resilience provides several advantages:

- Consistent resilience behavior across applications.
- Separation of business logic from infrastructure concerns.
- Reduced duplicated code.
- Improved system stability.
- Better observability through metrics and diagnostics.
- Easy extensibility for custom resilience strategies.

## When Should You Use It?

CoreSystem.Resilience is recommended whenever your application communicates with external resources, including:

- HTTP APIs
- Databases
- Message brokers
- Distributed caches
- Cloud services
- Any potentially unreliable dependency

Even highly available services can experience transient failures. Applying resilience consistently helps applications remain responsive and recover automatically whenever possible.

## Next Steps

Continue with the **Architecture** section to understand how resilience pipelines are composed and how policies are executed internally.