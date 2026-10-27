# Work Item

<!-- Short kebab-case identifier; becomes the plan folder name -->

# Specification

<!-- What the product manager / stakeholder asked for, in their words.
     Screens, behaviors, edge cases. This is the source of truth. -->

## Acceptance Criteria

<!-- Testable statements of done. Given/When/Then where it helps.
     For any user-visible or deployed behaviour, the end-to-end suite (or the
     E2E walk, if the project has no automated suite) must prove these. The plan
     runs it ONCE, in its final phase, as the single end-to-end gate — mid-plan
     phases gate on the unit tests only. -->

- [ ]

## Out of Scope / Non-Goals

<!-- Explicitly what NOT to build. Prevents gold-plating and scope drift. -->


## Feature Context

<!-- Links to wireframes, workflow docs, prior art -->

# Planning Instructions

This is a **specification and planning task. No production code is written in this session.**

- [ ] Read all references before planning
- [ ] Write a phased implementation plan to `tasks\todo\{WORK_ITEM}` (one file per phase plus `00-overview.md`)
- [ ] Record cross-cutting decisions in `00-overview.md` (treat these as lightweight ADRs — architecture decision records: decision, context, alternatives rejected)
- [ ] List **assumptions and open questions** in the overview. If an ambiguity materially changes the design, stop and ask rather than guess
- [ ] Keep commits small.
- [ ] **Plan for parallelism.** Shape the phases so as much work as possible can run at the same time. Put the shared contract (service models, requests/responses, interfaces, routes, `Errors` keys, module/projector registrations) in an early, small phase. Then fan the independent work out against that contract, for example the API endpoint, the public site page and the admin page as separate phases. Do not split a phase artificially just to parallelize: every phase must still be a coherent, atomic commit.
- [ ] Group the phases into **waves** in `00-overview.md`. A wave is a set of phases whose dependencies all sit in earlier waves and that are pairwise **parallel-safe** (see Phase Requirements). Include the dependency graph, a wave table (`Wave | Phases | Why parallel-safe`) and the critical path: the longest dependency chain, which sets the minimum number of sequential steps.

## Phase Requirements

Each phase must be:

- [ ] **Context-isolated** — executable in a fresh session with no state from prior sessions; the phase file is the complete handoff document
- [ ] **Atomic** — exactly one commit per phase; the commit message references the phase file
- [ ] **Independently revertible** — the phase declares its position in the **dependency graph** (`Depends on:` / `Depended on by:`) so a revert can cascade correctly (reverting phase 3 tells me to also revert 4 and 5 if they depend on it)
- [ ] **Reversible** — each phase includes a `## Rollback Procedure` section (the "down migration"): exact `git revert` target plus any manual steps (data, config, feature flags)
- [ ] **Verifiable** — each phase includes verification steps and a `## Definition of Done` checklist
- [ ] **Contract-defining** — each phase ends with a `## Hand-off` section: the interfaces, types, and routes it exposes to later phases
- [ ] **Risk-rated** — each phase declares a risk level (low / medium / high) and why. Anything touching auth, anonymous endpoints, data migration, or public API contracts is automatically high and flagged for extra human review
- [ ] **Footprint-declared:** each phase has a `## Touches` section. It lists every file or folder the phase creates, modifies or deletes (best estimate, globs allowed), plus any shared runtime resource it needs exclusively: local Mongo data, a running API or site, the browser, fixed ports, or a real external service.
- [ ] **Parallel-classified:** each phase declares `Wave:` and `Parallel with:` (phase numbers, or `none` and why). Two phases are parallel-safe only when **all** of these hold:
  - neither depends on the other, directly or transitively;
  - their `Touches` write sets are disjoint;
  - they do not both edit a **shared hotspot**: the `Errors.cs` error-key class, `*Module.cs`, `*CustomProjectors.cs`, `GlobalUsings.cs`, `*.csproj` / `Directory.Build.props` / `FatCat.Fakes.sln`, `README.md`, the anonymous-route session-check exclusion list(s), the shared design tokens (CSS variables / Tailwind theme), or shared UI components;
  - neither needs an exclusive runtime resource (above);
  - neither is a human-gated phase, an E2E walk, or a report that rolls up other phases. These always run alone.
  When in doubt, serialize. A wrong "parallel" claim costs a merge conflict and a halted pipeline. A wrong "serial" claim only costs time.

## Orchestrator

