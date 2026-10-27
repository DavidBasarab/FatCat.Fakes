---
name: code-security-review
description: Full-application security review. Always scans the ENTIRE source tree — never just a diff, project, or commit — and flags the top vulnerability classes using OWASP (Top 10, ASVS, and the LLM Top 10 where the application uses an LLM), Microsoft Secure Coding Guidance, the CWE Top 25, and GitHub CodeQL query coverage as the knowledge base. Use when the user says "security review", "check for vulnerabilities", "scan for security issues", or similar. Writes a dated, branch-stamped report to vulnerabilities/unresolved/ at the repo root — unless screen-only output is requested (e.g. by another Claude instance), in which case the report is printed in the session and no file is written. When a session later fixes a report's findings, the report moves to vulnerabilities/resolved/ — that move happens in the fixing session, never in this skill. This skill NEVER edits source code.
---

# Code Security Review

Review the **entire application** for security vulnerabilities and produce an actionable report. This skill **only reads and reports** — it does not fix code, does not commit anything, does not delete reports, and does not move reports between folders.

Unlike a style-focused code review, security review is never diff-scoped: a vulnerability can live in code nobody touched this week, and a change in one file can make old code in another file exploitable. **Every run covers the full source tree.** If the user names a narrower target ("security review the Api"), still review everything — you may organize the report so their named area appears first, but do not skip the rest.

## Report folders

Reports live in two committed folders at the repo root:

- `vulnerabilities/unresolved/` — reports whose findings have not all been fixed yet. New reports always land here.
- `vulnerabilities/resolved/` — reports whose findings have all been fixed. Reports are moved here **only by the session that fixed them** (see "Resolution workflow" below), never by this skill.

Create either folder if it does not exist.

## Step 0 — Choose the output mode

Two output modes:

- **File mode (default):** write the report to `vulnerabilities/unresolved/`.
- **Screen mode:** print the full report as markdown in the session and write **nothing** to disk.

Use screen mode when any of the following is true:
- The skill is invoked with an argument such as `screen`, `no-file`, `no-write`, or `stdout` (e.g. `/code-security-review screen`).
- The user's phrasing asks for it: "don't write a file", "just show me", "output to the screen", "inline".
- The skill is being run by another Claude instance / automated session as part of a larger task, where the findings will be consumed directly from the conversation rather than from disk.

If none of those apply, use file mode.

## Step 1 — Gather the scope and run context

The scope is always the whole repository, from the repo root:

- Review source and configuration that can carry vulnerabilities: `*.cs`, `*.razor`, `*.ps1`, JavaScript interop files under the Blazor projects' `wwwroot/`, `*.csproj` (package references), `appsettings*.json`, `*.yml`/`*.yaml`, `Dockerfile`, compose files.
- Skip generated code, `bin/`, `obj/`, `node_modules/`, compiled Tailwind output in `wwwroot/css/`, lock files, and prior reports in `vulnerabilities/`.
- Test projects, end-to-end test projects, and spike/prototype folders are in scope **only** for hard-coded secrets and credentials — application vulnerability classes do not apply to code that does not ship.
- Review the **current working-tree content** of each file (uncommitted changes included — they are part of the application as it stands).

Record the run context up front — it goes in the report header:
- **Datetime:** `pwsh -Command '. $PROFILE; (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")'` (and `yyyy-MM-dd-HHmmss` for the filename).
- **Branch:** `git branch --show-current` (if empty — detached HEAD — use `git rev-parse --short HEAD`).
- **Commit:** `git rev-parse --short HEAD`.

## Step 2 — Know what to look for

The knowledge base for this review is the union of these four sources. Do not invent exotic findings — anchor every finding to at least one of them:

1. **OWASP** — Top 10 (A01 Broken Access Control … A10 SSRF), the relevant ASVS controls, and — when the application uses an LLM (e.g. a RAG application) — the **OWASP Top 10 for LLM Applications** (prompt injection, insecure output handling, training/context data poisoning, excessive agency).
2. **Microsoft Secure Coding Guidance** — .NET-specific guidance: secure use of cryptography APIs, `System.Random` vs `RandomNumberGenerator`, deserialization (`BinaryFormatter` is banned), path handling, secrets in configuration, ASP.NET Core authentication/authorization patterns, CORS, anti-forgery.
3. **CWE Top 25** — the current Top 25 Most Dangerous Software Weaknesses. Tag every finding with its CWE ID (e.g. CWE-89, CWE-79, CWE-798, CWE-22, CWE-918).
4. **GitHub CodeQL** — the classes of issues CodeQL's C# query packs flag: tainted data flows (source → sink), log forging, regex injection, weak randomness, hard-coded credentials, missing certificate validation, XML external entities.

