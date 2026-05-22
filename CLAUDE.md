# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project status

VisiLog is an early prototype log viewer. The solution skeleton is in place, but most projects contain only stub `Loader.cs` files and a single `LogMessage` model — there is no working feature surface yet, no tests, and no database.

## Solution location and build

The solution uses the new `.slnx` XML format:

- Solution: [App/VisiLog/VisiLog.slnx](App/VisiLog/VisiLog.slnx)
- Target framework: **net10.0** across all projects
- Build: `dotnet build App/VisiLog/VisiLog.slnx`
- Run the Blazor host (entry point for the full stack): `dotnet run --project App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM`

There is no test project yet. Do not invent test commands.

## Architecture: numbered layers

The `.slnx` organizes projects into numbered solution folders that reflect the architectural layers and dependency direction (lower numbers depend on higher numbers):

```
1-Global       VisiLog.Model.App                 shared POCOs (e.g., LogMessage)
2-UI           VisiLog.Web.Blazor.WASM           Blazor server host
               VisiLog.Web.Blazor.WASM.Client    Blazor WebAssembly client
3-Abstraction  VisiLog.Abstraction.Contracts     interfaces for client-side HTTP wrappers
               VisiLog.Abstraction.Service.WebAPI  implementations of the above
4-Service      VisiLog.Model.Service.WebAPI      request/response DTOs
               VisiLog.Service.WebAPI            server-side WebAPI surface
5-Logic        VisiLog.Logic.Contracts           business logic interfaces
               VisiLog.Logic                     business logic implementations
6-Data         VisiLog.Data.Contracts            repository interfaces
               VisiLog.Data.SQL                  SQL-backed repository implementations
               VisiLog.Model.Data.SQL            SQL persistence models
```

Project naming convention is load-bearing — preserve it when adding new projects:

- `*.Contracts` → interfaces only, no implementations
- `*.Model.*` → POCOs / DTOs only, no behavior
- `*.Abstraction.*` → client-side abstractions over remote services
- A bare layer name (e.g., `VisiLog.Logic`, `VisiLog.Data.SQL`) → the concrete implementation of that layer's contracts

## The DI Loader pattern

Every non-model project has a `Loader.cs` that derives from `CodeFactory.NDF.DependencyInjectionLoader` (NuGet: `CodeFactory.NDF`). This is the project's mandatory DI registration entry point.

`DependencyInjectionLoader.Load(...)` orchestrates three overridable phases per library:

1. `LoadLibraries` — instantiate the `Loader` of each child library this library depends on and call its `.Load(...)`. Order is **leaf-to-trunk** (deepest dependency first).
2. `LoadManualRegistration` — hand-written `services.AddX<Y>()` registrations.
3. `LoadRegistration` — convention-based / generated registrations.

Application entry points (`Program.cs`) call the **trunk** loader for their composition root and let it cascade:

- [App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM/Program.cs](App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM/Program.cs) → `new Loader().Load(...)` → loads Data.SQL, Logic, Service.WebAPI
- [App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM.Client/Program.cs](App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM.Client/Program.cs) → `new Loader().Load(...)` → loads Abstraction.Service.WebAPI

**When adding a new project**: create its `Loader.cs` mirroring [App/VisiLog/VisiLog.Logic/Loader.cs](App/VisiLog/VisiLog.Logic/Loader.cs), then register it inside the appropriate parent loader's `LoadLibraries` — do **not** add registrations directly to `Program.cs`. The current parent loaders to extend are [VisiLog.Web.Blazor.WASM/Loader.cs](App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM/Loader.cs) (server) and [VisiLog.Web.Blazor.WASM.Client/Loader.cs](App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM.Client/Loader.cs) (WASM client).

## Blazor hosting model

The Blazor app uses the **Interactive WebAssembly** render mode applied **globally** — `<Routes @rendermode="InteractiveWebAssembly" />` in [App.razor](App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM/Components/App.razor) — so every routed component runs in the browser. This drives a strict project split:

- `VisiLog.Web.Blazor.WASM` (host) — only the HTML host shell (`App.razor`), the `Program.cs` entry point, server-only services, and references to the server-side layers (Logic, Service.WebAPI, Data.SQL). It does **not** contain `Routes.razor`, layouts, or pages.
- `VisiLog.Web.Blazor.WASM.Client` (WASM) — `Routes.razor`, all layouts (`Layout/`), all pages (`Pages/`), and any component that ends up in the rendered route tree. References only the Abstraction layer (must not pull server-only assemblies, or those assemblies would need to ship to the browser).

**Why `Routes.razor` must live in the Client project**: `App.razor` (server-side) renders `<Routes @rendermode="InteractiveWebAssembly" />`. The WASM runtime then needs to instantiate the `Routes` type in the browser, which requires its assembly to be downloaded — only the `.Client` assembly is. Putting `Routes.razor` (or any routed component) in the host project produces a runtime error: *"Root component type ... could not be found in the assembly 'VisiLog.Web.Blazor.WASM'."*

