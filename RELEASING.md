# Releasing

The order and the rules live in `AGENTS.md`; this file is the checklist.

Publishing is tag-driven. Nothing is pushed to NuGet from this machine: the tag
is what triggers `.github/workflows/publish.yml`, which builds and publishes on
ubuntu.

## Names

| Folder — used in the tag and in paths | PackageId — used on nuget.org |
|---|---|
| `Core.Correlation` | `CoreSystem.Correlation` |
| `Core.Http` | `CoreSystem.Http` |
| `Core.Http.ProblemDetails` | `CoreSystem.Http.ProblemDetails` |
| `Core.Memory` | `CoreSystem.Memory` |
| `Core.Observability` | `CoreSystem.Observability` |
| `Core.Observability.Abstractions` | `CoreSystem.Observability.Abstractions` |
| `Core.RateLimiting` | `CoreSystem.RateLimiting` |
| `Core.Redis` | `CoreSystem.Redis` |
| `Core.Resilience` | `CoreSystem.Resilience` |
| `Core.Serialization` | `CoreSystem.Serialization` |

The tag carries the **folder** name. `publish.yml` resolves
`src/<folder>/<folder>.csproj` and fails before packing if it does not match.
Keep this table in sync when a package is added; `publish-package.bat` does not
hardcode the list, so nothing will catch it for you.

`CoreSystem.Cache*` and `CoreSystem.Idempotency*` live in other repositories.
See `docs/External/index.md`. They are not releasable from here.

## Order

These are the only intra-repo dependency edges:

```
Core.Observability.Abstractions -> Core.Observability
Core.Observability.Abstractions -> Core.RateLimiting
Core.Observability.Abstractions -> Core.Resilience
```

Everything else — Correlation, Http, Http.ProblemDetails, Memory, Redis,
Serialization — depends on nothing else here and can be tagged at any time.

NuGet resolves a dependency to the **declared floor**, not to the latest
published version. Tagging a consumer while the central pin in
`Directory.Packages.props` still points at the old version publishes a package
whose dependency silently stays old. That is what forced the 1.0.2 / 1.2.2 /
2.0.2 "package-only fix" releases.

When several versions are staged, release in two phases:

1. Tag the packages that have no ordering constraints.
2. Once those are live on nuget.org, bump their pins in
   `Directory.Packages.props`, run
   `dotnet restore CoreSystem.sln --use-lock-file --force-evaluate`, commit,
   and only then tag the packages that depend on them.

Never bump a central pin to a version that is not on NuGet yet —
`RestoreLockedMode` fails the whole solution.

**Release notes describe what the .nupkg will declare.** A note promising
`CoreSystem.Observability.Abstractions 1.0.2` is only true if the pin was
bumped before the tag was pushed, so a note may legitimately name a version
that is not the current central pin. Those tags belong to phase 2. Publishing
one out of order ships a note that contradicts the nuspec.

## Pre-flight

```powershell
# the version in the csproj must already be the one being released
Select-String -Path "src/Core.RateLimiting/Core.RateLimiting.csproj" `
  -Pattern '<Version>|<AssemblyVersion>|<FileVersion>|<InformationalVersion>'

# the version must not be published yet
irm https://api.nuget.org/v3-flatcontainer/coresystem.ratelimiting/index.json

# the tag must not exist, locally or on origin
git tag -l "Core.RateLimiting/v1.2.3"
git ls-remote --tags origin "refs/tags/Core.RateLimiting/v1.2.3"

# clean tree, on main
git status --short
git branch --show-current
```

The tree must be clean including untracked files: a release is cut from a
commit, and a tag on `main` carries everything that was committed with it.

A `<Version>` in a csproj is a claim on a version number whether or not it was
ever tagged. Check `git tag --list 'Core.RateLimiting/*'` and the flat-container
index before choosing the next version — a version staged on a branch and never
tagged is still burned.

## Tag and push

```bash
git checkout main
git pull origin main

git tag -a Core.RateLimiting/v1.2.3 -m "CoreSystem.RateLimiting 1.2.3"
git push origin Core.RateLimiting/v1.2.3
```

Push only that tag. If the push fails, the local tag survives — fix the cause
and run `git push origin Core.RateLimiting/v1.2.3` rather than recreating it.

## Watch

```bash
gh run list --workflow publish.yml --limit 1
gh run watch
```

Do not poll the nuget.org index as the success signal — it lags by minutes and
the run already reports whether the push succeeded.

## With the local script

`publish-package.bat` and `generate-lock-files.bat` are gitignored, so they exist
only in this clone. The script runs every pre-flight check above plus restore,
build, `dotnet format --verify-no-changes`, tests, and .nupkg verification, and
refuses to tag if any of them fail:

```bat
publish-package.bat Core.RateLimiting 1.2.3
generate-lock-files.bat
```

It creates and pushes the tag; it does not publish. It does not pause on exit,
so a double-click closes the window before the result can be read.

## Two things CI does not check

`publish.yml` verifies the restore, the build, the tests, that `pack` produced
one file with the expected name, and that the version is not already published.
It **never opens the .nupkg**, and it **does not check the dependency floors**.

Before tagging, confirm that the package contains `README.md` and `LICENSE`,
that the nuspec declares `<readme>` and `<license>`, and that every
`CoreSystem.*` dependency it declares is already published *and* is the latest
published. `publish-package.bat` does this automatically; doing it by hand does
not.

## If it fails

`publish.yml` pushes with `--skip-duplicate`, so re-running a tag whose version
already published reports success instead of failing. Treat that as "already
live", never as "published just now".

That makes a partial release recoverable: fix the cause, re-push the tag.

A wrong *version* is not recoverable — published versions cannot be replaced, so
a mistake ships as a new patch.