### High-signal checks for this codebase

The product's security guarantees (see the README) are the product. Check these deliberately, not just generically:

- **Broken access control (A01, CWE-862/CWE-863):** the API is the **sole security boundary** — any authorization decision made only in the Blazor WASM app(s) is a finding, since WASM code runs on the attacker's machine. IDOR on any entity ID a caller can supply. Admin endpoints or Hub methods reachable without authorization. Service-to-service endpoints trusting unauthenticated callers.
- **Product guarantees enforced server-side:** any state the product promises is irreversible (e.g. access revoked after submission, a record sealed or locked) must be enforced by every read/write endpoint on the server — a bypass here defeats the product's core promise. Treat as **Critical**.
- **Authentication flows:** passkey/FIDO2/WebAuthn register/assert (challenge single-use, origin/RP-ID validation), nonce-based login (entropy, expiry, single-use), cross-device handoffs such as QR desktop↔mobile login (session fixation, token leakage), and single-use access codes (entropy, brute-force throttling, genuinely single-use).
- **Crypto / randomness (CWE-327/CWE-338):** any encryption pipeline (e.g. ECDH → SHA-256 → AES-GCM) — AES-GCM nonce/IV reuse under the same key is catastrophic; check nonce generation per encrypted unit. `System.Random` (or `IGenerator` backed by it) for anything with security meaning: tokens, IDs, access codes, login nonces, invite tokens. MD5/SHA1 for security purposes.
- **Injection (CWE-89/CWE-943):** MongoDB query construction from user input — filters built by string interpolation or `BsonDocument.Parse` on request data instead of typed builders / `IMongoRepository<T>`.
- **Prompt injection / insecure LLM output (OWASP LLM01/LLM02)** — when the application uses an LLM: user input or retrieved content concatenated into prompts without delimitation; LLM output rendered or executed without treating it as untrusted; grounding checks bypassable from input.
- **Privacy leaks (project non-negotiable):** user-submitted content, identity documents or images, biometric results, phone numbers, or IP addresses reaching logs, exception messages, metrics, persisted analytics, or debug output. Treat as **High** — this violates the product's guarantees to its users.
- **XSS (CWE-79):** `MarkupString` / raw HTML rendering in Blazor of anything derived from user input, document content, OCR'd text, or LLM output; unsafe `IJSRuntime` interop passing user data into JS.
- **SignalR hubs:** hub methods (the Hub manager interface, `SignalMessage` processors) trusting client-supplied identity or another service's identity without verification; messages routed to connections the sender should not reach.
- **Path traversal (CWE-22):** file, block, blob, or object-storage paths built from request, document, or client-supplied data.
- **SSRF (A10, CWE-918):** any outbound request whose URL is influenced by user, document, or client input (ingestion fetches, webhook-style calls, service registration, service-location configuration accepted from requests).
- **Secrets (CWE-798):** third-party provider credentials (SMS, email, identity verification), cloud connection strings, FIDO2 config, API keys, or tokens hard-coded in source or committed in `appsettings*.json`.
- **Deserialization (CWE-502):** `BinaryFormatter`, `Newtonsoft` with `TypeNameHandling`, or polymorphic deserialization of untrusted data.
- **Transport/config:** missing HTTPS redirection/HSTS, permissive CORS, anti-forgery gaps on state-changing endpoints, Docker images running as root with secrets baked in.
- **Vulnerable dependencies (A06):** check `dotnet list package --vulnerable` when feasible and flag known-vulnerable references.

## Step 3 — Review each file

Go file by file, tracing user-controlled data from where it enters (endpoint request bodies, Hub messages, uploaded or ingested documents and images, query strings, links delivered by SMS or email) to where it is used (Mongo queries, prompts, logs, HTML, file/blob paths, outbound HTTP, crypto operations).

For every finding capture:
- **File and line (or range).**
- **Severity:** `Critical` (remotely exploitable, direct data/privacy compromise, compromise of identity evidence, or of a core product guarantee), `High` (exploitable with conditions, or a privacy-guarantee violation), `Medium` (defense-in-depth gap, hardening), `Low` (best-practice deviation with limited impact).
- **Classification:** CWE ID + OWASP category (and CodeQL query family or Microsoft guidance reference where it applies).
- **Problem:** what an attacker can actually do, in plain language.
- **Suggested fix:** the concrete change — named APIs, patterns, or code direction that complies with the repo's coding standards (e.g. fixes must still use constructor injection, `IThread`, `IDateTimeUtilities`, translation-keyed errors on `FatCat.Fakes.Common.Errors`).

