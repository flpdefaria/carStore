---
applyTo: "Src/CarStore.Web/**/*.cshtml,Src/CarStore.Web/Controllers/**,Src/CarStore.Web/Models/**,Src/CarStore.Web/Program.cs,Src/CarStore.Web/*.csproj"
---

# Web instructions (ASP.NET Core MVC hosting Vue)

How `CarStore.Web` serves a Vue 3 + PrimeVue front-end from classic MVC. Companion to
`frontend.instructions.md` (everything inside `ClientApp/`), `pagination.instructions.md` and
`vault.instructions.md`.

This is the **reference implementation** for migrating a classic MVC + Razor + Bootstrap app to
an MVC-hosted Vue 3 single-page app (SPA).

## Mandatory vault workflow

1. Read `.github/instructions/vault.instructions.md`.
2. `/vault-search` before changing anything.
3. Do the work.
4. `/vault-write` afterwards (add new tags to `00_Index/Tags.md` first). Never hand-edit vault notes.

## The hosting model in one paragraph

ASP.NET Core MVC serves exactly one shell page (`HomeController.Index` -> `Views/Home/Index.cshtml`), which
renders a single `<div id="app">`. `_Layout.cshtml` loads the single Vite bundle (`~/dist/main.css`,
`~/dist/main.js` as `type="module"`); `ClientApp/src/main.ts` mounts one Vue app onto `#app`, wired with
`vue-router` (history mode). Vue Router - not MVC - owns in-app navigation between `/`, `/cars`, `/brands`,
`/customers`. `Program.cs` maps a fallback route (`MapFallbackToController("Index", "Home")`) so a deep link
or a full page refresh on any client route still resolves to the same shell, and Vue Router then renders the
right page from the current URL. Data is **not** pushed through the Razor model - Vue components fetch it
from JSON endpoints under `/api/*`.

```
Browser GET /cars (first load or refresh)
  └─ No BooksController exists -> MVC fallback -> HomeController.Index -> Views/Home/Index.cshtml (div#app only)
       └─ _Layout.cshtml            -> ~/dist/main.css + ~/dist/main.js
            └─ main.ts mount()      -> App.vue (Sidebar + RouterView) mounted on #app
                 └─ vue-router resolves "/cars" -> BooksPage.vue
                      └─ fetch /api/cars?page=1&pageSize=10  -> BooksApiController -> IBookService.GetPagedAsync

Browser click on a Sidebar nav item (already loaded)
  └─ vue-router intercepts, swaps <RouterView> content client-side - no server round-trip, no full reload
```

## Layout contract (`Views/Shared/_Layout.cshtml`)

- `<head>`: `~/dist/main.css`, then `~/css/site.css`, then the scoped-CSS bundle - all with `asp-append-version="true"` for cache busting.
- `<body class="h-screen overflow-hidden bg-surface-0">` - the shell layout itself uses Tailwind classes, which is why `ClientApp/src/style.css` declares `@source "../../Views";`.
- `@RenderBody()` is the **only** body content. `Views/Home/Index.cshtml` is the sole page that renders anything meaningful there - a single `<div id="app" class="flex h-full w-full gap-3 p-4">`, which `App.vue` (Sidebar + `<RouterView />`) mounts into. Any other Razor view (e.g. `Home/Error.cshtml`) renders without the SPA shell.
- Scripts, in order: `~/dist/main.js` (`type="module"`), `~/js/site.js`, then `@await RenderSectionAsync("Scripts", required: false)`. jQuery was removed together with the last Razor CRUD forms (Customers) - do not reintroduce it.
- The bundle is loaded **globally, once**, in the layout. Do **not** add per-view `<script src="~/dist/main.js">` inside a `@section Scripts` block - that would mount the app twice.

## Writing the SPA shell / adding a route

```cshtml
@{
    ViewData["Title"] = "Car Store";
}

<div id="app" class="flex h-full w-full gap-3 p-4"></div>
```

Rules:

