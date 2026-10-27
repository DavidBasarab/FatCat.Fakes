---
name: unit-test-review
description: Verify the correct unit tests exist for changed production code, per the TDD rules in .claude/rules/csharp/testing.md. Use when the user says "review the tests", "unit test review", "verify test coverage", "are the right tests written", or as the test gate in a phase's Definition of Done. Pairs every changed production class with its <Class>Specs tests, enumerates the behaviors each changed method must cover, and flags missing tests, tests that cannot fail, and test-stack violations. Only when findings exist does it write a report to .reviews/ (gitignored). Ends with an explicit PASS or FAIL verdict so a review loop can gate on it. This skill NEVER edits source code and NEVER deletes report files.
---

# Unit Test Review

Validate that the **correct** unit tests exist for the code in scope — not merely that tests pass. `dotnet test` proves the tests are green; this skill proves the tests are the *right* tests. It **only reads and reports** — it does not fix code, does not write tests, and does not delete report files.

## Step 1 — Resolve the scope

Map the user's phrasing to a concrete set of files. Run all `git` commands from the repo root.

| User says | Scope |
|---|---|
| "review the tests" / no scope given (phase gate) | Staged + unstaged + untracked files: `git status --porcelain`. Review the **current working-tree content**. |
| "review the tests for the Api project" (or Common / Site / any other project) | All source + test files for that project pair (e.g. `Api/FatCat.Fakes.Api` + `Api/Tests.FatCat.Fakes.Api`; every production project pairs with its mirrored test project). |
| "review the tests in the last commit" | Files changed in `HEAD` via `git show --stat --name-only HEAD`; content via `git show HEAD:<path>`. |
| "review the tests in commit `<hash>`" | Same, for `<hash>`. |

Rules:
- Only `*.cs` and `*.razor` files matter. Skip `bin/`, `obj/`, generated code, `.csproj`, config, spike/prototype folders, and end-to-end test projects.
- If the resolved scope is empty, say so, declare **PASS (nothing in scope)**, and stop — do not write a report.
- The scope always includes **both sides of the mirror**: when a production file is in scope, its Specs folder is in scope even if unchanged, and vice versa.

## Step 2 — Load the rules

Read `.claude/rules/csharp/testing.md` and the Testing section of `.claude/rules/csharp/not-allowed.md` (repo root). These are the source of truth — treat them as a checklist, do not rely on memory. Also keep in mind the structure rules from `csharp/naming-and-structure.md` for namespace mirroring.

## Step 3 — Build the pairing map

For every **production** file in scope, locate its test counterpart; for every **test** file in scope, locate its production class.

- `FatCat.Fakes.Api/<path>/<Class>.cs` → `Tests.FatCat.Fakes.Api/<path>/<Class>Specs/` containing an abstract `<Class>Tests` base plus one concrete class per method-under-test. Same shape for the other project pairs.
- Test namespace = production namespace with `Tests.` prepended, mirroring the folder path exactly: `FatCat.Fakes.Api.Orders` → `Tests.FatCat.Fakes.Api.Orders`.
- Test classes inherit the per-service base (named after the service, e.g. `ApiTests`) indirectly through their feature-level abstract base.

Classify every production class in scope as one of:

1. **Requires tests** — anything with logic: endpoints, services, projectors, workers, message processors, helpers, Blazor components.
2. **Exempt** — only these, and each must be verifiable:
   - Classes marked `[ExcludeFromCodeCoverage]` **with a specific `Justification`** naming the low-level API wrapped and confirming no branching logic. If the class contains branching or orchestration, the exemption is invalid — flag it.
   - Autofac `*Module` classes, `GlobalUsings.cs`, `Program`/`*ServiceStarted` startup hooks, custom-projector `Register()` classes (`*CustomProjectors`).
   - Pure contract POCOs (`*Data`, `*ServiceModel`, `*Request`, `*Response`, `*CacheItem`) with no behavior beyond properties.
   - Logging statements (TDD is not enforced for logging).

An in-scope production class that **requires tests** and has no Specs folder, or a changed public method with no corresponding concrete test class, is a **Gap** finding. An orphaned test file whose production class does not exist is also a finding.

## Step 4 — Review through three lenses

### Lens 1 — Coverage: are the right behaviors tested?

For each changed public method on a class that requires tests, enumerate its observable behaviors and check a test exists for **each**:

