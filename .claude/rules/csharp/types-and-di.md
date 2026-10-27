# Types & Dependency Injection

## var
- Use `var` as the default for local variable declarations.
- Small methods and good naming make the type obvious from context.
- Use explicit types only when the type is not clear from the right-hand side.

## Nullable Reference Types
- Nullable reference types are disabled in every project (`<Nullable>disable</Nullable>`).
- Use nullable annotations (`?`) only where a value is genuinely optional or nullable — for example, generic return types (`T?`), reflection-heavy code, or extension methods that accept null input by design.
- Do not annotate defensively. If a value cannot be null in normal usage, do not mark it nullable.
- Do not write `string?`, `ILogger?`, or other nullable annotations on injected dependencies or values that are always populated.

## Collection Initialization
- Use collection expressions (`[]`) to initialize collections. Do not use `new List<T>()`, `new T[0]`, or `new Dictionary<K, V>()` when an empty or inline-populated collection is needed.
- The target type drives the actual collection — `List<OrderLine> Lines { get; set; } = [];` produces an empty list, just like `new List<OrderLine>()`, but is shorter and consistent.

```csharp
// Correct — collection expressions
public List<OrderLine> Lines { get; set; } = [];
public string[] Names { get; set; } = [];
var pending = [firstTask, secondTask, thirdTask];

// Wrong — explicit constructor calls
public List<OrderLine> Lines { get; set; } = new List<OrderLine>();
public string[] Names { get; set; } = new string[0];
var pending = new List<Task> { firstTask, secondTask, thirdTask };
```

This applies to property initializers, field initializers, local variables, and method arguments. The exception is when you need a specific concrete type that the target cannot infer (e.g. assigning to `IEnumerable<T>` and needing a `HashSet<T>` specifically) — in that case, name the type explicitly.

## Thread-Safe Collections
- Use `ConcurrentDictionary<TKey, TValue>` for shared mutable state that is accessed across threads.
- Never use a plain `Dictionary` with manual locking for this purpose.

## Lazy Initialization
- The default lazy pattern uses the C# `field` keyword with null-coalescing assignment in a property getter. This is preferred over `Lazy<T>` for ordinary deferred initialization:

```csharp
public IReadOnlyList<string> AllWords
{
    get { return field ??= LoadWords(); }
}
```

- Use `Lazy<T>` only when you genuinely need its thread-safety guarantees (e.g. a value that may be initialized concurrently and must run the factory exactly once). When you do, use the factory constructor overload: `new Lazy<T>(() => ...)`.

## Records — BANNED
- Records are banned. Use classes only.

## Access Modifiers
- Public is the default. Do not add access modifiers to restrict visibility unless there is a specific reason.
- `dotnet format` (via `.editorconfig`) enforces readonly and auto-properties — follow its guidance.

