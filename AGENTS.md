# AGENTS.md

.NET 8 (SDK pinned to 8.0.402 in `global.json`) monorepo of independently published NuGet libraries (`src/Core.*` → package IDs `CoreSystem.*`). The root `README.md` lists the current packages; Cache and Idempotency were split out — they remain part of the ecosystem but have their own repositories and docs are external, see `docs/External`. Do not propose re-creating or re-adding them as packages in this repo. Use `dotnet sln list` / `src/` as the source of truth.

## Layout
- `src/Core.<Name>`: one NuGet package per project. `GeneratePackageOnBuild=true`, so every build also produces a `.nupkg`.
- `tests/Core.<Name>.UnitTests`: xUnit v3 + FluentAssertions + Moq.
- `samples/`: demo API plus a docker-compose stack (OTel collector, Prometheus, Grafana).
- `docs/` + `mkdocs.yml`: MkDocs site with one folder per package. It is deployed to GitHub Pages on push to `main`.

## Dependencies / restore (common pitfall)
- Package versions are managed centrally in `Directory.Packages.props`. Do not put `Version=` on `PackageReference`.
- `Directory.Build.props` turns on `RestoreLockedMode`, and `packages.lock.json` files are committed. After adding, removing, or changing a package, run `dotnet restore CoreSystem.sln --use-lock-file --force-evaluate` and commit the updated lock files. Otherwise restore fails.
- Most libraries reference each other as **NuGet packages**, not project references. For example, `Core.Resilience` and `Core.RateLimiting` use `PackageReference CoreSystem.Observability.Abstractions`, so local Abstractions changes do not reach them until that package is published and the central version is bumped.
- Exception: `Core.Observability` can use a `ProjectReference` to Abstractions by passing `-p:UseLocalObservabilityAbstractions=true` (defaults to false, i.e. the published NuGet package).

## Commands
- Build/test everything: `dotnet build CoreSystem.sln` / `dotnet test CoreSystem.sln -c Release` (CI uses Release).
- Single project: `dotnet test tests/Core.Redis.UnitTests -c Release`.
- Affected tests only: `pwsh scripts/test-changed.ps1`. It maps changed `src/X` → `tests/X.UnitTests` and runs the full suite when solution-wide files, `Core.Serialization`, or `Core.Observability.Abstractions` change.
- Docs: `pip install -r requirements.txt`, then `mkdocs build` (or `mkdocs serve`).
- `*.bat` files are gitignored and only exist locally (`serve-docs.bat`, `generate-lock-files.bat`, `setup/`). Don't rely on them or add new ones expecting them to be committed.

## Release
- Publishing is tag-driven (`.github/workflows/publish.yml`). The tag format is `<src folder name>/v<version>`, e.g. `Core.Http.ProblemDetails/v1.0.1`.
- CI restores the solution, builds that one project with `/p:Version=<tag version>`, runs **all** tests, then pushes to NuGet.org and GitHub Packages.
- Keep `<Version>`, `AssemblyVersion`, `FileVersion`, and `InformationalVersion` in the csproj in sync with the tag. When a package's public surface changes, update its `README.md` (packed into the nupkg) and its `docs/<Package>` pages.

## Memory

- At the start, read `MEMORY.md` to learn the current state of the project and past decisions.
- When a task is finished, update it: current state, important decisions (with their rationale), and mistakes to avoid.
- Keep it short (max ~50 lines): summarize or drop what no longer adds value.
- If something becomes a permanent rule, propose moving it to `AGENTS.md` instead of keeping it in memory.
- Never store sensitive data (keys, tokens, personal data).
