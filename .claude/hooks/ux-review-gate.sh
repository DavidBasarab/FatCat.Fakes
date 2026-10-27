#!/usr/bin/env bash
# UX review gate — Stop hook for the project's Blazor WASM site(s).
#
# Blocks a session from ending while changed .razor / .razor.cs / .css files
# have not been reviewed by the ux-reviewer agent AT THEIR CURRENT CONTENT.
# One holistic review when the work is done — no per-edit nagging.
#
# Files are matched by extension anywhere in the repo; build output (bin/, obj/),
# node_modules/, and test projects are excluded.
#
# Usage:
#   ux-review-gate.sh          run the gate (invoked as the Stop hook)
#   ux-review-gate.sh --mark   record that the current UX changes were reviewed
#
# Resolving a block: run the ux-reviewer agent. It ends by calling this script
# with --mark, which stamps the current file set as reviewed. A manual override
# file .claude/.cache/ux-review.skip (any content) bypasses the gate entirely.

set -euo pipefail

project_dir="${CLAUDE_PROJECT_DIR:-$(git rev-parse --show-toplevel 2>/dev/null || pwd)}"
cache_dir="$project_dir/.claude/.cache"
marker="$cache_dir/ux-review.stamp"
skip="$cache_dir/ux-review.skip"

ux_file_pattern='(\.razor$|\.razor\.cs$|\.css$)'
excluded_path_pattern='(^|/)(bin|obj|node_modules)/|(^|/)(Tests?\.[^/]*|[^/]*\.Tests?|tests?)/'

changed_ux_files() {
	{
		git -C "$project_dir" diff --name-only
		git -C "$project_dir" diff --name-only --cached
		git -C "$project_dir" ls-files --others --exclude-standard
	} 2>/dev/null | sort -u | grep -E "$ux_file_pattern" | grep -vE "$excluded_path_pattern" || true
}

compute_stamp() {
	local files="$1"
	while IFS= read -r f; do
		[ -z "$f" ] && continue
		printf '%s:%s\n' "$f" "$(git -C "$project_dir" hash-object "$project_dir/$f" 2>/dev/null || echo missing)"
	done <<<"$files" | git hash-object --stdin
}

files="$(changed_ux_files)"

if [ "${1:-}" = "--mark" ]; then
	mkdir -p "$cache_dir"
	if [ -z "$files" ]; then
		: >"$marker"
	else
		compute_stamp "$files" >"$marker"
	fi
	echo "UX review recorded for the current changed Blazor UI files."
	exit 0
fi

# --- Gate mode (Stop hook) ---
[ -f "$skip" ] && exit 0
[ -z "$files" ] && exit 0

stamp="$(compute_stamp "$files")"
if [ -f "$marker" ] && [ "$(cat "$marker")" = "$stamp" ]; then
	exit 0
fi

count="$(printf '%s\n' "$files" | grep -c . || true)"
{
	echo "UX review gate: $count changed Blazor UI file(s) (.razor / .razor.cs / .css) have not been UX-reviewed at their current content."
	echo ""
	echo "Before finishing, run the ux-reviewer agent over the changed files:"
	echo "  Launch the Agent tool with subagent_type \"ux-reviewer\" and ask it to review the current git diff."
	echo ""
	echo "The agent records completion automatically (it calls ux-review-gate.sh --mark on the way out)."
	echo "To bypass once, create .claude/.cache/ux-review.skip."
	echo ""
	echo "Changed UI files:"
	printf '%s\n' "$files" | sed 's/^/  - /'
} >&2
exit 2
