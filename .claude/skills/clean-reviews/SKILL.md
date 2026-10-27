---
name: clean-reviews
description: Delete standards-review report markdown files from the .reviews/ folder. Use ONLY when the user explicitly asks to clean up, clear, remove, or delete review reports. This is the only skill permitted to delete files in .reviews/. Never invoke it as a side effect of reviewing or resolving a report.
---

# Clean Reviews

Remove generated review reports from `.reviews/` (at the repo root). This is the **only** skill that deletes report files — `/standards-review` never does.

## Steps

1. **List what is there.** Show the current report files: `pwsh -Command '. $PROFILE; Get-ChildItem -Path .reviews -Filter *.md -ErrorAction SilentlyContinue | Select-Object Name, LastWriteTime'`. If `.reviews/` does not exist or is empty, say there is nothing to clean and stop.

2. **Determine which to delete from the user's phrasing:**
   - "clean up reviews" / "clear the reviews" / "delete all reviews" → all `*.md` files in `.reviews/`.
   - "delete the review for <project>" / "remove `<name>`" → only the matching file(s). If more than one matches, list them and confirm before deleting.
   - A specific filename → that file only.

3. **Confirm before deleting more than one file.** List the exact files you are about to remove and ask for confirmation, unless the user already said "all" / "everything" explicitly. Deleting is not reversible.

4. **Delete** the selected files: `pwsh -Command '. $PROFILE; Remove-Item -Path .reviews/<file>.md -Confirm:$false'`. Only ever delete `*.md` files inside `.reviews/` — never touch anything outside that folder.

5. **Report** which files were removed and how many remain.

## Hard rules
- Only delete files under `.reviews/`, and only `*.md` files.
- Never delete source code, `.gitignore`, or anything outside `.reviews/`.
- This skill does not review code and does not create reports — it only removes them.
