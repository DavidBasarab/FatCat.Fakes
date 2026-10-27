---
name: standards-review
description: Review code against the FatCat.Fakes coding standards in .claude/rules. Use when the user says "review all uncommitted changes", "review the Api project", "review this directory", "review the last commit", "review commit <hash>", or similar. Loads the relevant language rule files, finds every violation (especially the C# one-class-per-file rule), and — only when violations exist — writes a human-readable, session-actionable markdown report to .reviews/ (gitignored). This skill NEVER edits source code and NEVER deletes report files.
---

# Standards Review

Review code in a requested scope against the FatCat.Fakes coding standards and produce a report another session can act on. This skill **only reads and reports** — it does not fix code and does not delete report files.

## Step 1 — Resolve the scope

Map the user's phrasing to a concrete set of files to review. Run all `git` commands from the repo root.

| User says | Scope |
|---|---|
| "review all uncommitted changes" / "review my changes" | Staged + unstaged + untracked files: `git status --porcelain` (each entry is in scope). Review the **current working-tree content** of each file. |
| "review the Api project" / "review Common" / "review the Site" etc. | All source files under that project directory. |
| "review this directory" / "review `<path>`" | All source files under the given directory (or the current working directory if none named). |
| "review the last commit" | Files changed in `HEAD`: `git show --stat --name-only HEAD`. Review the **content as of that commit** via `git show HEAD:<path>`. |
| "review commit `<hash>`" | Files changed in `<hash>`: `git show --stat --name-only <hash>`. Review content via `git show <hash>:<path>`. |

Rules:
- Only review source files: `*.cs`, `*.razor`, `*.ts`, `*.tsx`, `*.ps1`. Skip generated code, `bin/`, `obj/`, `build/`, `*.csproj`, `*.json`, and other non-source files.
- For commit-scoped reviews, read the file content **at that commit**, not the working tree.
- If the resolved scope is empty (e.g. no uncommitted changes), say so and stop — do not write a report.
- If the phrasing is ambiguous about which scope, pick the most likely one and state your assumption before proceeding.

## Step 2 — Load the relevant rules

Read only the rule files for the languages present in the scope. The rules live in the repo's `.claude/rules/` folder.

- **C# (`*.cs`, `*.razor`)** — read all of: `csharp/naming-and-structure.md`, `csharp/types-and-di.md`, `csharp/toolchain.md`, `csharp/async.md`, `csharp/errors-and-logging.md`, `csharp/testing.md`, `csharp/not-allowed.md`.
- **TypeScript / React (`*.ts`, `*.tsx`)** — read the relevant files in `typescript/` (`naming-and-structure.md`, `react.md`, `not-allowed.md`, and others that apply to what you see — forms, i18n, performance, errors, testing, async).
- **PowerShell (`*.ps1`)** — read `powershell/powershell.md`.

These rule files are the source of truth. The standard is "indistinguishable from code written by a senior member of this team." Treat the rules as a checklist — do not rely on memory.

## Step 3 — Review each file

Go file by file. For every file, check it against every applicable rule. Pay special attention to these high-signal violations:

### C# — emphasized checks
- **One class per file.** A `.cs` file must contain exactly one class. The *only* acceptable second type in the file is the single interface that the class directly implements (per `naming-and-structure.md`: interface + class in the same file, file named after the class). Two classes, two unrelated interfaces, or an enum + class in one file are all violations. Flag every extra top-level type and name it.
- File named after the class, never the interface.
- File-scoped namespaces only; namespace matches folder path; production namespaces start with `FatCat.Fakes.*`, tests with `Tests.FatCat.Fakes.*`.
- No expression-bodied members (any access level, including tests).
- No records; no nullable annotations on always-populated/injected values.
- Constructor (primary-constructor) injection only; no `new` for dependencies.
- No `DateTime.UtcNow`/`DateTime.Now`, `Task.Delay`/`Thread.Sleep`, `.Result`/`.Wait()`, `async void`, `ConfigureAwait(false)` in production code.
- Collection expressions (`[]`) not `new List<T>()`; switch expressions with a discard arm over if/else chains.
- **String interpolation, never `+` concatenation.** Require `$"Some string with data {theData}"`; flag any `"..." + value`. No analyzer catches this — it is a review-only rule, so check it carefully.
- Endpoints return `WebResult`; kebab-case error keys on `FatCat.Fakes.Common.Errors`, not inlined.
- **Project boundaries.** Business logic and authorization belong in the API project. Contracts belong in the Common project. The Blazor site project displays and collects only, and never declares its own request/response shapes or hand-rolls a URL.
- **Transport direction.** Client-initiated work is REST; server-initiated work is a Hub signal message through the Hub manager interface — never `IHubContext<T>` directly, never client polling for progress.
- **Privacy.** No logging of user-submitted content or anything identifying a user.
- **Assertions.** `FatCat.Testing`, not FluentAssertions; negation is `.Should().Not.X()`, never `.Should().NotX()`.
- TDD: production class has a matching `<Class>Specs` folder; tests follow the abstract-base + one-concrete-class-per-method layout and verb-first names.

### TypeScript / React — emphasized checks
- No `any`, no `@ts-ignore`/`@ts-expect-error`, no `as` casts to dodge type errors, no `!` non-null assertions.
- Functional components only; props typed as `type <Name>Props`; no components defined inside components.
- Data fetching via the established endpoint hooks, never raw `fetch`/`axios`.
- All user-visible text goes through i18n; keys typed as `LanguageKeys`.
- No `.sort()` (use `.toSorted()`); no barrel imports where a direct import exists.
- New tests are Cypress component tests using `getFake*` data and `data-cy` selectors.

### PowerShell — emphasized checks
- `Verb-Noun` names with approved verbs; one function per file; no aliases; typed params; `[switch]` not `[bool]`.

Be precise. For each violation capture: the file, the line (or line range), what rule it breaks, and the concrete change required to satisfy the rule. Reference the rule file by name. Do not invent rules that are not in `.claude/rules/`.

## Step 4 — Report

**If there are no violations:** report a clean pass inline in the session (briefly list what was reviewed). **Do not write a file.**

**If there are violations:** write one markdown report to `.reviews/`.

- Ensure the folder exists (`.reviews/` at repo root — it is gitignored).
- Filename: `.reviews/<YYYY-MM-DD-HHmmss>-<scope-slug>.md`, where the scope slug describes the target (`uncommitted`, `api`, `site`, `last-commit`, `commit-<shorthash>`, a directory name, etc.). Generate the timestamp with `pwsh -Command '. $PROFILE; (Get-Date).ToString("yyyy-MM-dd-HHmmss")'`.
- Use the template below. It must be **actionable by another session** (precise file/line/change) and **readable by a human** (grouped, plain language, no raw tool dumps).
- After writing, tell the user the report path and give a one-line summary of how many violations were found.

### Report template

```markdown
# Standards Review — <scope description>

- **Reviewed:** <what was in scope, e.g. "uncommitted changes (7 files)">
- **Generated:** <timestamp>
- **Result:** <N> violation(s) across <M> file(s)

> This report was generated by `/standards-review`. To resolve it, point a session at this
> file and ask it to fix the listed violations. **Do not delete this file** — use
> `/clean-reviews` when you want to remove reports.

---

## <relative/path/to/File.cs>

### 1. <short title of the violation>
- **Lines:** <line or range>
- **Rule:** <rule file>, <which rule>
- **Problem:** <plain-language description of what is wrong>
- **Required change:** <the concrete edit needed to comply>

### 2. <next violation in this file>
...

---

## <relative/path/to/Next.razor>
...

---

## Summary checklist
- [ ] <File.cs> — <one-line of what to fix>
- [ ] <Next.razor> — <one-line of what to fix>
```

## Hard rules for this skill
- **Never edit source files.** This skill reviews and reports only. Fixing is a separate, explicit action a session does when pointed at the report.
- **Never delete a report.** If the user asks you to "review the markdown" (i.e. read a report and resolve its issues), fix the *source code* the report points to but leave the report file in place. Removing reports is exclusively `/clean-reviews`.
- **Do not commit anything.** `.reviews/` is gitignored by design.
- Only flag violations that trace to a rule in `.claude/rules/`. If something looks off but no rule covers it, you may add it under an "Observations (not rule violations)" section, clearly separated.
