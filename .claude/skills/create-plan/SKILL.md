---
name: create-plan
description: Turn a rough one-off task markdown (a freeform brief, screenshot, or note like tasks/one_off/one_off_task.md) into a completed planning task document based on the repo task template (`task.md` at the repo root). Use when the user says "create task", "create phased plan", "make a plan from this one-off", "turn this brief into a task", "plan this based on the task template", or points at a one-off markdown and asks for a plan. Two modes — "create task" fills the task doc and stops for review; "create phased plan" also generates the phased plan into tasks/todo. This skill reads references and asks clarifying questions before drafting; it writes ONLY planning docs and NEVER production code.
---

# Create Plan

Convert an informal **one-off brief** into a rigorous **planning task document** built from the generic template, and — on request — into a full **phased plan**. This is a specification/planning activity: **no production code is ever written by this skill.**

The quality bar is the worked example the user already accepted: read the brief, read the real code it touches, ask about anything that materially changes the design, then fill the template with concrete file links, testable acceptance criteria, explicit non-goals, and ADR guidance — flagging anything security-sensitive as high-risk.

## Step 1 — Determine the mode

The user's verb selects the mode:

| User says | Mode | Deliverable |
|---|---|---|
| "create task", "make the task", "fill in the task", "draft the task for review" | **task** | Filled task doc only. Stop for the user to review. |
| "create phased plan", "create the plan", "plan it out", "make the phased plan" | **plan** | Filled task doc **and** the phased plan in `tasks/todo/{work-item}/`. |

If the phrasing is ambiguous (e.g. just "plan this"), ask which mode with a one-line question before doing any work. Do not guess — the modes differ a lot in output.

## Step 2 — Locate and read the source brief

- The source is a markdown file the user names, pastes a path to, or has open in the IDE (an `<ide_opened_file>` hint). If none is identified, ask for the path.
- Read the **entire** brief.
- Resolve and read every reference it contains — **especially images** (`![...](image.png)` and similar): read screenshots with the Read tool so you understand the screen. Read any linked docs, prior briefs, or wireframes it points to.
- Note relative paths are relative to the brief's own folder.

## Step 3 — Read the repo task template

Read `task.md` at the repo root in full. It is the scaffold for the output. It has two halves:

- **Top half (you fill this in):** `# Work Item`, `# Specification`, `## Acceptance Criteria`, `## Out of Scope / Non-Goals`, `## Feature Context`.
- **Bottom half (preserve VERBATIM):** everything from `# Planning Instructions` onward — Phase Requirements, Orchestrator, Definition of Done, Quality Gates, Commit Policy, Phase Report, References, Notes. Do not reword these; copy them exactly. You may **append** task-specific references/notes to the `# References` and `# Notes` sections, but never rewrite the standing content.

Always read the template fresh each run — it is the source of truth and may change.

## Step 4 — Gather real context before drafting

The template's own first instruction is "Read all references before planning." Do that now, against the actual codebase:

- Read `README.md` (repo overview, standing rules, ownership) unless the task is trivially self-contained.
- Explore the code the brief touches: find the exact pages/components/endpoints/data models/APIs involved. Use Grep/Glob/Read (or the Explore agent for broad fan-out). Confirm what already exists versus what must be built.
- Surface the gotchas that change the design: missing endpoints, derived-vs-persisted data, existing prior-art patterns to mirror, invariants (e.g. anonymous-route exclusion lists, security trust anchors), and anything the brief assumes but the code contradicts. The README rule holds: **where the README and the code disagree, the code is authoritative** — trust the code and flag the stale doc.

Prefer citing concrete files with repo-relative markdown links (e.g. `[UpdateOrderEndpoint.cs](Api/Orders/Endpoints/UpdateOrderEndpoint.cs)`).

## Step 5 — Ask clarifying questions

Before writing anything, identify the ambiguities that **materially change the design** and ask them with `AskUserQuestion` (batch them; give a recommended option first where you have one). Examples of material ambiguity: which fields/entities are in scope, gating/permission rules, security-sensitive behaviors, business rules and their limits, audit/telemetry detail level, where output lives, backward-compatibility.