- [ ] Create a phase orchestrator (runbook or script) that executes phases **wave by wave**, in dependency order
- [ ] The orchestrator MUST spawn a **fresh session per phase** (context isolation is not optional — never run multiple phases in one session/context window)
- [ ] **Parallel waves:** a wave with one phase runs in the main working tree, as before. A wave with several phases runs them concurrently, **at most 3 at a time**, with each phase in its own fresh session and its own git worktree. Two sessions never share a working tree, because builds, tests and the git index collide.
  - Create each worktree from the current task-branch tip: `git worktree add ..\FatCat.Fakes-wt\{WORK_ITEM}-{NN} -b wt/{WORK_ITEM}/{NN}`. Copy the git-excluded local files a build needs (for example `global.json` and local `appsettings.json` overrides) into it.
  - Each phase meets its full Definition of Done and commits once, inside its worktree.
  - When every phase in the wave has committed, cherry-pick the phase commits onto the task branch **in phase-number order**. Do not squash, amend or rewrite them.
  - **Integration gate** after each multi-phase wave, on the task branch: `dotnet build` with no new warnings, then `dotnet test FatCat.Fakes.sln` green apart from known flakes. A cherry-pick conflict or a red integration gate means the plan's parallel claim was wrong, so treat it as a failure (halt policy below). Do not hand-merge it.
  - After a successful integration, remove the wave's worktrees and `wt/` branches (`git worktree remove`, `git branch -D`).
  - If one phase in a wave fails, let its siblings finish. Integrate only the phases that passed and do not depend on the failed one, then halt.
- [ ] Commit boundary between every phase; the orchestrator never squashes or amends
- [ ] **Move the plan to done:** after the last phase is committed and its integration gate is green, move the whole plan folder with `git mv tasks\todo\{WORK_ITEM} tasks\done\{WORK_ITEM}`. This includes the phase files, reports, overview and orchestrator. Commit the move as its own commit, `{WORK_ITEM} complete - move plan to tasks/done`. Only move it when every phase is committed. If the pipeline halted or is paused at a human gate, the folder stays in `tasks\todo`. If `tasks\done\{WORK_ITEM}` already exists, stop and ask instead of overwriting it.
- [ ] **Halt-on-failure policy:** if a phase's Definition of Done cannot be met after 2 self-correction attempts, STOP the pipeline, leave the working tree clean (stash or discard), write a failure report, and do not start dependent phases
- [ ] The orchestrator never force-pushes, never rebases published history, and never pushes to remote

# Definition of Done (every phase)

## Quality Gates

- [ ] Tests written before implementation (TDD — red before green)
- [ ] No compiler warnings introduced
- [ ] Namespaces match folder paths exactly
- [ ] Must follow all rules in `.claude\rules\csharp` — no exceptions
- [ ] No banned patterns used (see `.claude/rules/not-allowed.md`)
- [ ] All tests pass (`dotnet test`)
- [ ] `dotnet format` run on all modified files
- [ ] `dotnet build` to apply CSharpier changes
- [ ] **Unit tests are the per-phase gate; end-to-end is the plan's final-phase gate.** If the project has an end-to-end suite, it is excluded from `dotnet test` and is **not** run per phase. A phase that adds or changes a deployed surface (an endpoint, a Hub push, a site page) may author or extend end-to-end scenarios alongside it, but does not run the suite green. The plan's **final phase** runs the **whole** suite (never a filtered subset) and it must finish fully green **twice in a row, with no manual step between the runs**. If the plan changed the end-to-end tests and the final phase cannot get a clean run, it fixes them or STOPS and reports to the human; never leave the suite red or silently skip it
- [ ] Run `unit-test-review` on uncommitted code; it must end with `Unit test review: PASS` — resolve every gap, defect, and conformance finding (write the missing tests) before moving to `code-review`
- [ ] Run `code-review` on uncommitted code; resolve any findings before commit
- [ ] Run `code-security-review` after `code-review` passes; any vulnerabilities found MUST be resolved before commit
- [ ] **Review loop:** code changes → `unit-test-review` until it passes → `code-review` until it passes → `code-security-review`. Any fix made at a later stage sends you back to the start of the loop (`unit-test-review`), because fixes are code changes that need tests too. Repeat until all three reviews pass with no findings — only then commit

## Commit Policy

- [ ] Work happens on the task's feature branch (`task/{WORK_ITEM}`), never on main  (only create the branch if not on main if you are on a different branch then main use the current branch)
- [ ] Create the phase commit locally; **do not push to remote** — human review gates the push

## Phase Report

Before finishing, produce a structured report:

- [ ] Files added / changed / deleted, and any file touched outside the phase's declared `## Touches` (a footprint miss can break parallel siblings)
- [ ] Test counts (new, total, passing)
- [ ] **Deviation log** — every place the implementation departed from the plan file, and why (an empty deviation log is a claim, not a default)
- [ ] Open questions or discovered risks for the human reviewer

# References

- Read the `README.md` for an overview of the application

# Notes

- Use components as needed to simplify changes and for look and feel.
- I control FatCat.Toolkit the source is found here `C:\Code\FatCat.Toolkit\src` if making a change to the toolkit would then suggest those changes.  A task would be written in `C:\Code\FatCat.Toolkit\src\task\todo` following the same rules found in this document.