## Constructor Injection Only
- All dependencies are injected via the constructor. No property injection. No setter injection.
- Use primary constructors (C# 12+) as the standard form for all new code. Do not write explicit constructor bodies with `this.field = param` assignments.
- Never use `new` inside a class to instantiate a dependency — ask for it via the constructor.

```csharp
// Correct — primary constructor
public class OrderManager(
    IFatCatCache<OrderCacheItem> orderCache,
    IGenerator generator,
    IJsonOperations jsonOperations,
    IThread thread
) : IOrderManager
{
    // orderCache, generator, jsonOperations, thread are available directly
}

// Wrong — traditional explicit constructor
public class OrderManager : IOrderManager
{
    private readonly IFatCatCache<OrderCacheItem> orderCache;
    private readonly IThread thread;

    public OrderManager(IFatCatCache<OrderCacheItem> orderCache, IThread thread)
    {
        this.orderCache = orderCache;
        this.thread = thread;
    }
}
```

## Autofac Module Registration
All dependency registration uses Autofac `Module` classes. Each project that registers services has one `*Module : Module` class with a `Load(ContainerBuilder builder)` override.

### When to register in the module
Only add a registration to the module when there are **multiple implementations of the same interface** and you need to override the default. Autofac automatically resolves a single implementation of an interface — no module entry is required for one-to-one mappings.

Add to the module when:
- A project provides its own implementation of an interface that already has a default implementation elsewhere (e.g. `IGetToken` has a default in Common, but the API project registers its own to override)
- The type requires `.SingleInstance()` lifetime that cannot be inferred automatically
- The type requires a factory method for construction (`.Factory` pattern)
- The type is an open generic requiring `RegisterGeneric`
- The implementation is chosen from configuration at startup (e.g. a real vs a fake/local implementation)

Do NOT add to the module when:
- There is exactly one implementation of the interface in the container — Autofac resolves it automatically

### Rules
- Always register as the interface: `builder.RegisterType<MyClass>().As<IMyCapability>()`
- Add `.SingleInstance()` only when the type is genuinely stateless and safe to share across all requests
- Use `RegisterGeneric` for open generic types: `builder.RegisterGeneric(typeof(FluentProjection<,>)).As(typeof(IFluentProjection<,>))`
- Mark the module class `[ExcludeFromCodeCoverage]` (with a short `Justification`) and `[UsedImplicitly]` — it contains no testable logic and is invoked by reflection
- Do not register the concrete type without `.As<IInterface>()` unless there is an explicit reason (e.g. a fake response holder resolved by a factory method)
- For classes that require a factory method for construction, use a static `.Factory` method on the class and register it via `builder.Register(MyClass.Factory)`
- For configuration-driven swaps (real vs fake implementation chosen at startup), register a factory method that reads `IConfiguration` from the system scope

```csharp
// Common project — default implementation, resolved automatically (no module entry needed)
public class CommonGetToken : IGetToken { ... }

// API project — overrides the default; must be registered in the module
public class ApiToken : IGetToken { ... }

[UsedImplicitly]
[ExcludeFromCodeCoverage(Justification = "Infrastructure")]
public class ApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Overrides the Common default for IGetToken
        builder.RegisterType<ApiToken>().As<IGetToken>();

        // SingleInstance lifetime cannot be inferred
        builder.RegisterType<OrderSenderFactory>().As<IOrderSenderFactory>().SingleInstance();

        // Factory method construction / chosen from configuration
        builder.Register(SendNotificationToUser.Factory).As<ISendNotificationToUser>();
    }
}
```

## Object Mapping — FatCat.Projections (IProjector)
Object-to-object mapping uses `FatCat.Projections`, not AutoMapper. There is no `IConfigureMappings`, no profile setup, and no `CreateMap` calls.

Rules:
- Inject `IProjector` and call `projector.ProjectTo<TDestination>(source)` to map a single object
- The projector matches by property name and type — no configuration is needed for straightforward POCO-to-POCO mapping
- For mappings that need custom logic, write a class that implements `IDoProjection<TDestination>` and register it once per project in a `*CustomProjectors.Register()` static method via `ProjectionConfiguration.UseCustomProjection<TDestination, TProjector>()`. The `Register()` method is called from the project's startup hook.
- Place custom projector classes alongside the source type in a `Projectors/` or `ServiceModelProjectors/` sub-folder. Name them `<Destination>Projector` (e.g. `OrderProjector` produces `OrderServiceModel`).
- `IProjector` itself is registered once by the toolkit's projection module — do not register it again.

```csharp
public static class ApiCustomProjectors
{
    public static void Register()
    {
        ProjectionConfiguration.UseCustomProjection<UserServiceModel, UserProjector>();
        ProjectionConfiguration.UseCustomProjection<OrderServiceModel, OrderProjector>();
        // ...
    }
}
```

```csharp
public class CreateOrderEndpoint(IMongoRepository<OrderData> repository, IProjector projector) : Endpoint
{
    [HttpPost("api/Order")]
    public async Task<WebResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = projector.ProjectTo<OrderData>(request);

        order = await repository.Create(order);

        return Ok(projector.ProjectTo<OrderServiceModel>(order));
    }
}
```

Use `[UsedImplicitly]` on classes that are constructed by Autofac/reflection but have no compile-time references — this suppresses the unused-type warning.

## LINQ
- Use LINQ for querying and transforming collections. Prefer it over imperative loops.
- Always use method chaining syntax. Never use query syntax (`from x in y where...`).
- CSharpier handles formatting — write readable code and let it format.

## IThread — Threading Abstraction
- Threading and sleep operations use `IThread` (from `FatCat.Toolkit.Threading`). Never use `Task.Delay`, `Thread.Sleep`, or raw `Thread` directly.
- `IThread` is injected via constructor like all other dependencies.
- `FakeThread` (also from `FatCat.Toolkit.Threading`) provides a synchronous substitute for unit tests — see `testing.md`.

## IWorker — Scheduled Background Work
- Long-running scheduled work uses `FatCat.Worker.IWorker`. Create a class named `<Something>Worker` that implements `IWorker`, with a `TimeBetweenWorks` property and `DoWork(CancellationToken)` method.
- Register workers and start them through `IWorkerRunner` (from `FatCat.Worker`) in the project's startup hook.
- Inject `ISystemScope` and resolve per-iteration dependencies inside `DoWork` when each tick needs a fresh scope.

## IDateTimeUtilities — Time Abstraction
- Always read the current time via `IDateTimeUtilities.UtcNow()` injected into the class. Never call `DateTime.UtcNow` or `DateTime.Now` in production code.
- This applies to endpoints, services, message processors, workers, and projectors. The only acceptable use of `DateTime.UtcNow` is in test data builders and fake/stub generators.
- Injecting the abstraction is what makes `Faker`-generated dates and time-sensitive assertions deterministic.

## TimeSpan — Fluent Durations
- Express durations with fluent extensions, not `TimeSpan.From*` factory calls. Write `500.Milliseconds()`, `3.Seconds()`, `10.Minutes()`, `1.Days()` — never `TimeSpan.FromMilliseconds(500)`, `TimeSpan.FromSeconds(3)`, etc.
- Humanizer provides these in production code (add `using Humanizer;` where needed); `FatCat.Testing` ships the same `Hours()` / `Minutes()` / `Seconds()` / `Days()` / `Milliseconds()` extensions for tests. Fluent durations read as prose and keep duration literals consistent across the codebase.
- This applies everywhere a `TimeSpan` is constructed from a constant: worker intervals (`TimeBetweenWorks`), cache expirations, `IThread.Sleep`, and `BeCloseTo` tolerances in tests.

```csharp
// Correct — fluent durations
public TimeSpan TimeBetweenWorks { get { return 250.Milliseconds(); } }
await thread.Sleep(3.Seconds());
cache.Add(item, 15.Minutes());
createdDate.Should().BeCloseTo(currentDate, 3.Seconds());

// Wrong — TimeSpan factory calls
public TimeSpan TimeBetweenWorks { get { return TimeSpan.FromMilliseconds(250); } }
await thread.Sleep(TimeSpan.FromSeconds(3));
cache.Add(item, TimeSpan.FromMinutes(15));
createdDate.Should().BeCloseTo(currentDate, TimeSpan.FromSeconds(3));
```

## ISystemScope — Late Resolution
- `ISystemScope` (FatCat) is used only when a class genuinely must resolve a type at runtime — for example, a worker that wants a fresh scope per tick, a dispatcher that picks an `ISignalMessageProcessor` based on the inbound message type, or a factory method that picks between real and fake implementations from configuration.
- Do NOT use `ISystemScope` as a service locator to dodge constructor injection. If a class always needs the same dependency, inject it directly.

## Production GlobalUsings
Each production project may have a single `GlobalUsings.cs` at the project root that declares `global using` directives for namespaces used throughout the project. Keep these short and project-wide — they are part of the public surface of the project, not a dumping ground.

```csharp
// <API project>/GlobalUsings.cs
global using System.Text.Json.Serialization;
global using FatCat.Projections;
global using FatCat.Toolkit;
global using FatCat.Toolkit.Data.Mongo;
global using FatCat.Toolkit.WebServer;
global using FatCat.Fakes.Common.ServiceModels;
global using Microsoft.AspNetCore.Mvc;
```

## C# 14 Features
- The `field` keyword is accepted in property getters for backing-field initialization (`field ??= ...`) and in computed properties that need to cache.
- Extension blocks (`extension(TargetType target) { ... }`) are accepted for grouping multiple extension methods on the same type.

## FatCat Ecosystem
FatCat.Fakes depends on the FatCat toolkit for several core capabilities. Use these instead of rolling your own or pulling in equivalents:
- `IProjector` / `IFluentProjection<,>` (`FatCat.Projections`) — object mapping; replaces AutoMapper
- `IFatCatCache<T>` (`FatCat.Toolkit.Caching`) — typed in-memory caches
- `IJsonOperations` (`FatCat.Toolkit.Json`) — JSON serialisation
- `IThread` / `FakeThread` (`FatCat.Toolkit.Threading`) — threading abstraction
- `IGenerator` (`FatCat.Toolkit`) — id and value generation
- `IMongoRepository<T>` — Mongo persistence
- `IWorker` / `IWorkerRunner` (`FatCat.Worker`) — scheduled background work
- `Faker.Create<T>()` (`FatCat.Fakes`) — random test data generation
- `.Should()` assertions (`FatCat.Testing`) — assertions in test projects

The Common project references `FatCat.Toolkit` directly, so contracts can use the toolkit's base types (`EqualObject`, `ServiceModel`, signal-message types). It also ships into the Blazor WebAssembly payload — keep what Common actually *uses* to contracts, serialization, and transport. Server-only work (Mongo access, workers, caching, business pipelines) belongs in the API project.
