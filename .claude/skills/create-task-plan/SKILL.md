---
name: create-task-plan
description: Turn a rough brief markdown into a completed planning document built from the repo task template (`task.md` at the repo root), saved as {work-item}-plan.md in tasks/planning. Use when the user says "create task plan", "make a planning doc", "fill the task template from this brief", "plan this out for a separate session", or points at a brief markdown and asks for a planning document. Reads the brief, the README, and the real code it touches, asks clarifying questions on material ambiguities, then fills the template's specification half. The resulting doc is a hand-off consumed in a SEPARATE session to generate the phased plan. This skill writes ONLY planning markdown and NEVER production code or tests.
---

# Create Task Plan

Convert an informal **brief markdown** into a rigorous **planning document** built from the repo task template. The output is the complete specification hand-off that a **separate later session** reads to generate the phased plan — this skill does **not** generate the phased plan itself, and **never writes production code or tests.**

The quality bar: read the brief, read the real code it touches, ask about anything that materially changes the design, then fill the template with concrete file links, testable acceptance criteria, explicit non-goals, and planner guidance — flagging anything security-sensitive as high-risk.

## Step 1 — Locate and read the source brief

- The source is a markdown file the user names, pastes a path to, or has open in the IDE (an `<ide_opened_file>` hint). If none is identified, ask for the path.
- Read the **entire** brief.
- Resolve and read every reference it contains — **especially images** (`![...](image.png)` and similar): read screenshots with the Read tool so you understand the screen. Read any linked docs, prior briefs, or wireframes it points to.
- Relative paths are relative to the brief's own folder.

## Step 2 — Read the repo task template

Read `task.md` at the repo root in full. It is the scaffold for the output. It has two halves:

- **Top half (you fill this in):** `# Work Item`, `# Specification`, `## Acceptance Criteria`, `## Out of Scope / Non-Goals`, `## Feature Context`.
- **Bottom half (preserve VERBATIM):** everything from `# Planning Instructions` onward — Planning Instructions, Phase Requirements, Orchestrator, Definition of Done, Quality Gates, Commit Policy, Phase Report, References, Notes. These are the standing instructions the **separate planning session** will follow; do not reword them. You may **append** task-specific bullets to `# References` and `# Notes`, but never rewrite the standing content.

Always read the template fresh each run — it is the source of truth and may change.

## Step 3 — Gather real context before drafting

The template's own first planning instruction is "Read all references before planning." Do that now, against the actual codebase:

- Read `README.md` (repo overview, standing rules, ownership) unless the task is trivially self-contained.
- Explore the code the brief touches: find the exact pages/components/endpoints/data models/APIs involved. Use Grep/Glob/Read (or the Explore agent for broad fan-out). Confirm what already exists versus what must be built.
- Surface the gotchas that change the design: missing endpoints, derived-vs-persisted data, existing prior-art patterns to mirror, invariants (e.g. the anonymous-route exclusion list, security trust anchors), and anything the brief assumes but the code contradicts. Standing rule: **where the README and the code disagree, the code is authoritative** — trust the code and flag the stale doc.

Prefer citing concrete files with repo-relative markdown links (e.g. `[UpdateOrderEndpoint.cs](Api/Orders/Endpoints/UpdateOrderEndpoint.cs)`).

## Step 4 — Ask clarifying questions

Before writing anything, identify the ambiguities that **materially change the design** and ask them with `AskUserQuestion` (batch them; give a recommended option first where you have one). Examples of material ambiguity: which fields/entities are in scope, gating/permission rules, security-sensitive behaviors, business rules and their limits, audit/telemetry detail level, backward-compatibility.

Do **not** ask about things you can determine from the code or sensible defaults. If nothing is materially ambiguous, skip this step. If an answer opens a new material ambiguity, ask again rather than guess.

## Step 5 — Fill the template

Produce the completed planning doc. Fill the top half from the brief + context + answers:

- **Work Item** — a short kebab-case identifier; it becomes the output file name and the eventual plan folder name. Keep it stable and specific.
- **Specification** — the stakeholder's ask in their intent, expanded into concrete screens, behaviors, and edge cases, with the decisions from Step 4 baked in. Name the exact target files. State business rules with their limits and where they are enforced (UI vs API — the API is the authority).
- **Acceptance Criteria** — testable `- [ ]` checkboxes, Given/When/Then where it helps. Include a final **E2E walk** bullet that proves the criteria end to end. Include a criterion that new/changed code is covered test-first.
- **Out of Scope / Non-Goals** — explicit exclusions that prevent scope drift, including adjacent things a reader might assume are included.
- **Feature Context** — links to the real files, prior art, and (as a sub-section) **planner guidance**: the cross-cutting decisions, the recommended approach, and the alternatives to reject. This is what the separate planning session leans on. Include a **parallelism sketch**: the shared contract that should land first (service models, interfaces, routes, `Errors` keys, registrations), the independent work streams that can fan out after it, each with the files it will write, and the work that must stay serial and why (shared hotspot files, local Mongo or a running site, human gates, the E2E walk).

Then:

- **Preserve the bottom half verbatim.** Append task-specific bullets to `# References` (the brief path, screenshot path, the key files) and to `# Notes` only if the task needs them.
- **Flag security-sensitive / high-risk aspects** anywhere they apply (auth, anonymous endpoints, data migration, public API contracts, trust anchors). Add a note that the relevant phases will be automatically high-risk and need extra human review — mirroring the template's Risk-rated rule.

## Step 6 — Write the output

- The output file name is `{work-item}-plan.md`, where `{work-item}` is the kebab-case Work Item identifier from Step 5 — **unless the user gave an explicit name**, in which case use that (kebab-cased, with the `-plan` suffix).
- Ensure `tasks/planning/` exists and write the completed doc to **`tasks/planning/{work-item}-plan.md`**.
- If a file already exists at that path, tell the user and confirm before overwriting.

This skill stops at the filled planning doc. **Do not generate the phased plan** — that happens in a separate session that reads this doc and follows its Planning Instructions.

## Step 7 — Report

Briefly report: the work-item id, the output path, the material decisions captured (and who decided them), any high-risk flags, and any open questions left for the human. Note that the doc is ready for a separate planning session. Keep it tight — the user reads the doc for detail.

## Hard rules

- Never write production code or tests — this skill only produces the planning markdown.
- Never generate the phased plan or the orchestrator — that is a separate session's job.
- Never reword the template's bottom half; preserve it verbatim and only append task-specific references/notes.
- Never invent answers to material ambiguities — ask.
- Prefer concrete, clickable file links over prose descriptions of where code lives.
