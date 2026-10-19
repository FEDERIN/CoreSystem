	# Plan: Multi-Targeting `Core.Correlation`

## Objective

Enable `Core.Correlation` to support `.NET 8`, `.NET 9`, and `.NET 10` while maintaining a single codebase, a single project, and one Semantic Versioning version per NuGet package.

This is a pilot migration. Do not migrate the entire repository.

## 1. Scope

Focus exclusively on `Core.Correlation` and its unit tests.

Inspect the following files and components:

- `src/Core.Correlation/Core.Correlation.csproj`
- All source files in `src/Core.Correlation`
- `tests/Core.Correlation.UnitTests/Core.Correlation.UnitTests.csproj`
- Existing unit tests
- `Directory.Build.props`
- `global.json`
- `Directory.Packages.props`
- `.github/workflows/ci.yml`
- `.github/workflows/publish.yml`

## 2. Constraints

- Do not change runtime behavior or the public API unless required for compatibility.
- Do not introduce separate projects or packages for each target framework.
- Do not migrate other libraries or samples.
- Do not add dependencies unless strictly necessary.
- Preserve nullable reference types, analyzers, code style rules, and warnings-as-errors.
- Do not suppress warnings to force a successful build.
- Do not modify lock files blindly; regenerate them only when required.
- Do not publish packages, create tags, or perform releases.
- Keep the changes minimal and limited to this pilot.

## 3. Phase One: Analyze Before Changing Code

Before implementing anything:

1. Inspect the current project structure and build configuration.
2. Identify how `TargetFramework` is inherited from `Directory.Build.props`.
3. Determine how to enable multiple target frameworks for `Core.Correlation` without affecting other projects.
4. Check the compatibility of the source code and dependencies with all three target frameworks.
5. Review the SDK requirements and the impact of `global.json`.
6. Identify the required CI and package-publishing workflow changes.
7. Report the findings, risks, proposed file changes, and validation strategy.

**Do not modify files during this phase. Present the analysis and wait for approval before proceeding.**

## 4. Phase Two: Implement the Migration

After approval:

1. Configure `Core.Correlation` to target `net8.0`, `net9.0`, and `net10.0`.
2. Resolve the inherited `TargetFramework` configuration without unintentionally multi-targeting other projects.
3. Keep the existing implementation shared across all target frameworks.
4. Make conditional changes only when a genuine compatibility requirement exists.
5. Update the unit-test project as needed to validate the supported frameworks.
6. Adjust CI and publishing workflows only as required to build, test, and package the supported targets.
7. Update SDK configuration only when necessary, preserving compatibility with the repository's other projects.
8. Regenerate affected package lock files when required.
9. Keep one package ID and one package version across all target frameworks.

## 5. Validation

Run the following commands from the repository root, adapting them only when the SDK or repository configuration requires it:

```bash
dotnet --list-sdks

dotnet restore CoreSystem.sln

dotnet build src/Core.Correlation/Core.Correlation.csproj \
  -c Release -f net8.0

dotnet build src/Core.Correlation/Core.Correlation.csproj \
  -c Release -f net9.0

dotnet build src/Core.Correlation/Core.Correlation.csproj \
  -c Release -f net10.0

dotnet test tests/Core.Correlation.UnitTests/Core.Correlation.UnitTests.csproj \
  -c Release

dotnet pack src/Core.Correlation/Core.Correlation.csproj \
  -c Release
```

Verify that:

- All three target frameworks build successfully.
- Existing tests pass and any new tests cover identified compatibility gaps.
- The generated NuGet package contains assets for `net8.0`, `net9.0`, and `net10.0`.
- The package retains a single ID and version.
- CI validates the intended target frameworks.
- No unrelated projects have been modified.

If a command fails, identify the root cause before changing configuration or code. Do not treat skipped tests or suppressed warnings as successful validation.

## 6. Acceptance Criteria

The migration is complete when:

- `Core.Correlation` supports all three target frameworks.
- The existing public API and runtime behavior remain unchanged unless a justified compatibility fix is necessary.
- Unit tests pass.
- The NuGet package contains all three target-framework assets.
- CI and publishing configuration are consistent with the migration.
- No unrelated library or sample has been migrated.
- All changes and validation results are documented.

## 7. Final Report

Provide a concise report containing:

- Files changed and the reason for each change.
- Compatibility issues found and how they were addressed.
- Build and test results for each target framework.
- NuGet package validation results.
- Remaining risks or limitations.
- Any SDK or environment requirements for local development and CI.

## Execution Rule

Proceed incrementally. Analyze first, report findings, and wait for approval. After approval, implement only the changes necessary for `Core.Correlation`. Do not expand the scope without explicit authorization.