Do **not** ask about things you can determine from the code or sensible defaults. If nothing is materially ambiguous, skip this step. If an answer opens a new material ambiguity, ask again rather than guess.

## Step 6 — Fill the template

Produce the completed task doc. Fill the top half from the brief + context + answers:

- **Work Item** — a short kebab-case identifier; it becomes the plan folder name. Keep it stable and specific.
- **Specification** — the stakeholder's ask in their intent, expanded into concrete screens, behaviors, and edge cases, with the decisions from Step 5 baked in. Name the exact target files. State business rules with their limits and where they are enforced (UI vs API — the API is the authority).
- **Acceptance Criteria** — testable `- [ ]` checkboxes, Given/When/Then where it helps. Include a final **E2E walk** bullet that proves the criteria end to end. Include a criterion that new/changed code is covered test-first.
- **Out of Scope / Non-Goals** — explicit exclusions that prevent scope drift, including adjacent things a reader might assume are included.
- **Feature Context** — links to the real files, prior art, and (as a sub-section) **ADR guidance** for the planner: the cross-cutting decisions, the recommended approach, and the alternatives to reject.

Then:

- **Preserve the bottom half verbatim.** Append task-specific bullets to `# References` (the brief path, screenshot path, the key files) and to `# Notes` only if the task needs them.
- **Flag security-sensitive / high-risk aspects** anywhere they apply (auth, anonymous endpoints, data migration, public API contracts, trust anchors). Add a note under Planning Instructions that the relevant phases are automatically high-risk and need extra human review — mirroring the template's Risk-rated rule.

## Step 7 — Write the output

- Ensure `tasks/plans/` exists and write the completed task doc to **`tasks/plans/{work-item}.md`**.
- If a file already exists at that path, tell the user and confirm before overwriting.

Then, by mode:

- **Mode = task:** stop here. Tell the user the doc is ready at `tasks/plans/{work-item}.md` for review, and that they can re-run with "create phased plan" to generate the phased plan.
- **Mode = plan:** continue and execute the template's **Planning Instructions** against the just-written task doc:
  - Write a phased plan to **`tasks/todo/{work-item}/`** — `00-overview.md` plus one file per phase.
  - `00-overview.md` records the cross-cutting decisions as lightweight ADRs (decision, context, alternatives rejected), plus **assumptions and open questions**. If an open question materially changes the design and was not resolved in Step 5, stop and ask rather than guess.
  - **Design for parallelism before writing phase files.** Draft the phase list, then for each phase estimate its write set (the real files from Step 4) and any exclusive runtime resource. Build the dependency graph, then group the phases into waves using the template's parallel-safe rules. If two phases are serial only because they share one hotspot file (for example both add an `Errors` key), move that edit into the earlier contract phase so they can fan out. Put the wave table, dependency graph and critical path in `00-overview.md`.
  - Each phase file satisfies every bullet under **Phase Requirements**: context-isolated, atomic (one commit), dependency graph (`Depends on:` / `Depended on by:`), `Wave:` / `Parallel with:`, `## Touches`, `## Rollback Procedure`, verification steps + `## Definition of Done`, `## Hand-off` contract, and a risk rating (auth/anonymous/data-migration/public-API phases are automatically high).
  - Create the phase **orchestrator** (runbook or script) per the Orchestrator section: wave-by-wave execution, fresh session per phase, one worktree per concurrent phase (max 3), cherry-pick in phase order plus the integration gate after each multi-phase wave, commit boundary between phases, halt-on-failure policy, a final step that `git mv`s the plan folder from `tasks/todo/{work-item}` to `tasks/done/{work-item}` in its own commit once every phase is committed, never force-push/rebase/push.
  - This is still planning only — **write no production code and no tests**; the phases describe the work for later execution sessions.

## Step 8 — Report

Briefly report: the mode, the work-item id, the output path(s), the material decisions captured (and who decided them), any high-risk flags, and any open questions left for the human. Keep it tight — the user reads the doc for detail.

## Hard rules

- Never write production code or tests — this skill only produces planning markdown.
- Never reword the template's bottom half; preserve it verbatim and only append task-specific references/notes.
- Never invent answers to material ambiguities — ask.
- Prefer concrete, clickable file links over prose descriptions of where code lives.
