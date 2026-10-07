---
name: Documentation Accuracy
description: Use when writing or reviewing README.md files, docs pages, code examples, XML doc comments, or quickstart snippets. Prevents documenting types that do not exist, internal APIs presented as public, wrong package names, and copy-pasted examples that do not compile.
---

# Documentation Accuracy

Documentation that lies is worse than no documentation. A user copies an example,
it does not compile, and they conclude the library is broken.

**This class of error ships to the registry and cannot be corrected** — published
versions are immutable, so fixing it requires a new patch release.

## Always verify type names against source

Never write an example from memory or from an older version. Grep the real
declaration first:

```powershell
Select-String -Path src/**/IWhatever.cs -Pattern "public interface IWhatever"
```

Real defects this catches:
- A README referenced an interface name that never existed (the real contract had a
  different name).
- Docs showed injecting a type that turned out to be `internal`; the public entry
  point was a different interface.
- An install command used the project folder name instead of the `PackageId`, so the
  command simply fails.

## Check accessibility before documenting

A type is only usable by consumers if it is `public`. An `internal` interface in a
public-looking example is a trap. Verify:

```powershell
Select-String -Path src/**/IFoo.cs -Pattern "^\s*(public|internal).*interface IFoo"
```

Same for constructors, extension methods, and namespaces that moved.

## Verify the install command

The package name in `dotnet add package` must equal the `<PackageId>` in the
csproj — not the project folder name, not the class namespace.

## Verify method signatures, not just names

When a doc calls `AcquireAsync(key, ct)` or `Serialize(value)`, confirm the actual
signature and default values:

```powershell
Select-String -Path src/**/IFoo.cs -Pattern "Task<.*> AcquireAsync" -Context 0,3
```

Signature drift is the most common silent doc bug: the method name survives a
redesign while parameters change.

## Documentation structure

For package READMEs, see the `readme` skill for the mandated section layout. For
accuracy:

- One README per project, named `README.md`, packed via `<PackageReadmeFile>`.
- All content in **English** for registry-facing material.
- Use fenced code blocks with a language tag (```` ```csharp ````, ```` ```bash ````).
- No HTML `<p>`/`<br>` in package READMEs.
- Keep README under ~200 lines; move depth into `docs/`.

## XML doc comments are part of the public API

With `GenerateDocumentationFile=true`, a missing `///` on a public member is a build
error (CS1591) **and** a gap in the consumer's IntelliSense. Write real summaries:

```csharp
/// <summary>
/// Acquires an asynchronous lock associated with the specified key.
/// </summary>
/// <param name="key">Logical identifier of the resource to synchronize.</param>
/// <param name="cancellationToken">Token used to cancel the wait.</param>
/// <returns>A disposable handle that releases the lock when disposed.</returns>
```

Partial `<param>` coverage is CS1573. Either document every parameter or none.

## Docs are only as current as the code

When you rename, move, or change the visibility of a public type, grep the docs for
the old name in the same change:

```powershell
Select-String -Path docs/**/*.md,src/**/*.md -Pattern "OldTypeName"
```

A rename that leaves the docs behind is a regression even though CI is green.