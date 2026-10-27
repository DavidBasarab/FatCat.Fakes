# PowerShell Standards

## Scope
These standards apply to all PowerShell scripts in this codebase. PowerShell is used for:
- Build automation
- Local environment configuration and orchestration
- Application API interaction and deployment automation (deploy scripts run the release gates and refuse to deploy on red)

---

## Naming

- All functions use `Verb-Noun` format with an approved PowerShell verb from `Get-Verb`.
- PascalCase for: function names, parameters, hashtable keys.
- camelCase for local variables inside functions.
- Full words only — no abbreviations unless they meet the top-3 Google rule (same as C#).
- No aliases in code. Always use full cmdlet names (`Get-ChildItem` not `gci`, `Where-Object` not `where`).
- Users may define their own aliases in their profile — do not define aliases for them in scripts.

---

## Files

- One function per file. The file is named after the function: `Deploy-FatCat.FakesApi.ps1` contains only `Deploy-FatCat.FakesApi`.
- Exception: helper functions defined inside another function's scope (nested functions) may live in the same file as their parent.
- Do not define multiple top-level functions in a single file.

---

## Function Structure

- Every function has a `param()` block at the top.
- Parameters are typed explicitly.
- Use `[switch]` for boolean flags — never a `[bool]` parameter with `$true/$false`.
- Use `[Parameter(Mandatory = $true)]` only when the parameter is genuinely required to function.
- Do not use `#Requires` statements.

---

## Shell

- Always PowerShell 7 (`pwsh`). Never `powershell.exe` (Windows PowerShell 5).
- Load the profile when invoking `pwsh` for an ad-hoc command: `pwsh -Command '. $PROFILE; Your-Command'`.

---

## Testing

- PowerShell does not require tests. Do not write Pester tests or any other test framework for PowerShell scripts.
- Testing is required for C# and TypeScript only.
