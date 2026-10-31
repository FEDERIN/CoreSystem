# AGENTS.md

## 1. Engineering Standards

Before performing any task, read `CORE-SYSTEM-STANDARDS.md`.

These standards are mandatory and apply to all development
activities in this repository.

The repository-specific instructions in this file complement
the common standards.

If a project-specific rule differs from a common standard,
follow the explicit project-specific exception.

## 2. Repository Overview

CoreSystem is a .NET monorepo containing independently
published NuGet libraries.

- Solution: CoreSystem.sln
- Source: src/Core.*
- Package IDs: CoreSystem.*
- Documentation: MkDocs
- Samples: demo API and observability stack.

Follow CORE-SYSTEM-STANDARDS.md for common
engineering rules.

## 3. Repository Architecture

- Each src/Core.* project represents an independent library.
- Packages are published independently.
- Use dotnet sln list and src/ as the source of truth.
- Do not recreate packages that have been extracted
  into independent repositories.
- Respect the existing dependency direction.

## 4. Dependencies

- Package versions are centralized.
- Most inter-library dependencies use PackageReference.
- Core.Observability has a documented exception for
  local Observability.Abstractions references.
- Preserve conditional package versions by TFM.
- `Directory.Build.props` enables `RestoreLockedMode`
  (unless `UseLocalProjectReferences=true` is set in the
  gitignored `Directory.Build.local.props`).
- After adding, removing or changing a package, run
  `dotnet restore CoreSystem.sln --use-lock-file --force-evaluate`
  and commit the updated `packages.lock.json` files.
- Per-TFM packages are pinned with conditional
  `<ItemGroup Condition="'$(TargetFramework)' == 'netX.0'">` blocks
  in `Directory.Packages.props`. CPM permits one `PackageVersion`
  per id, so each TFM needs its own block.
- Inter-library dependencies use `PackageReference`, not
  `ProjectReference`, so local changes reach dependents only after
  the package is published and the central version is bumped.
- Exception: `Core.Observability` can use a local `ProjectReference`
  to Abstractions with `-p:UseLocalObservabilityAbstractions=true`.

## 5. Framework Configuration

- SDK: 10.0.201
- Target frameworks: net8.0;net9.0;net10.0
- All `src/Core.*` libraries and their `tests/Core.*.UnitTests`
  projects are multi-targeted from the central
  `<TargetFrameworks>` in `Directory.Packages.props`.
- The three `samples/` projects stay on net8.0 via
  `samples/Directory.Build.props`.
- A project that must be single-TFM sets an explicit
  `<TargetFramework>` in its nearest `Directory.Build.props`
  (the central one imports first, so a non-empty value wins).
  Do not re-add a global `TargetFramework` default.

## 6. Testing

- Unit tests: tests/Core.*.UnitTests
- Integration tests must follow the infrastructure
  requirements of each library.
- Use scripts/test-changed.ps1 for affected tests.
- Test all projects when solution-wide changes require it.

## 7. Build and Commands

dotnet build CoreSystem.sln -c Release
dotnet test --solution CoreSystem.sln -c Release
dotnet format CoreSystem.sln --verify-no-changes

On SDK 10 (Microsoft.Testing.Platform mode) `dotnet test`
rejects a positional path: always use `--solution` or
`--project`. Single-project example:
`dotnet test --project tests/Core.Redis.UnitTests -c Release`.

## 8. CI/CD

- GitHub Actions.
- Restore, build, format, tests and security audit.
- Independent package publishing.
- `main` is protected: never push directly. Create a branch and
  open a PR with `gh pr create`.
- Preserve existing workflow mappings and filters.

## 8.1 Release

- Publishing is tag-driven (`.github/workflows/publish.yml`).
  The tag format is `<src folder name>/v<version>`,
  e.g. `Core.Http.ProblemDetails/v1.0.1`.
- CI builds that one project with `/p:Version=<tag version>` and
  runs all tests before pushing to NuGet.org and GitHub Packages.
- Keep `<Version>`, `AssemblyVersion`, `FileVersion` and
  `InformationalVersion` in the csproj in sync with the tag
  (`/p:Version` does not override the last two).

## 9. Documentation

- MkDocs documentation.
- One folder per package.
- Navigation configured in mkdocs.yml.
- README files packaged with their respective libraries.
- When a package's public surface changes, update its
  `README.md` (packed into the nupkg) and its `docs/<Package>` pages.
- Documentation and published artifacts are written in English.

## 9.1 Editing rules

- Write UTF-8 explicitly. PowerShell `Set-Content` without
  `-Encoding utf8` writes mojibake (`U+FFFD`) and silently corrupts
  files; prefer the editor tools instead. Many source files
  carry a UTF-8 BOM from Visual Studio — that is the committed
  state, do not normalize it away.
- `*.bat` files are gitignored and exist only locally
  (`serve-docs.bat`, `generate-lock-files.bat`). New `.bat`
  helpers are not committed.

## 10. Memory

Read MEMORY.md at the beginning of a task.
Update it when significant decisions or project state change.
Keep it concise and avoid duplicating permanent rules
(max ~50 lines: summarize or drop what no longer adds value).
If something becomes a permanent rule, propose moving it to
`AGENTS.md` instead of keeping it in memory.
Never store sensitive data (keys, tokens, personal data).