Before writing a finding, check `vulnerabilities/unresolved/` for existing reports: if the same issue is already reported there, still include it (each report stands alone as a full picture of the branch at its run time), but note that it also appears in the earlier report.

Be precise and honest. Do not pad the report with theoretical findings that have no path to exploitation here — if something is only an observation, label it as one. If the codebase is clean, say so.

## Step 4 — Report

Order findings by severity (Critical first). Use the template below in both modes.

**Screen mode:** print the full report in the session. State clearly that no file was written.

**File mode:**
- Ensure `vulnerabilities/unresolved/` exists.
- Filename: `vulnerabilities/unresolved/<YYYY-MM-DD-HHmmss>-<branch-slug>.md` — branch slug is the branch name with `/` replaced by `-`.
- Write the report even when there are **no findings** — a dated clean pass on a branch is itself a useful record. A clean report may be written directly to `vulnerabilities/resolved/` since there is nothing to resolve.
- After writing, tell the user the report path and a one-line summary (counts by severity).
- Do not commit the report or anything else — committing is the user's decision. The folders are intentionally **not** gitignored, so reports can be committed with the code they describe.

### Report template

```markdown
# Security Review — full application

- **Run:** <yyyy-MM-dd HH:mm:ss>
- **Branch:** <branch name>
- **Commit:** <short hash of HEAD>
- **Reviewed:** entire source tree (<N> files)
- **Result:** <N> finding(s) — <c> Critical, <h> High, <m> Medium, <l> Low
- **Sources:** OWASP Top 10 / ASVS (/ LLM Top 10 where applicable), Microsoft Secure Coding Guidance, CWE Top 25, CodeQL query coverage
- **Status:** UNRESOLVED

> Generated by `/code-security-review`. This skill never edits source code itself.
>
> **To the session asked to fix this report:** fix the findings below in the source code,
> run `dotnet build FatCat.Fakes.sln` and `dotnet test FatCat.Fakes.sln` to confirm green, and check
> off each finding in the summary checklist as you resolve it. When — and only when — every
> finding is fixed and verified, change **Status** above to `RESOLVED (<date>)` and move this
> file to `vulnerabilities/resolved/` (same filename). If any finding remains unfixed, the
> file stays in `vulnerabilities/unresolved/` with the checklist showing what is left.

---

## 1. <short title> — <Severity>
- **File:** <relative/path/to/File.cs>, lines <range>
- **Classification:** CWE-<id> — <name>; OWASP <category>
- **Problem:** <what an attacker can do and why this code allows it>
- **Suggested fix:** <the concrete change>

## 2. <next finding>
...

---

## Observations (not vulnerabilities)
- <hardening suggestions or notes that are not exploitable findings — omit section if none>

## Summary checklist
- [ ] 1. <File.cs> — <one-line of what to fix> (<Severity>)
```

## Resolution workflow (for the fixing session — not this skill)

When the user says "fix this security report", "run a fix on <report>", or points a session at a file in `vulnerabilities/unresolved/`:

1. Fix each finding in the source code, following the repo's coding standards (TDD included — security fixes get tests like any other production change).
2. Run `dotnet build FatCat.Fakes.sln` and `dotnet test FatCat.Fakes.sln`; everything must be green.
3. Check off each resolved finding in the report's summary checklist.
4. **All findings fixed:** update the report's **Status** line to `RESOLVED (<yyyy-MM-dd>)` and move the file to `vulnerabilities/resolved/`, keeping the same filename (use `git mv` if the report is tracked).
5. **Some findings remain:** leave the file in `vulnerabilities/unresolved/` with the checklist reflecting what is done and what is left. Never move a partially resolved report.

This move is the **only** sanctioned way a report leaves `unresolved/` — the review skill itself never moves, renames, or deletes reports.

## Hard rules for this skill
- **Always review the entire source tree** — never narrow to a diff, commit, project, or directory, even if asked; the full application is the point.
- **Never edit source files.** Review and report only; fixing is a separate, explicit action.
- **Never move or delete reports.** New reports go to `unresolved/`; moving to `resolved/` belongs exclusively to the session that fixed the findings.
- **Do not commit anything.**
- Every finding must trace to OWASP, Microsoft Secure Coding Guidance, the CWE Top 25, or a CodeQL query class. Anything else goes under Observations.
- The report itself must obey the privacy rule: reference files, lines, and identifiers — never reproduce user-submitted content, identity images or biometric data, phone numbers, secrets values (name the key, redact the value), or anything that identifies a person.
