# Core.Observability.Abstractions

---


**CoreSystem.Observability.Abstractions** is the modular observability core for the **CoreSystem** ecosystem. It provides the base contracts required to implement health checks and telemetry (**OpenTelemetry**) in a decoupled manner within **.NET 8.0** applications.

## 🚀 Purpose

This project eliminates the need to couple monitoring logic with business logic. Through the use of contribution interfaces, any module can define its own metrics, traces, and health checks, which are then automatically discovered and registered by the **Core.Observability** engine.

## 📦 Architecture

The system is built on an **assembly-discovery pattern**, allowing seamless integration across distributed modules.

## 🛠 Main Interfaces

### 1. `IHealthCheckContributor`

Allows modules to register their own health verification checks.

**Responsibilities:**
- Define health checks specific to a module.
- Register health check implementations in a centralized manner.
- Enable automatic discovery and registration by the observability engine.

**Example use cases:**
- Database connectivity validation.
- External API availability checks.
- Message broker health verification.
- Cache service status monitoring.

```csharp
public interface IHealthCheckContributor
{
    void Configure(IHealthChecksBuilder builder);
}