- Every return path: each guard clause / early return, each enum outcome, each switch arm.
- Every repository interaction: `Create`, `Update`, `Delete` — verified via `MongoFakeRepository<T>` (`VerifyCreate(expected)`, `Item`/`Items` assertions).
- Every Hub push: a test asserting the push happened on the faked Hub manager interface. **Every new server-initiated push ships with this test — no exceptions.**
- Endpoint shape: `endpoint.Should().BePost(nameof(X.Method), "api/Route")` (or `BeGet`/`BePut`/`BeDelete`, from `FatCat.Toolkit.WebServer.Testing`) **and** result-body assertions (`.BeOk().Be(expected)`, `.BeBadRequest(Errors.X)`, `.BeNotFound()`).
- Time-dependent behavior tested through `IDateTimeUtilities`; delay/sleep behavior through `FakeThread`.
- Blazor components (`.razor`): a bunit test rendering the component (through the project's shared bunit test base when one exists) and asserting the observable markup/behavior.

One test verifies exactly one thing. A single test asserting five behaviors is a finding; so is one behavior with no test.

### Lens 2 — Validity: can each test actually fail?

A test that cannot fail is worse than a missing test — it certifies nothing.

- Every `[Fact]` must contain at least one assertion (`.Should()...`) or verification (`MustHaveHappened`).
- **Tautology check:** flag tests that assert a fake's configured return value equals that same configured value, tests that assert against the very object they constructed as the expectation without the SUT transforming anything, and tests where the assertion cannot be affected by the production code under test.
- Flag tests that hit a real database, real file system, real time (`DateTime.UtcNow` outside test-data setup), real threads/delays, a real LLM or embedding model, or the network. Unit tests are deterministic, pure C#; the interfaces are faked.
- Flag hard-coded test data where `Faker.Create<T>()` is required.

### Lens 3 — Test-stack conformance

- Layout: `<Class>Specs` folder, abstract `<Class>Tests` base holding fakes/SUT/`Verify*` helpers, one concrete class per method-under-test, `protected readonly` fields, SUT constructed in the protected base constructor.
- Naming: verb-first `[Fact]` names (`ReturnOk`, `CreateTheOrderDataInMongo`) — no `Should`, no underscores, no Given/When/Then.
- Assertions: `FatCat.Testing` only (`.Should()`, `.Be()`, `.BeEquivalentTo()`); negation is `.Should().Not.X()`, never `.Should().NotX()`; no `FluentAssertions` package reference. Endpoint shape helpers from `FatCat.Toolkit.WebServer.Testing`.
- Fakes: `A.Fake<T>()` / `A.CallTo(...)`; matchers are `A<T>._`, never `A<T>.Ignored`; `ReturnsLazily` for values that vary per test.
- Mongo: concrete `MongoFakeRepository<T>`, never `A.Fake<IMongoRepository<T>>()`.
- Threading: `FakeThread`, never real delays.
- bunit: derive from `BunitContext` — directly, or through the project's shared bunit test base (which extends `BunitContext`) and its render helper when one exists. `TestContext` and bunit's legacy `RenderComponent<T>()` are findings.
- Durations: Humanizer fluent extensions (`3.Seconds()`), never `TimeSpan.From*`.
- Block bodies everywhere — the expression-bodied ban applies to test code.
- Equivalency tolerances (`Equivalency.Using<T>(...)`) and custom `Faker.AddGenerator` calls registered in the per-service base's static constructor (or a fixture/constructor), never inside a `[Fact]`.

## Step 5 — Report and verdict

Categorize every finding:

- **Gap** — a behavior, branch, push, or class with no test. (Blocks the gate.)
- **Defect** — a test that cannot fail, tests real infrastructure, or verifies the wrong thing. (Blocks the gate.)
- **Conformance** — wrong layout, naming, assertion library, fake pattern. (Blocks the gate.)

**If there are no findings:** state **`Unit test review: PASS`** inline with a one-paragraph summary of what was paired and checked. **Do not write a file.**

**If there are findings:** write one markdown report to `.reviews/` and end with **`Unit test review: FAIL — <N> finding(s)`**.

- Filename: `.reviews/<YYYY-MM-DD-HHmmss>-tests-<scope-slug>.md`. Generate the timestamp with `pwsh -Command '. $PROFILE; (Get-Date).ToString("yyyy-MM-dd-HHmmss")'`.
- The report must be actionable by another session (precise file, class, method, missing behavior, required test) and readable by a human.

### Report template

```markdown
# Unit Test Review — <scope description>

- **Reviewed:** <scope, e.g. "uncommitted changes (5 production files, 3 test files)">
- **Generated:** <timestamp>
- **Verdict:** FAIL — <N> finding(s): <g> gap(s), <d> defect(s), <c> conformance
- **Pairing map:** <one line per production class → its Specs folder or "MISSING">

> Generated by `/unit-test-review`. To resolve, point a session at this file and ask it
> to write the missing tests / fix the listed findings, then re-run `/unit-test-review`.
> **Do not delete this file** — use `/clean-reviews` to remove reports.

---

## <relative/path/to/Class.cs>

### 1. [Gap] <behavior with no test>
- **Method:** <method name>
- **Behavior:** <the observable outcome that is untested>
- **Rule:** csharp/testing.md, <which rule>
- **Required test:** <concrete test to add — target Specs class, verb-first name, what it asserts>

### 2. [Defect] <test that cannot fail>
- **Test:** <Specs class>.<FactName> (<file>:<line>)
- **Problem:** <why it certifies nothing>
- **Required change:** <the fix>

---

## Summary checklist
- [ ] <Class> — <one line per finding>
```

## Hard rules for this skill

- **Never edit source or test files.** Review and report only; writing the missing tests is a separate, explicit action by a session pointed at the report.
- **Never delete a report.** Removing reports is exclusively `/clean-reviews`.
- **Do not commit anything.** `.reviews/` is gitignored by design.
- **Never run `dotnet test` as a substitute for this review.** Passing tests are a separate gate; this skill judges whether the *right* tests exist.
- Ground every Gap/Defect/Conformance finding in `.claude/rules/csharp/testing.md` or `not-allowed.md`. Judgment calls that no rule covers go in a clearly separated "Observations (not findings)" section and do not affect the verdict.
- Always end with the explicit verdict line (`Unit test review: PASS` or `Unit test review: FAIL — <N> finding(s)`) so a phase's review loop can gate on it.
