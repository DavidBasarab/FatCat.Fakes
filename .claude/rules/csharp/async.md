# Async / Await

- Async all the way up. If anything in a call chain is async, the whole chain is async.
- Never block on a Task with `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`.
- `async void` is banned. All async methods return `Task` or `Task<T>`.
- Do NOT use `ConfigureAwait(false)`. We do not use it in this codebase.
- Threading and sleep operations use `IThread` — see types-and-di.md.
- Streaming work (progress updates, incremental results) pushes through the Hub as it happens — do not buffer a whole result and send it at the end when the contract says it streams.

## Blocking calls — very limited exceptions only
Blocking on a Task is only acceptable in these specific scenarios:
- Top-level synchronous entry points where converting to async is genuinely infeasible
- Interfacing with a third-party API that exposes only synchronous entry points and cannot be changed
- Properties (which cannot be awaited) — isolate and keep minimal

When a blocking call is unavoidable: isolate it, document in a comment why async was not possible, and consider `Task.Run` with caution.
