---
description: Prepare and execute a NuGet release (version sync, tag, publish, verify)
---

Load the `nuget-release` skill and run a release.

Target: $ARGUMENTS

Sync the version across csproj and central pins, regenerate lock files, verify the
build and tests, inspect the generated .nupkg contents, then stop and report the
proposed version and tag before anything is tagged or pushed. Remember: published
versions are immutable, so the artifact must be verified first.