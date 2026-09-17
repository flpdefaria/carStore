---
type: guide
created: 2026-07-22
updated: 2026-09-17
tags:
  - note
  - type/guide
  - area/engineering
  - component/web
  - entity/customer
---

# Controllers

MVC controllers in `CarStore.Web`, post-SPA migration (see [[Frontend-SPA]]). There is exactly one
page-serving controller left; everything else is JSON API controllers under `Controllers/Api/`.

## Context

`BooksController`, `AuthorsController` and `CustomersController` (the old paged-Index-view controllers) were
removed when the app became a Vue SPA. Their former responsibility — serving a paged list page per entity —
is now owned entirely by `vue-router` (client-side) plus the existing `/api/*` controllers (data).

## HomeController (the only page controller)

```csharp
// Src/CarStore.Web/Controllers/HomeController.cs
public class HomeController : Controller
{
    // Serves the single SPA shell page; Vue Router owns every other client-side route.
    public IActionResult Index() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = ... });
}
```

- `Index()` renders `Views/Home/Index.cshtml`, which contains the single `<div id="app">` mount point.
- `Program.cs` maps `app.MapFallbackToController("Index", "Home")` after the default route, so any
  unmatched path (e.g. `/cars`, `/brands/42`) also resolves to this same action — vue-router then renders
  the right page client-side from the URL.
- `Error()` is the `UseExceptionHandler` target; it renders outside the SPA shell (no `div#app`), so it is
  plain server-rendered HTML.

## JSON API controllers own all data access

`BooksApiController`, `AuthorsApiController`, `CustomersApiController` (all in `Controllers/Api/`) are
unchanged in shape by the SPA migration — see [[Application-Services]] and `pagination.instructions.md` for
their conventions (DTO mapping, `DomainException` → 400, paging). `CustomersApiController` gained
`Create`/`Update`/`Delete` actions as part of the migration (previously GET-only, since Customer CRUD used to
be classic Razor forms).

## Anti-forgery

No controller uses `[ValidateAntiForgeryToken]` anymore — the last Razor POST forms (Customers) were removed.
API controllers were never anti-forgery protected (there is no authentication in this solution).

## Source

- `Src/CarStore.Web/Controllers/HomeController.cs`
- `Src/CarStore.Web/Controllers/Api/BooksApiController.cs`
- `Src/CarStore.Web/Controllers/Api/AuthorsApiController.cs`
- `Src/CarStore.Web/Controllers/Api/CustomersApiController.cs`

## Related

- [[Frontend-SPA]]
- [[Application-Services]]
- [[Pagination]]
- [[Architecture-Overview]]
- [[Engineering-Overview]]
