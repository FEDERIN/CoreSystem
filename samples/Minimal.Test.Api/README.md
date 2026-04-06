# Minimal.Test.Api (Sample Project)

This is a demonstration project used to validate the implementation of **Core.Observability**. It simulates a real-world microservice to show how logs, traces, and metrics are generated.

---

## 🛠️ Infrastructure Setup

This sample requires **Jaeger** and **Prometheus** to be running. We provide a pre-configured Docker Compose file for this:

1. **Start the containers:**

   ```bash
   docker-compose up -d
