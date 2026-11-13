---
name: cpm
description: Work safely with Central Package Management (Directory.Packages.props): add, remove, bump, or reconcile package versions, including deleting packages in use and auditing missing/unused dependencies.
---

# Central Package Management (`Directory.Packages.props`)

Follow the conventions in `AGENTS.md` and `CORE-SYSTEM-STANDARDS.md`.

## Key rules

First, print/read `Directory.Packages.props` to know its current state
(do not assume; entries may have been added manually).

- CPM allows only one `PackageVersion` per id; per-TFM pins need separate
  conditional `ItemGroup` blocks keyed on `$(TargetFramework)`.
- Do not downgrade transitive dependencies.
- Preserve conditional package versions by TFM.
- After any change, run
  `dotnet restore CoreSystem.sln --use-lock-file --force-evaluate`
  and commit updated `packages.lock.json` files.
- Inter-library deps use `PackageReference`, not `ProjectReference`
  (except documented Observability.Abstractions exception).
- Never edit lock files manually.

## Deleting a package in use

1. Find consumers: search `*.csproj` for the package id.
2. Remove `PackageReference` from every consuming csproj.
3. Remove `PackageVersion` from `Directory.Packages.props`.
4. Remove source/`using` references.
5. Regenerate lock files.
6. Build and test affected projects.

## Dependency audit (discard unused)

For each `*.csproj`:

1. Collect `PackageReference` items.
2. For each listed `PackageReference`, verify it is actually used in code.
3. If unused, remove the `PackageReference` (and the orphaned `PackageVersion`).
4. Regenerate lock files.
5. Build and test affected projects.

## Validation

```bash
dotnet restore CoreSystem.sln --use-lock-file --force-evaluate
dotnet build CoreSystem.sln -c Release
dotnet test --solution CoreSystem.sln -c Release
dotnet format CoreSystem.sln --verify-no-changes
```