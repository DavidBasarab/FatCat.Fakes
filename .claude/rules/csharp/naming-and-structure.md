# Naming & Structure

## Core Philosophy
- Follow Clean Code principles (Robert C. Martin) and SOLID.
- Methods do one thing. Classes have one responsibility.
- Code reads like prose. Names make intent obvious without reading the implementation.
- Prefer interfaces and polymorphism over if/switch chains.
- Do NOT over-engineer. Do NOT introduce abstractions that do not already exist in this codebase.
- Match the abstraction level and style of the surrounding code.

## Naming Rules
- Avoid abbreviations. Prefer full words so readers never have to guess meaning.
- Acceptable abbreviations: widely recognized acronyms (e.g. `HTTP`, `URL`, `ID`, `LLM`) and any abbreviation that appears among the top 3 Google results for that term. When in doubt, use the full word.
- Names reveal intent. A method name makes it unnecessary to read the body.
- No comments explaining what code does — rename until it is obvious.
- PascalCase: classes, interfaces, methods, properties, constants
- camelCase: local variables, parameters, private fields — no leading underscore
- Private fields prefer `readonly` for dependencies where applicable
- Boolean names read as questions or states: `isReady`, `hasOutputs`, `canRestore`
- String interpolation required — never string concatenation with `+`
- Do NOT suffix method names with `Async` just because they return a `Task`. Name the method after what it does: `Save`, not `SaveAsync`. Only use the `Async` suffix when a non-async overload with the same name already exists and both must coexist.

## Discards
- Use `_` to discard outputs you intentionally do not need — `out _` for ignored out parameters, `using var _ = ...` for disposables acquired only for their side effect.

## Method Size
- Methods should be as short as possible.
- ~10 lines is a signal to evaluate refactoring — not an automatic rule.
- No method should require a comment to explain what it does. Refactor or rename instead.

## Spacing
- Leave a blank line between method definitions.
- Leave a blank line after variable declarations in a method before logic begins.
- Leave a blank line before return statements.

## Control Flow
- Avoid deep if/else nesting. Prefer guard clauses and early returns to keep the main flow readable.
- Avoid complex nested ternary expressions — prefer clear `if` statements or extract into a well-named method.
- If you need to explain what code does with a comment, first ask whether a better name makes the comment unnecessary.
- Use switch expressions (not if/else chains) when branching on an enum or type. Always include a discard arm `_` that throws `ArgumentOutOfRangeException` for unhandled cases:

```csharp
// Correct — switch expression
var result = documentType switch
{
    DocumentType.Pdf => ParsePdf(document),
    DocumentType.PlainText => ParsePlainText(document),
    _ => throw new ArgumentOutOfRangeException(nameof(documentType)),
};

// Wrong — if/else chain
if (documentType == DocumentType.Pdf) result = ParsePdf(document);
else if (documentType == DocumentType.PlainText) result = ParsePlainText(document);
```

