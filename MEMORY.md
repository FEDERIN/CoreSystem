# Memory

Facts and current repo state NOT covered by AGENTS.md. Read this after AGENTS.md.

## Current state (Oct 2026 — after the multi-targeting wave)

- All 10 `src/Core.*` packages multi-target `net8.0;net9.0;net10.0` (samples stay net8.0). Verified: solution build 0/0, format clean, **624/624**, every nupkg has all three `lib/` groups.
- **All 10 packages are published**, verified on the NuGet flat-container (not just tags): Correlation 1.1.0, Memory 1.1.0, Redis 1.1.0, Abstractions 1.1.0, Observability 1.1.0, RateLimiting 1.3.0, Resilience 2.1.0, Serialization 1.3.0, Http 1.2.0, Http.ProblemDetails 1.1.0. csproj versions match.
- Central pins in `Directory.Packages.props` bumped to the published versions (Http 1.2.0, Memory 1.1.0, Observability 1.1.0, Redis 1.1.0, Resilience 2.1.0, Serialization 1.3.0; Abstractions 1.1.0 already current) after all 10 were verified on NuGet. Lock files unchanged — no project still consumes those packages (`Http`/`Memory`/`Redis`/`Serialization` have no in-repo consumers; samples use `ProjectReference`s). Build 0/0 and 624/624 verified after the bump.
- Samples smoke (Docker) never run locally (no daemon); the rolling `sdk:10.0` build tag must resolve to 10.0.2xx+ for `rollForward: latestFeature`.
- External repos: `CoreSystem.Cache*` and `CoreSystem.Idempotency*` live elsewhere (`docs/External/index.md`). Never re-add them here.

## Decisions and rationale

- `Core.Observability` and `Core.Correlation` are independent packages with no cross-reference; composition is opt-in. `CorrelationOptions.SectionName` is `"Core:Correlation"`.
- `Resilience` 2.0.x stayed a minor despite the `ct`→`cancellationToken` rename: consistency bugfix, noted in `PackageReleaseNotes`.
- `tests/Core.Resilience.UnitTests` legitimately uses `StackExchange.Redis` types — not a stray dependency.
- `tests/Core.Observability.UnitTests` tests the *config-binding fix* (PR #63): malformed sub-sections log via `Trace` instead of aborting startup, and a valid `Enabled` flag is still honoured on partial bind failure.
- READMEs/docs are verified against the real public API, not eyeballed (past docs invented types like `IMemoryLockProvider`).
- Every `src/Core.*` has exactly one `README.md` packed via `PackageReadmeFile`; do not reintroduce `README_NUGET.md`.

## Gotchas (empirically verified)

- **NU1109**: bump whole package families together, never just the top package, then `dotnet restore --use-lock-file --force-evaluate`. One reported NU1109 usually hides three more queued behind it (worked example: SourceLink.GitHub 10.0.402 forced `Microsoft.Build.Tasks.Git` too; SourceLink ceiling is **10.0.402**).
- **Per-TFM pins drag their framework-provided transitives**: a per-TFM pin of any `Microsoft.Extensions.*` forces `Configuration(.Abstractions)`/`Options`/`DI.Abstractions` per TFM too, or the net8.0 pin becomes a downgrade. One nuspec group list must be read fully before multi-targeting anything.
- **NU1510 (SDK 10 pruning)** is net10.0-only and an error here: fix = delete the prunable `PackageReference` (the assemblies travel via the `FrameworkReference`). Only `Core.Http` and `Core.Http.ProblemDetails` had it — do not re-add those `PackageReference`s.
- **TFMs live centrally in `Directory.Packages.props`**: `<TargetFrameworks>` **and** the empty `<TargetFramework />` (the latter clears parent-file values, enabling SDK-cross-targeting). Projects declaring nothing inherit all three TFMs; single-TFM ones override in their csproj. Never "tidy" those two lines — silent reversion to net8.0-only, no diagnostics. Verify with `dotnet msbuild <csproj> -getProperty:TargetFramework -getProperty:TargetFrameworks`.
- **Vulnerable transitives are fixed with a central pin, not by upgrading the parent** (worked example: `System.Security.Cryptography.Pkcs` via SqlClient). `CentralPackageTransitivePinningEnabled=true` turns the pin into a `CentralTransitive` override.
- **Central pin vs NuGet floor**: bumping a central pin to a version that is not yet published breaks `RestoreLockedMode`; and a consumer's dependency floor stays at the *declared* version unless the central pin moved **before** the tag. Always check release order and the flat-container.
- **The OTel family does not move as one unit**: core (`OpenTelemetry`, `.Api`, `.Exporter.OpenTelemetryProtocol`, `.Extensions.Hosting`) ships releases the `Instrumentation.*` contributions do not. Bump core and instrumentation independently.
- Advisories are unreliable for .NET CVEs (`vulnerable_version_range` wrong twice): bisect empirically instead of reasoning from the advisory.
- Dotnet solutions/large configs: run `dotnet format --verify-no-changes` before pushing; docs build with `.venv\Scripts\mkdocs.exe`, never system Python.