- There is exactly **one** mount point (`id="app"`), declared once in `Views/Home/Index.cshtml`. Do not add
  per-feature Razor views or mount `<div>`s anymore - new pages are added entirely on the Vue side (see
  `frontend.instructions.md` -> "How a component reaches the page").
- `Program.cs` maps `app.MapFallbackToController("Index", "Home")` after the default MVC route, so any path
  vue-router owns (`/cars`, `/brands/123`, ...) that doesn't match a real MVC/API endpoint falls back to
  this same shell on a full load or refresh.
- Serialize domain objects into the view only when there is no alternative. The default is: markup in Razor,
  data over `/api`.

### Current page inventory

| Route (client-side, via vue-router) | Component | Rendering |
|---|---|---|
| `/` | `Home.vue` | Vue |
| `/cars` | `BooksPage.vue` | Vue: list + Create/Edit/Details/Delete dialogs (full CRUD in Vue) |
| `/brands` | `AuthorsPage.vue` | Vue: list + Create/Edit/Details/Delete dialogs (full CRUD in Vue) |
| `/customers` | `CustomersPage.vue` | Vue: list + Create/Edit/Details/Delete dialogs (full CRUD in Vue) |

| Server route | View | Purpose |
|---|---|---|
| `/` (and any unmatched path) | `Views/Home/Index.cshtml` | SPA shell (`div#app`); served directly for `/` and via `MapFallbackToController` for every vue-router path |
| `/Home/Error` | `Views/Home/Error.cshtml` | Server-rendered error page (`UseExceptionHandler`), intentionally outside the SPA shell |

Customers was the last page migrated off Razor CRUD forms (`Create/Edit/Details/Delete.cshtml` + jQuery
unobtrusive validation) - there are no surviving Razor forms in the app. `Views/Shared/_Pagination.cshtml`
and `_ValidationScriptsPartial.cshtml` were removed for the same reason.

## MVC controllers

`HomeController` is the only page-serving controller left: `Index()` (SPA shell) and `Error()`
(`UseExceptionHandler` target). `BooksController`, `AuthorsController` and `CustomersController` were removed
- their former responsibility (paged Index views) is now owned entirely by vue-router + the `/api/*`
controllers. Do not add business logic to `HomeController` (see `.github/copilot-instructions.md`).

## JSON API controllers (`Controllers/Api/`)

The bridge between MVC services and Vue components:

```csharp
[ApiController]
[Route("api/cars")]
public class BooksApiController : ControllerBase
{
    [HttpGet]                        // GET /api/cars?page=1&pageSize=10 -> PagedResultDto<BookDto>
    [HttpPost]                       // create      -> 201 + BookDto
    [HttpPut("{id}")]                // update      -> 200 + BookDto | 404
    [HttpDelete("{id}")]             // delete      -> 204 | 404
}
```

Conventions:

- **Reuse the existing application services** (`IBookService`, `IAuthorService`, `ICustomerService`). Never duplicate business rules, validation or paging logic in an API controller - see `pagination.instructions.md`.
- **Always map to a DTO** in `Models/Api/`. Entities have circular navigation properties (`Car.Brand` / `Brand.Cars`) that break JSON serialization, and DTOs also flatten what the table needs (`AuthorName`, `BooksCount`, `Age`).
- Paged responses use `PagedResultDto<T>` (`Items`, `PageNumber`, `PageSize`, `TotalItems`, `TotalPages`, `HasPrevious`, `HasNext`) - it mirrors `PagedResult<T>` and the `PagedResult<T>` interface in `ClientApp/src/types.ts`.
- Catch `DomainException` and return `BadRequest(new { message = ex.Message })`. The front-end composables read `body.message` and show it in the dialog, so the domain message is the user-facing error.
- Return `NotFound()` for a missing id, `NoContent()` for a successful delete.
- Dropdown/lookup data gets its own endpoint (`GET /api/brands/options` -> `AuthorOptionDto[]`), never a full paged fetch.
- API controllers use `[ApiController]` + `ControllerBase` (no views). There is no authentication in this solution; add auth before exposing mutating endpoints publicly.
- Keep the JSON casing default (camelCase) - `types.ts` depends on it.

