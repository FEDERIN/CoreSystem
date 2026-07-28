# Why CoreSystem.Idempotency?

HTTP retries are a normal part of modern distributed systems.

Clients retry requests because of network failures, timeouts, gateway errors, or uncertain response states. While retries improve reliability, they also introduce a significant challenge: ensuring that the same business operation is not executed multiple times.

Operations such as payment processing, order creation, inventory updates, and financial transactions must be executed exactly once, regardless of how many times a client repeats the request.

Implementing idempotency independently in every application often results in duplicated middleware, inconsistent persistence strategies, limited observability, and complex storage implementations.

CoreSystem.Idempotency provides a unified idempotency platform that coordinates these concerns while allowing applications to remain focused on business logic.

---

# The Problem

Modern distributed applications frequently require capabilities such as:

- Preventing duplicate execution of critical operations.
- Persisting idempotency state across multiple application instances.
- Returning the original response for repeated requests.
- Detecting when the same idempotency key is reused with different request data.
- Supporting multiple distributed storage providers.
- Measuring request execution, duplicate detection, and storage performance.
- Integrating with OpenTelemetry.
- Extensibility without modifying application code.

Although these capabilities are common in production systems, they are rarely implemented consistently. As a result, teams often build custom idempotency middleware that duplicates infrastructure code across projects.

---

# The Solution

CoreSystem.Idempotency acts as the orchestration layer of the CoreSystem idempotency ecosystem.

Instead of requiring every application to implement its own idempotency logic, incoming requests pass through a configurable middleware pipeline that validates idempotency keys, computes request fingerprints, coordinates persistence, and transparently replays previously stored responses.

This architecture coordinates specialized CoreSystem packages while exposing a single, unified API for application developers.

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Idempotency** | Idempotency middleware, request fingerprinting, response replay, and storage orchestration |
| **CoreSystem.Redis** | Redis connectivity and distributed storage infrastructure |
| **CoreSystem.Serialization** | JSON, MessagePack, and Protocol Buffers serialization |
| **CoreSystem.Http** | HTTP abstractions used by the middleware |
| **CoreSystem.Observability** *(Optional)* | Ready-to-use OpenTelemetry instrumentation, metrics, tracing, and diagnostics for CoreSystem packages |
| **CoreSystem.Observability.Abstractions** | Shared observability contracts for implementing custom instrumentation and integrations |

Applications remain independent from storage-specific implementations while benefiting from a production-ready idempotency platform.

---

# Benefits

Using CoreSystem.Idempotency provides several advantages:

- Exactly-once execution for critical operations.
- Distributed persistence across multiple application instances.
- Automatic response replay for duplicate requests.
- Request fingerprint validation.
- Provider-independent storage architecture.
- Built-in observability and diagnostics.
- Consistent serialization across providers.
- Separation of business logic from infrastructure concerns.
- Extensible middleware pipeline.
- Production-ready defaults.

---

# When Should You Use CoreSystem.Idempotency?

CoreSystem.Idempotency is recommended for applications that require reliable execution of state-changing operations, including:

- Payment processing.
- Order creation.
- Inventory management.
- Financial transactions.
- Distributed APIs.
- Microservices.
- Cloud-native applications.
- Public APIs exposed to unreliable networks.
- Systems requiring OpenTelemetry observability.
- Production environments where duplicate execution must be prevented.

If your application does not expose operations that can be retried or duplicated, implementing idempotency may not be necessary.

CoreSystem.Idempotency is designed for applications that require a complete, extensible, and production-ready idempotency platform.

---

# Next Steps

Continue with the **Architecture** section to understand how the middleware processes requests, how storage providers are coordinated, how request fingerprints are generated, and how CoreSystem.Idempotency integrates with the CoreSystem ecosystem.