## Files & Namespaces
- One class per file. File named after the class, never the interface.
- When a class directly implements a single interface, the interface and class live in the same file — named after the class. Do not create a separate file for the interface.
- Only create a standalone interface file when the interface has multiple implementations or is consumed without a single obvious implementation.
- Namespace must exactly match the folder path within the project. No exceptions.
- All production namespaces start with `FatCat.Fakes.*` (e.g. `FatCat.Fakes.Api.Orders`, `FatCat.Fakes.Common.ServiceModels`, `FatCat.Fakes.Common.Infrastructure`).
- Test project mirrors source project: same folder structure, same namespace with `Tests.` prepended — `FatCat.Fakes.Api.Orders` → `Tests.FatCat.Fakes.Api.Orders`.
- Always use file-scoped namespaces (C# 10+). Never use block-style `namespace X { }`.

```csharp
// Correct — file-scoped
namespace FatCat.Fakes.Api.Orders;

public class CreateOrderEndpoint { }

// Wrong — block-scoped
namespace FatCat.Fakes.Api.Orders
{
    public class CreateOrderEndpoint { }
}
```

## Endpoint Pattern

Endpoints inherit from `Endpoint` (FatCat toolkit) and return `WebResult`.

1. **Return `WebResult`.** All endpoint action methods return `WebResult` or `Task<WebResult>`. Never return raw ASP.NET Core types (`IActionResult`, `Ok<T>()`, etc.). Use the inherited helpers (`Ok(...)`, `NotFound()`, etc.) to build the result.

2. **Route via attribute.** Annotate each action with `[HttpGet]` / `[HttpPost]` / `[HttpPut]` / `[HttpDelete]` and an explicit `"api/..."` route (e.g. `[HttpPost("api/Order")]`).

3. **One endpoint per file.** Endpoint-per-file is the rule — a file holds one endpoint class, named after what it does.

4. **Interface only when reused.** An endpoint does not need an interface by default. Only add one when another part of the codebase needs to call the endpoint's logic directly (e.g. one service calling into another). When an interface is needed, define it in the same file immediately above the class:

```csharp
// Only add this when something else needs to call the endpoint's logic directly
public interface IChangeOrderStatus
{
    Task<WebResult> Change(ChangeOrderStatusRequest request);
}

public class ChangeOrderStatusEndpoint(IMongoRepository<OrderData> repository)
    : Endpoint, IChangeOrderStatus
{
    ...
}
```

If the endpoint is only ever called via HTTP and nothing injects `IChangeOrderStatus`, no interface is needed.

5. **Mutable state fields for request context.** When an endpoint breaks its logic into multiple private helper methods, it may use non-`readonly` private fields to share working state across those methods within a single request. These fields are intentionally mutable and are not injected — they are populated during the request. Use `null!` to suppress the nullable warning, since the field is always populated before it is read:

```csharp
public class CreateOrderEndpoint(
    IMongoRepository<OrderData> repository,
    IProjector projector,
    IDateTimeUtilities dateTimeUtilities
) : Endpoint
{
    private OrderData order = null!;   // request working state — intentionally NOT readonly

    [HttpPost("api/Order")]
    public async Task<WebResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        order = new OrderData { ... };
        order = await repository.Create(order);

        var serviceModel = projector.ProjectTo<OrderServiceModel>(order);
        return Ok(serviceModel);
    }
}
```

This pattern avoids passing many parameters between helper methods. It is only valid within an endpoint class where the lifetime of the object is a single HTTP request.

## POCO Suffix Conventions
The codebase uses a strict vocabulary of type-role suffixes. Always pick the existing suffix for the role — do not invent new ones.

| Suffix | Role | Base type |
|---|---|---|
| `*Data` | Persisted Mongo entity | `MongoObject` |
| `*ServiceModel` | Wire/DTO returned from the API | `ServiceModel : EqualObject` |
| `*Request` | Inbound API body | `ServiceModel` or `ServiceRequest : EqualObject` |
| `*Response` | Outbound result object (typically nested in a `WebResult`) | `EqualObject` |
| `*CacheItem` | Item stored in an `IFatCatCache<T>` | `EqualObject, ICacheItem` (with `CacheId`) |
| `*Endpoint` | Web endpoint (route handler) | `Endpoint` |
| `*Worker` | Scheduled background worker | `IWorker` |
| `*Projector` | Custom `IProjector` projection | `IDoProjection<TDestination>` |
| `*MessageProcessor` | Inbound `SignalMessage` handler | `ISignalMessageProcessor` (or similar) |
| `*Repository` | Persistence abstraction | `IMongoRepository<T>` (FatCat) or custom |

Examples: `OrderData`, `OrderServiceModel`, `CreateOrderRequest`, `OrderConnectionCacheItem`, `ExpireOrderWorker`, `OrderProjector`.

## Project Boundaries
Put new code in the project that owns it:

| Project | Owns |
|---|---|
| The API project | Endpoints, SignalR hubs, business pipelines, authorization, MongoDB access. The application. |
| The Common project | Contracts referenced by BOTH the API and the site: request/response models, signal-message types and enums, the typed API client. No behavior beyond serialization and transport. |
| The Blazor site project | Presentation only: displays information, collects input, calls the typed client. No business logic, no auth logic beyond acquiring and sending a token. |

The site never declares its own request/response shapes and never hand-rolls a URL. One definition per contract, in the Common project, shared by both sides.

## Folder Conventions Within a Project
Each domain area inside a service project follows a consistent layout:
- `<Feature>/` — root folder for the feature
- `<Feature>/Endpoints/` — `*Endpoint` classes
- `<Feature>/Datas/` (or `<Feature>/` directly when there is one type) — `*Data` Mongo entities, often with a `Datas/Projectors/` sub-folder for `*Projector` classes that produce derived `*Data` types
- `<Feature>/ServiceModelProjectors/` — `*Projector` classes that produce `*ServiceModel` types
- `<Feature>/Helpers/` — small feature-specific helpers
- `<Project>/Infrastructure/` — Autofac module, custom-projector registration, fakes, app-startup hooks
- `<Common project>/ServiceModels/` — shared `*ServiceModel` types and `Requests/` / `Response/` sub-folders

Match the existing layout. Do not place a new endpoint, projector, or data class in an arbitrary location.

## Translation-Keyed Error Strings
User-facing error strings returned from `BadRequest(...)` and similar helpers are kebab-case translation keys, not English sentences. They are declared as `public const string` on the central `FatCat.Fakes.Common.Errors` class and referenced by name:

```csharp
return BadRequest(Errors.OrderCannotBeArchived);   // value: "error-order-cannot-be-archived"
```

- Add new error keys to the Common project's `Errors.cs` — never inline a raw kebab-case string at the call site.
- Keys always start with `error-` and are kebab-case.
- The client side translates the key into a displayed message; the server does not.

## Interfaces
- All interfaces use the `I` prefix.
- Interface names describe a capability or action: `IOrderManager`, `ISendNotificationToUser`, `IGetAllWordsList`, `IVerifyLicense`.
- NOT: `IOrder`, `INotification`, `ILicenseService` — these describe what something is, not what it does.
- Default to narrow, single-purpose interfaces. One interface = one capability.
- Exception: highly cohesive groups (e.g. all REST calls to the same API resource, or a manager exposing a small related set of operations) may be grouped.
- All cross-boundary dependencies must be interfaces: LLMs and embedding models, object storage, threading, file system, time, external processes, REST clients, the Mongo driver, etc.
- If something cannot be faked in a test, it is not properly abstracted.