**When adding a routed page or layout component, always create it under the `.Client` project**, never under the host. If a page needs server-side data, fetch it through an `Abstraction.Service.WebAPI` client (which calls the WebAPI), not via a direct service reference.

## UI component library

**MudBlazor is the standard UI component library for this project.** When building or modifying UI:

- Prefer `Mud*` components (`MudButton`, `MudTextField`, `MudDataGrid`, `MudDialog`, etc.) over raw HTML or hand-rolled components.
- Do not introduce other Blazor component libraries (Radzen, Telerik, MudBlazor.Extensions forks, plain Bootstrap component wrappers, etc.) without explicit user agreement.
- MudBlazor service registration (`AddMudServices`) is wired into the `LoadManualRegistration` phase of both Loaders — the [Client Loader](App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM.Client/Loader.cs) (for WASM runtime) and the [host Loader](App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM/Loader.cs) (for prerendering). Do not duplicate registrations in `Program.cs`.
- The four provider components (`MudThemeProvider`, `MudPopoverProvider`, `MudDialogProvider`, `MudSnackbarProvider`) live once at the top of [Client/Layout/MainLayout.razor](App/VisiLog/VisiLog.Web.Blazor.WASM/VisiLog.Web.Blazor.WASM.Client/Layout/MainLayout.razor). The default chrome uses `MudLayout` + `MudAppBar` + `MudDrawer` + `MudMainContent` — extend that layout rather than adding parallel layouts.
- Bootstrap has been removed; do not reintroduce `btn`, `navbar`, `text-danger`, etc. or re-add the `bootstrap` static asset folder.

## Data access

**The repository pattern with Dapper is the standard for all data access in this project.** When adding or modifying anything that touches SQL:

- Repository **interfaces** live in `VisiLog.Data.Contracts` (e.g., `ILogMessageRepository`). They expose async, domain-typed methods that work with POCOs from `VisiLog.Model.App` — never SQL-specific types.
- Repository **implementations** live in `VisiLog.Data.SQL` and use Dapper (`Query<T>`, `Execute`, `QueryFirstOrDefault<T>`, etc.) over an injected `IDbConnection` or a connection factory. No raw `SqlCommand`/`SqlDataReader`, no string-built SQL outside parameterized Dapper calls.
- SQL-shape DTOs (rows that don't map cleanly to a `Model.App` type — denormalized join results, flattened projections, etc.) live in `VisiLog.Model.Data.SQL`. The repository implementation translates them to `Model.App` types before returning.
- Do **not** introduce Entity Framework Core, NHibernate, Dapper.Contrib, Dapper-FluentMap, or any other ORM/micro-ORM. If a need arises that Dapper can't cleanly cover, raise it explicitly before adding tooling.
- Repository bindings (`services.AddScoped<ILogMessageRepository, LogMessageRepository>()`) and the connection factory registration belong in the `LoadManualRegistration` phase of [VisiLog.Data.SQL/Loader.cs](App/VisiLog/VisiLog.Data.SQL/Loader.cs), not in `Program.cs`.
- All SQL must be parameterized (Dapper's `@param` syntax). Never interpolate or concatenate user input into a query.
- Repository methods follow a strict try/catch shape — see [LogMessageRepository.cs](App/VisiLog/VisiLog.Data.SQL/Repositories/LogMessageRepository.cs) as the canonical example. Each method:
    1. Declares a typed `result` local initialized to a safe default at the top of the method (`new List<T>()`, `null`, `0`, etc.).
    2. Inside a `try` block: opens the connection via `_connectionFactory.Create()`, builds a `CommandDefinition` (always passing the `CancellationToken`), runs the Dapper call, and assigns the outcome to `result`.
    3. `catch (SqlException ex)` → `ex.ThrowManagedException();` (from `CodeFactory.NDF.SQL`) — translates SQL error numbers (deadlock, duplicate key, FK violation, timeout, auth, etc.) into the typed NDF exception hierarchy. Do **not** add `throw;` after; the method's terminal `return result;` is what satisfies the compiler's definite-assignment / flow analysis.
    4. `catch (Exception)` → `throw new UnhandledException();` (from `CodeFactory.NDF`) — shields callers in the Logic layer from infrastructure-specific exception types.
    5. Returns `result` once, at the bottom of the method, outside the try/catch.

    Public methods on both the interface and the implementation carry XML doc comments describing parameters and return values.
- Connection strings come from `IConfiguration.GetConnectionString("VisiLog")` via [SqlConnectionFactory](App/VisiLog/VisiLog.Data.SQL/Connections/SqlConnectionFactory.cs). The connection string name `"VisiLog"` is a constant on that class — reuse it, don't hardcode the literal.
