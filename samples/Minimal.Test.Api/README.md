# Minimal.Test.Api (Sample Project)

This is a demonstration project used to validate the implementation of **Core.Observability**. It simulates a real-world microservice to show how logs, traces, and metrics are generated.

---

## 🛠️ Infrastructure Setup

This sample requires **Jaeger** and **Prometheus** to be running. We provide a pre-configured Docker Compose file for this:

1. **Start the containers:**

   ```bash
   docker-compose up -d



### 🔍 Observability Stack Details

This sample implementation validates the following "Golden Signals":

- **Latency:** Measured via OpenTelemetry Histograms exported to Prometheus.
- **Traffic:** Tracked using HTTP request counters.
- **Errors:** Correlated between logs (Serilog) and traces (Jaeger) using the `TraceId`.
- **Saturation:** Monitored via Docker container resources (CPU/RAM).

**Access Points:**
- 🚀 **API:** http://localhost:5185/hello
- 🕵️ **Jaeger:** http://localhost:16686
- 📈 **Prometheus:** http://localhost:9090
- 🎨 **Grafana:** http://localhost:3000 (User/Pass: admin/admin)