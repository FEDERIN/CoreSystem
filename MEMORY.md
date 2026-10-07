# Memory

Facts and current repo state NOT covered by AGENTS.md. Read this after AGENTS.md.

## Current state (Oct 2026 release cycle complete)

- **Published** (NuGet.org + GitHub Packages): Http 1.1.0, Http.ProblemDetails 1.0.2, Memory 1.0.2, Redis 1.0.2, Observability 1.0.1, Observability.Abstractions 1.0.1, RateLimiting 1.2.1, Resilience 2.0.1, Serialization 1.2.2, **Correlation 1.0.0** (new package, extracted from Observability via PR #50).
- `Core.Observability` and `CoreSystem.Correlation` are independent packages with **no cross-reference** (verified: no `PackageReference`/`ProjectReference`/usings between them). Composition is opt-in by the user.
- `CorrelationOptions.SectionName` is `"Core:Correlation"` (not `Core:Observability:Correlation`).
- Central pins in `Directory.Packages.props` match the published versions; lock files regenerated.

## Decisions and rationale

- Correlation was split out because it only needs `ILogger`/`IOptions`/`IServiceCollection`; keeping it in Observability would drag Serilog+OTel into apps that only want a correlation header. Coupling it back would also force version-locked republishing.
- `Resilience` published as 2.0.1 (not 3.0.0) despite the `ct` → `cancellationToken` rename in `NoOpResiliencePipeline`: the old name already disagreed with `IResiliencePipeline`, so it was a consistency bugfix. Noted in `PackageReleaseNotes`.
- `tests/Core.Resilience.UnitTests` legitimately references `StackExchange.Redis` (uses `RedisConnectionException`/`RedisTimeoutException` in pipeline tests) — not a stray dependency.
- `tests/Core.Observability.UnitTests` was deleted: after the correlation extraction it had zero tests. Its contracts are now covered by `tests/Core.Observability.Abstractions.UnitTests` (3 contract tests).
- `<TargetFramework>net8.0</TargetFramework>` stays declared in every csproj even though `Directory.Build.props` sets it. Closed as a non-issue: keeping it explicit makes each project readable and independently buildable.

## Open items

None. PR #51 merged; `main` is clean and CI, Samples Smoke, CodeQL and Pages are green.

## Gotchas

- Build/format must stay green with `TreatWarningsAsErrors` + `AnalysisLevel latest-recommended`; public APIs without XML docs are build errors.
- Many source files carry a UTF-8 BOM from Visual Studio. That is the committed state — do not "normalize" it away.
- `main` is protected: always branch + PR via `gh pr create`.
- External repos: `CoreSystem.Cache*` and `CoreSystem.Idempotency*` live elsewhere, see `docs/External/index.md`. Do not re-add them here.
- Docs work: use `.venv\Scripts\mkdocs.exe build` (gitignored `serve-docs.bat` for `mkdocs serve`); do not use system Python.