## Static assets

- `wwwroot/dist/` - Vite output (`main.js`, `main.css`, PrimeIcons fonts). **Generated, but committed**: `dotnet build`/`publish` never runs npm, so the deployed app would otherwise ship no front-end. Never hand-edit.
- `wwwroot/images/` - Figma-exported assets, referenced by absolute path from Vue (`/images/home/cars.jpg`).
- `wwwroot/lib/` - leftover jquery/jquery-validation packages from the pre-SPA Customers forms; no longer
  referenced by any view. **No bootstrap folder** - do not reintroduce one.
- `wwwroot/css/site.css` - base font sizing only; app styling belongs to Tailwind/PrimeVue.
- `Program.cs` uses `app.MapStaticAssets()` + `.WithStaticAssets()` (ASP.NET Core 10 static asset pipeline), so fingerprinting/compression is handled for files present at build time.

## Build, run, deploy

```bash
# front-end (after any ClientApp change)
cd Src/CarStore.Web/ClientApp && npm install && npm run build

# back-end
dotnet build Src/CarStore.slnx
dotnet run --project Src/CarStore.Web      # http://localhost:5045
dotnet test Src/CarStore.Tests/CarStore.Tests.csproj

# deploy (Azure App Service, zip deploy) - requires wwwroot/dist to be up to date and committed
./deploy.sh
```

VS Code tasks: `build` (default) and `watch` (`dotnet watch run`). `dotnet watch` reloads C#/Razor only -
front-end changes still need `npm run build`.

Persistence is EF Core **In-Memory** (`CarStoreDb`), re-seeded by `DataSeeder.Seed(db)` on every start:
data resets on restart, and every mutation done through the API disappears with the process.

## Checklist for a change touching this layer

- [ ] `/vault-search` before, `/vault-write` after.
- [ ] Controller stays thin; business rules stay in the domain.
- [ ] New/changed API shape mirrored in `ClientApp/src/types.ts`.
- [ ] New client-side page added as a `router/index.ts` route + `Sidebar.vue` nav item, not a new Razor view/mount point.
- [ ] `dotnet build Src/CarStore.slnx` clean; `npm run build` re-run if ClientApp changed.
- [ ] Page loaded in the browser (including a hard refresh on a deep route) and the network tab shows the expected `/api/*` call.

## Never

- Never load `~/dist/main.js` from a view - it is a layout-level global.
- Never add a second bundler entry point or a per-page bundle; `main.ts` is the single entry.
- Never reintroduce Bootstrap CSS/JS or `bi-*` icons (PrimeIcons `pi pi-*` only).
- Never serialize EF entities directly to JSON - always a DTO.
- Never move paging or validation logic into an API controller.
- Never add EF Core migrations or change the persistence provider.
- Never add a page-serving MVC controller/Razor view for a route vue-router already owns - extend `router/index.ts` instead.

## Porting to a new project

1. Keep a single `HomeController` (SPA shell) + `Controllers/Api/` + `Models/Api/` DTOs over the existing services.
2. Add the `ClientApp/` Vite project with `vue-router` (see `frontend.instructions.md` -> "Porting to a new project").
3. In `_Layout.cshtml`: drop Bootstrap, add `~/dist/main.css` and `~/dist/main.js` (`type="module"`, `asp-append-version`), keep `@RenderBody()` only for genuinely server-rendered pages (errors, etc.).
4. `Views/Home/Index.cshtml` renders the single `<div id="app">`; add `app.MapFallbackToController("Index", "Home")` in `Program.cs` after the default route.
5. Migrate page by page: add a vue-router route + `<Feature>Page.vue` -> Vue list over the paged API -> then the CRUD dialogs -> then delete the obsolete Razor controller/views for that feature.
6. Commit `wwwroot/dist` if the deployment pipeline does not run npm; otherwise add an npm build step to CI and gitignore it.
7. Record the resulting structure in that project's vault with `/vault-write`.
