# Error Handling & Logging

## Error Handling
- Exceptions are for unplanned, unexpected failures only (hardware failures, network timeouts, corrupted state).
- Never throw an exception for a predictable outcome (validation failure, value out of range, known bad state).
- For known failure modes, return a value — an enum is preferred.
- Let exceptions bubble to the boundary where they can be meaningfully handled.
- Do not catch and swallow exceptions silently. The one exception: if a failure is genuinely non-actionable (e.g. a socket error on disconnect), an empty catch with a `// ignored` comment is acceptable. This must be rare and deliberate — never use it to hide logic errors.
- "Log and rethrow at the boundary" is allowed at endpoint or top-level entry points. Do not log-and-rethrow at every layer — pick one boundary.

```csharp
// Preferred for known failures:
public enum LoginResult { Success, UnknownUser, InvalidPassword }

public LoginResult TryLogin(LoginRequest request)
{
    if (!userExists)        return LoginResult.UnknownUser;
    if (!passwordMatches)   return LoginResult.InvalidPassword;

    SignIn();

    return LoginResult.Success;
}
```

## Logging — Serilog
- We use Serilog. Inject `Serilog.ILogger` via the constructor for permanent logging. Alias it as `using ILogger = Serilog.ILogger;` to disambiguate from `Microsoft.Extensions.Logging.ILogger` — never inject the Microsoft `ILogger`.
- Log at the action site, not at the boundary.
- Log thoughtfully — do not add log entries without a clear reason.
- Active log levels: `Debug`, `Information`, `Warning`, `Error`.
- Do not leave temporary tracing behind. If you add a trace while diagnosing, remove it before merging.

## Logging and TDD
- Logging is the one area where strict TDD is not enforced.
- Do not block on log string test coverage — test critical entries, use judgment for the rest.
