---
name: CoreSystem Readme Structure
description: Use when creating or editing a package README.md (CoreSystem.* nuget packages). It defines the mandatory structure, badge style, emojis, conventions and code blocks so all package docs stay consistent. Base pattern: src/Core.Resilience/README.md.
---

# CoreSystem README Structure

Every `src/Core.*/README.md` (the file packed into the NuGet `<PackageReadmeFile>`) must follow the structure of `src/Core.Resilience/README.md`. If a section does not apply, keep the header anyway or drop it explicitly — do not invent new top-level sections.

## Rules

1. **Title**: `# ⚡ CoreSystem.<PackageName>` with one leading emoji.
2. **Tagline**: a single `> **one-line summary in English.**`
3. **Intro paragraph**: 1–3 sentences explaining what problem the package solves and why it exists.
4. **Badges block** (4 badges, one per line, for Rider with `style=for-the-badge`):
   ```markdown
   ![NuGet](https://img.shields.io/nuget/v/CoreSystem.<Package>?style=for-the-badge)
   ![Downloads](https://img.shields.io/nuget/dt/CoreSystem.<Package>?style=for-the-badge)
   ![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
   ![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
   ```
5. `---` horizontal separators between major sections.
6. **`## ✨ Features`**: bullet list `- ✅ <feature>`. ✅ on every item.
7. **`## 📦 Installation`**: `dotnet add package ...` fenced bash block.
8. **`## 🚀 Quick Start`**: minimal `csharp` example, plus short prose per code block.
9. **Core concepts section(s)**: one or more `##` sections describing what the package does, with tables, sequence diagrams in ```text blocks and 1-2 code snippets. Example tables from Resilience: strategies, metrics.
10. **`## 🏗 Architecture`**: ASCII architecture diagram in ```text, short paragraph.
11. **`## 📖 Documentation`**: bullets of what the docs cover. No links needed; the GitHub repo is the anchor.
12. **`## 🤝 Contributing`**: short.
13. **`## 📄 License`**: `Released under the MIT License.` exactly.

## General conventions

- All content in **English**, consistent with the rest of the NuGet-facing material.
- Do **not** use HTML `<p>`/`<br>` in package READMEs (unlike the root repo README).
- Code blocks: fenced with language tag (```` ```csharp ````, ```` ```bash ````, ```` ```text ````).
- Emoji conventions: `⚡` package, `✨` features, `📦` install, `🚀` quick start, `🏗` architecture, `📖` docs, `🤝` contributing, `📄` license, `✅` feature bullets, `.NET-8.0` badge.
- Headings: one `#` title, `##` sections, `###` only inside a section.
- Keep README ≤ ~200 lines.

## Copy template

Copy `src/Core.Resilience/README.md` and replace: title, tagline, intro, package name in badges/install command, features list, quick start API names, strategies/metrics table, architecture diagram, docs bullets.
