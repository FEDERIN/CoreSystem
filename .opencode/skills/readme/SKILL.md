---
name: Readme Structure
description: Use when creating or editing a package README.md that ships to a package registry (NuGet/npm/PyPI). Defines the mandatory section layout, badge style, emoji headings and code-block conventions so all package docs in one ecosystem stay consistent.
---

# Package Readme Structure

Every package README that gets published to a registry must follow one structure,
identical across the ecosystem. Pick the most complete existing README in the
repository as the canonical pattern and mirror its section order.

## Layout

1. **Title**: `# ÔÜí <PackageId>` ÔÇö one leading emoji.
2. **Tagline**: a single `> **one-line summary in English.**`
3. **Intro paragraph**: 1ÔÇô3 sentences on what problem it solves and why it exists.
4. **Badges block** ÔÇö one badge per line, `style=for-the-badge` so they render
   compactly in Rider:

   ```markdown
   ![NuGet](https://img.shields.io/nuget/v/<PackageId>?style=for-the-badge)
   ![Downloads](https://img.shields.io/nuget/dt/<PackageId>?style=for-the-badge)
   ![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
   ![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
   ```

5. `---` horizontal rule between major sections.
6. **`## Ô£¿ Features`** ÔÇö bullet list, `- Ô£à <feature>` on every item.
7. **`## ­ƒôª Installation`** ÔÇö install command in a fenced bash block.
8. **`## ­ƒÜÇ Quick Start`** ÔÇö minimal runnable example, one short prose line before
   each block so the reader knows what it does.
9. **Concept sections** ÔÇö one or more `##` covering the package's substance: tables,
   ASCII diagrams in ```text blocks, per-API notes.
10. **`## ­ƒÅù Architecture`** ÔÇö ASCII diagram, then a short paragraph.
11. **`## ­ƒôû Documentation`** ÔÇö bullets of what the full docs cover.
12. **`## ­ƒñØ Contributing`** ÔÇö short.
13. **`## ­ƒôä License`** ÔÇö `Released under the MIT License.` exactly.

If a section does not apply, keep the heading with a one-line note or drop it
deliberately. Do not invent new top-level sections.

## Conventions

- Content in **English** for registry-facing material.
- No HTML `<p>`/`<br>` in package READMEs (they may appear in a root repo README).
- Fenced code blocks always carry a language tag: ```` ```csharp ````,
  ```` ```bash ````, ```` ```text ````.
- Emoji vocabulary: `ÔÜí` package ┬À `Ô£¿` features ┬À `­ƒôª` install ┬À `­ƒÜÇ` quick start ┬À
  `­ƒÅù` architecture ┬À `­ƒôû` docs ┬À `­ƒñØ` contributing ┬À `­ƒôä` license ┬À `­ƒº®` components ┬À
  `ÔÜÖ´©Å` configuration ┬À `Ô£à` feature bullets.
- Headings: exactly one `#`, then `##`, and `###` only inside a section.
- Keep under ~200 lines; push depth into the docs site.

## One README per package

Keep exactly one `README.md` per project and pack that file. Do not maintain a
second `README_NUGET.md` / `README_PACKAGE.md` alongside it ÔÇö the packed file is the
one consumers see, and two files drift apart. See the `nuget-release` skill for the
csproj wiring.

## Accuracy is part of structure

Before shipping, verify every type name, signature and package name in the README
against the source. A well-structured README that does not compile is still broken.
See the `docs-accuracy` skill.

## Adaptation template

Copy the canonical README and replace: title, tagline, intro, package id in badges
and install command, features list, quick-start API names, concept tables,
architecture diagram, docs bullets. Keep the section order identical.
