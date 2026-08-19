# Why CoreSystem.Resilience?

Modern distributed applications rarely fail because of business logic alone. They can fail because of unreliable networks, overloaded services, temporary infrastructure issues, or downstream dependencies that become unavailable.

Some failures are **transient**, meaning that retrying the operation can allow it to recover successfully. Others require limiting execution time or preventing repeated calls to an unhealthy dependency.

Without a consistent resilience strategy, applications can end up implementing retry, timeout, and circuit breaker logic independently. This leads to duplicated code and inconsistent behavior.

CoreSystem.Resilience provides a unified way to configure resilience strategies through reusable pipelines while keeping business logic independent from the underlying resilience implementation.

## The Problem

Applications communicating with external systems commonly experience situations such as:

* Temporary network interruptions.
* Slow or unresponsive services.
* Database connectivity issues.
* Short-lived infrastructure failures.
* Cascading failures between dependent services.

Handling these scenarios manually can quickly become repetitive and difficult to maintain.

## The Solution

CoreSystem.Resilience centralizes resilience configuration into pipelines that can be reused throughout the application.

Instead of scattering resilience configuration across application code, strategies can be configured for a `PipelineType` and executed through `IResiliencePipeline`.

The framework currently provides:

* Retry for configured exceptions.
* Timeout policies.
* Circuit breakers.
* Configurable resilience pipelines.
* Dependency Injection integration.
* Built-in metrics and OpenTelemetry metrics registration.

## Benefits

Using CoreSystem.Resilience provides several advantages:

* Consistent resilience configuration across applications.
* Separation of resilience concerns from business logic.
* Reduced duplicated code.
* Reusable configured pipelines.
* Built-in operational metrics.
* A stable application-facing abstraction over the underlying Polly pipeline.

## When Should You Use It?

CoreSystem.Resilience is useful whenever your application communicates with resources that may experience transient failures, delays, or temporary unavailability.

Examples include:

* HTTP APIs
* Databases
* Message brokers
* Distributed caches
* Other external dependencies

Different workloads can use different `PipelineType` configurations while sharing the same programming model.

## Next Steps

Continue with the **Architecture** section to understand how resilience pipelines are constructed, registered, resolved, and executed internally.
