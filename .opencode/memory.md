# Memory

Facts and current repo state NOT covered by AGENTS.md. Read this after AGENTS.md.

## What changed recently

- `CoreSystem.Cache*` and `CoreSystem.Idempotency*` belong to the ecosystem but live in their own repos (see `docs/External/index.md`). Do not re-create or re-suggest them here (also noted in AGENTS.md).
- Their docs are treated as external projects: everything related lives in `docs/External/index.md`.
- `samples/` consumes published NuGet packages except its internal ProjectReferences (`Samples.Core`, `Samples.Infrastructure`). They do not validate changes in `src/`.
- The Dockerfile path in `samples/CoreSystem.Samples.Api/docker-compose.yml` used to point to `samples/Minimal.Test.Api/Dockerfile` (broken); it now points to `samples/CoreSystem.Samples.Api/Dockerfile`.

## Real internal dependencies

- `Core.Resilience` → `CoreSystem.Observability.Abstractions` (package)
- `Core.RateLimiting` → `CoreSystem.Observability.Abstractions` (package)
- `Core.Observability` → `Core.Observability.Abstractions` (project, via `UseLocalObservabilityAbstractions=true`)
- Everything else in `src/` has no internal dependencies.

## Debt / things to watch

- `Core.Observability` declares `<Version>1.1.0</Version>` but `Directory.Packages.props` pins `CoreSystem.Observability` to `1.0.0`, and the assembly does not set `AssemblyVersion` (defaults to 1.0.0.0). Other projects reference Abstractions as a package, so local changes do not propagate until published.
- `tests/Core.Http.ProblemDetails.UniTests` is misspelled ("Uni" instead of "Unit"). `scripts/test-changed.ps1` looks for `*.UnitTests.csproj`, so it misses that project when mapping `src/` → tests.
- `*.bat` and `site/` are in `.gitignore`. `setup/*.bat`, `serve-docs.bat`, `generate-lock-files.bat` are local-only.
- There is no `Core.Memory`, `Core.Redis`, or `Core.Serialization` section under `docs/` (only `README_NUGET.md` in each folder): docs coverage is incomplete.
- `CoreSystem.Resilience` already exists and is published (v2.0.0); older READMEs once marked it as planned.
- Docs heading convention is now consistent across all pages: single `#` title, sections `##`, subsections `###`. For docs work use the local `serve-docs.bat` (gitignored, local-only): it creates `.venv`, pip-installs `requirements.txt`, and runs `mkdocs serve` on http://127.0.0.1:8000. For one-off builds use `.venv\Scripts\mkdocs.exe build` — do NOT use system Python 3.11 (no pip, missing `mkdocs-mermaid2-plugin`).
