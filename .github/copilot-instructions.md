# CarStore - Copilot repository instructions

## Solution layout
- .NET 10 multi-project solution under `Src/`.
  - `CarStore.Domain` - entities, `CarStoreContext` (EF Core In-Memory), seed data.
  - `CarStore.Application` - application services (orchestration only).
  - `CarStore.Web` - ASP.NET Core 10 MVC (controllers, Razor views) whose UI is rendered by a Vue 3 + PrimeVue + Tailwind CSS v4 single-page app built from `ClientApp/`. Bootstrap and jQuery have been removed.
- No authentication.

## Front-end
- See `.github/instructions/frontend.instructions.md` (ClientApp: Vue/PrimeVue/Tailwind conventions) and `.github/instructions/web.instructions.md` (how MVC serves the Vue bundle and the `/api/*` contract).
- `npm run build` in `Src/CarStore.Web/ClientApp` is required after any front-end change; it is not part of `dotnet build`.

## Architecture rules
- **Domain layer** (`CarStore.Domain`) owns all business rules.
  - Entities (`Brand`, `Car`) are **rich**: invariants are enforced through domain methods, not in services.
  - Invariants include: `Model required`, `Price >= 0`, `Stock >= 0`, `ModelYear` not in the future, `FoundedDate` not in the future, `IsAvailable = Stock > 0`, "Brand with cars cannot be deleted".
  - Throw `DomainException` (in `CarStore.Domain/Exceptions/DomainException.cs`) on invariant violations. Never throw `InvalidOperationException` from entities.
- **Application layer** (`CarStore.Application`) only orchestrates:
  - Load aggregates via `CarStoreContext`.
  - Call domain methods on entities.
  - Persist via `SaveChangesAsync`.
  - Do not duplicate business rules.
- **Controllers** are thin: model binding - service call - view/redirect. No business logic.

## Code style
- C# 12, nullable enabled, file-scoped namespaces, `var` when type is obvious.
- `async`/`await` end-to-end for all DB calls.
- Domain methods use verbs: `Rename`, `Restock`, `ChangePrice`, `AssignBrand`, etc.

## Build & run
- Build: `dotnet build Src/CarStore.slnx`
- Run: `dotnet run --project Src/CarStore.Web`
- Persistence: EF Core In-Memory only. Do not add migrations or change the provider.

## General guidance
- Read the file before editing it. Do not invent class names.
- Keep changes minimal and focused on the requested task.