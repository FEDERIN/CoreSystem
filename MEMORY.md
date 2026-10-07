# Memory

Facts and current repo state NOT covered by AGENTS.md. Read this after AGENTS.md.

## What changed recently

- `CoreSystem.Cache*` and `CoreSystem.Idempotency*` belong to the ecosystem but live in their own repos (see `docs/External/index.md`). Do not re-create or re-suggest them here (also noted in AGENTS.md).
- Their docs are treated as external projects: everything related lives in `docs/External/index.md`.
- `samples/` consumes published NuGet packages except its internal ProjectReferences (`Samples.Core`, `Samples.Infrastructure`). They do not validate changes in `src/`.
- CI: `.github/workflows/ci.yml` now runs restore/build/test (Release) on push to `main` and PRs, with NuGet cache keyed on `packages.lock.json`. CodeQL remains separate. `samples-smoke.yml` and `publish.yml` unchanged.
- 2026-10-07: branch `release/nuget-updates-oct2026` created and pushed; commit `110bc50` (English). First publish from it: tag `Core.Http/v1.1.0` → NuGet.org + GitHub Packages via `publish.yml` run 37558591452 (success). Remaining planned: patch bumps for ProblemDetails 1.0.2, Memory 1.0.1, Observability 1.0.1, Abstractions 1.0.1, RateLimiting 1.2.1, Redis 1.0.1, Resilience 2.0.1, Serialization 1.2.1, then props pins refresh.
- The Dockerfile path in `samples/CoreSystem.Samples.Api/docker-compose.yml` used to point to `samples/Minimal.Test.Api/Dockerfile` (broken); it now points to `samples/CoreSystem.Samples.Api/Dockerfile`.

## Real internal dependencies

- `Core.Resilience` → `CoreSystem.Observability.Abstractions` (package)
- `Core.RateLimiting` → `CoreSystem.Observability.Abstractions` (package)
- `Core.Observability` → `CoreSystem.Observability.Abstractions` (package by default; ProjectReference only with `UseLocalObservabilityAbstractions=true`)
- Everything else in `src/` has no internal dependencies.

## Debt / things to watch

- ~~`Core.Observability` declares `<Version>1.1.0</Version>` but `Directory.Packages.props` pins `CoreSystem.Observability` to `1.0.0`~~ RESOLVED 2026-10-06: csproj back to `1.0.0` with explicit `AssemblyVersion/FileVersion/InformationalVersion` (props stays 1.0.0). Only 1.0.0 exists on NuGet; bump all three together when releasing 1.1.0.
- `*.bat` and `site/` are in `.gitignore`. `setup/*.bat`, `serve-docs.bat`, `generate-lock-files.bat` are local-only.
- 2026-10-06: removed stray `tests/Core.Http.ProblemDetails.UniTests/` (leftover bin/obj); `Core.RateLimiting` no longer forces `<Optimize>true</Optimize>` nor `<LangVersion>preview</LangVersion>` (C# 12 default, tests pass).
- 2026-10-06: Central `Directory.Build.props` now also enforces `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` and `<AnalysisLevel>latest-recommended</AnalysisLevel>`. Fixed the fallout: missing XML docs added across Observability/Abstractions/Resilience public APIs, `CA1305` (InvariantCulture for Console/RetryAfter), `CA1725` (param rename in NoOpResiliencePipeline), `CA1716`/`CA1848` suppressed locally with justification, CS1998 sample fixed, `CA1707`/`CA1848` suppressed for test projects via `tests/Directory.Build.props`. Ran `dotnet format` once repo-wide (whitespace-only, 21 files); CI now runs `dotnet format --verify-no-changes` after build.
- 2026-10-06: NuGet metadata centralized — root `Directory.Build.props` adds `Deterministic`; new `src/Directory.Build.props` imports root and adds `GenerateDocumentationFile`, `PublishRepositoryUrl`, `EmbedUntrackedSources`, `ContinuousIntegrationBuild`, and `Microsoft.SourceLink.GitHub` for all src projects. Per-csproj duplicates and the two SourceLink PackageReferences (Memory, Serialization) removed. `Directory.Packages.props` pruned of pins absent from all lock graphs (Dapper, Npgsql, Testcontainers.PostgreSql, AspNetCore.HealthChecks.Redis, Microsoft.Net.Http.Headers, Microsoft.AspNetCore.Http(.Abstractions) 2.x, System.Text.RegularExpressions, System.Net.Http, bare `xunit`). Resilience: `AssemblyVersion 2.0.0.0`, removed `OutputType`/`DockerDefaultTargetOS`. New CS1591 warnings now surface in Observability/Abstractions/Resilience (doc comments incomplete there — real gap, not errors).
- There is no `Core.Memory`, `Core.Redis`, or `Core.Serialization` section under `docs/` (only `README_NUGET.md` in each folder): docs coverage is incomplete.
- `CoreSystem.Resilience` already exists and is published (v2.0.0); older READMEs once marked it as planned.
- Docs heading convention is now consistent across all pages: single `#` title, sections `##`, subsections `###`. For docs work use the local `serve-docs.bat` (gitignored, local-only): it creates `.venv`, pip-installs `requirements.txt`, and runs `mkdocs serve` on http://127.0.0.1:8000. For one-off builds use `.venv\Scripts\mkdocs.exe build` — do NOT use system Python 3.11 (no pip, missing `mkdocs-mermaid2-plugin`).
