# Plan: Central Package Management (`Directory.Packages.props`)

## Objective

Work safely with the repository's Central Package Management (CPM) setup:
add, remove, bump, or reconcile package versions in `Directory.Packages.props`
while preserving per-TFM pins and lock files.

## Scope

- `Directory.Packages.props`
- `Directory.Build.props` / `Directory.Build.local.props` (TFM inheritance, `RestoreLockedMode`)
- affected `*.csproj` `PackageReference` items
- `packages.lock.json` files (regenerated, never hand-edited)

## Constraints

- CPM allows only one `PackageVersion` per id; per-TFM pins need separate
  conditional `ItemGroup` blocks keyed on `$(TargetFramework)`.
- Do not downgrade a transitive dependency: if a consumer raises a package for a
  given TFM, move that package into the matching conditional group here.
- Preserve conditional package versions by TFM.
- After any change, run
  `dotnet restore CoreSystem.sln --use-lock-file --force-evaluate`
  and commit the updated `packages.lock.json` files.
- Inter-library dependencies use `PackageReference`, not `ProjectReference`
  (except the documented Observability.Abstractions exception).
- Do not edit lock files manually.

## Deleting a package that is in use

When removing a package that is still referenced (e.g. `Swashbuckle.AspNetCore.SwaggerUI`):

1. Find all consumers:
   `rg "Swashbuckle.AspNetCore.SwaggerUI" --include=*.csproj`
2. Remove the `PackageReference` from every consuming `*.csproj`.
3. Remove the corresponding `PackageVersion` from `Directory.Packages.props`
   (unconditional or the matching conditional TFM block).
4. Remove any source code / `using` references to the package's APIs.
5. Regenerate lock files with `--use-lock-file --force-evaluate`.
6. Build and test affected projects to confirm no dangling references.

## Dependency audit (discard unused)

For each `*.csproj`:

1. Collect `PackageReference` items.
2. For each listed `PackageReference`, verify it is actually used in code.
3. If unused, remove the `PackageReference` (and the `PackageVersion` if no other
   consumer remains).
4. Regenerate lock files with `--use-lock-file --force-evaluate`.
5. Build and test affected projects.

## Procedure

1. First, print/read `Directory.Packages.props` to know its current state
   (do not assume; the user may have added entries manually).
2. Inspect and identify the affected package.
2. Determine whether the package is framework-agnostic (unconditional) or
   TFM-specific (conditional block).
3. Add / modify the `PackageVersion` entry in the correct location.
4. Update `PackageReference` in the consuming project(s) if needed (no version).
5. Regenerate lock files with `--use-lock-file --force-evaluate`.
6. Build and test affected projects.
7. Report changes, versions, and lock files touched.

## Validation

```bash
dotnet restore CoreSystem.sln --use-lock-file --force-evaluate
dotnet build CoreSystem.sln -c Release
dotnet test --solution CoreSystem.sln -c Release
dotnet format CoreSystem.sln --verify-no-changes
```

## Acceptance Criteria

- All projects restore and build cleanly.
- No NU1109 / downgrade warnings.
- Lock files updated and committed.
- No unrelated package changes.