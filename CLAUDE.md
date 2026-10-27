# FatCat.Fakes Coding Standards

This file defines the coding standards for the FatCat.Fakes codebase.
All code you generate — in any context — must follow the rules for the relevant language below.
The goal is that AI-generated code is indistinguishable from code written by a senior member of this team.

## ⚠️ READ THIS FIRST — Before Any Work

**Before doing anything else in this repository — reading, planning, or writing code — read [`README.md`](README.md) in full.**
It is the single source of truth for what this product is and how it is built. Do not skip it, even for a "small" change.

The README tells you the things you must know before you touch a line of code:
- **Product:** FatCat.Fakes is a NuGet library (`FatCat.Fakes`) that fills objects with random fake data for unit tests. A single static entry point, `Faker`, creates primitives, classes with nested objects, lists, arrays and dictionaries, and random implementations of interfaces and abstract classes (picked from the requested type's assembly). Callers can change an object after creation, ignore properties (nested ones too), create random strings with a prefix or length, and register their own `FakeGenerator` for a type.
- **Tech stack:** **.NET 9** class library (`net9.0`, `LangVersion latest`), strong-name signed with `FakerRelease.snk`. Its only runtime dependency is **fasterflect** (reflection). Tests use **xUnit** + **FluentAssertions**. There is no UI, web server or database.
- **Architecture:** which project owns which responsibility, so a new generator, `Faker` method or test lands in the right project.
- **Standing rules:** invariants that nothing enforces at compile time — for example, the package version lives in `VersionPrefix` in `FatCat.Fakes/FatCat.Fakes.csproj`, and a new built-in generator must be registered in `Generators/FakeFactory.cs` for both `T` and `T?`.

If anything below in the coding standards appears to conflict with the README, the README describes *what* the system is; these rules describe *how* to write code for it — follow both. Where the README and the actual code disagree, **the code is authoritative** — trust the code and flag the stale README.

## Repository Layout

| Path | Project | Purpose |
|---|---|---|
| `FatCat.Fakes/` | `FatCat.Fakes` | The published NuGet library: `Faker` (public entry point), `DynamicRecordCreator`, and `Generators/` (one `FakeGenerator` per type, registered in `FakeFactory`) |
| `FatCat.Fakes.Tests/` | `FatCat.Fakes.Tests` | xUnit + FluentAssertions tests for the library (`Primitives/`, `RandomTests/`, `SpeedUp/`) |
| `OneOff/` | `OneOff` | Console scratch app for trying out and timing fakes by hand; not shipped |
| `tasks/` | — | Task write-ups for planned changes |

All production namespaces start with `FatCat.Fakes.*`; test namespaces start with `Tests.FatCat.Fakes.*`.

Everything must build and every test must pass before work is considered done:
```bash
dotnet build FatCat.Fakes.sln
dotnet test FatCat.Fakes.sln
```

---

## C# Rules

Apply these rules to all C# code. Do not apply them to TypeScript, PowerShell, or any other language.

@.claude/rules/csharp/naming-and-structure.md
@.claude/rules/csharp/types-and-di.md
@.claude/rules/csharp/toolchain.md
@.claude/rules/csharp/async.md
@.claude/rules/csharp/errors-and-logging.md
@.claude/rules/csharp/testing.md
@.claude/rules/csharp/not-allowed.md

## PowerShell Rules

Apply these rules to all PowerShell scripts. Do not apply them to C# or any other language.

@.claude/rules/powershell/powershell.md
