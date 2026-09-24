# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users
Primary "users" are workshop attendees and instructors following the GitHub Copilot Webinar Series. There is no real end-user audience: the interface simulates a car dealership back-office (managing brands, cars, customers) so that Copilot exercises have realistic domain complexity to work against.

## Product Purpose
CarStore is a teaching artifact, not a production product. It exists to demonstrate GitHub Copilot's Ask/Plan/Agent modes, custom instructions, skills, and agents on a realistic .NET 10 MVC + Vue 3 codebase. Each `Modules/moduleN/content.md` script uses the app to walk through a concrete Copilot workflow (design fixes, domain refactors, pagination, front-end migration, etc.).

## Positioning
Unlike a toy to-do app, CarStore mirrors a real three-layer .NET solution (rich domain, application services, MVC host) with an MVC-hosted Vue 3 + PrimeVue + Tailwind SPA - giving Copilot exercises authentic architecture, naming and layering to work within.

## Operating Context
- Solution: `Src/CarStore.slnx` (`CarStore.Domain`, `CarStore.Application`, `CarStore.Web`, `CarStore.Tests`).
- Front end: Vue 3 + PrimeVue 4 + Tailwind CSS v4 SPA under `Src/CarStore.Web/ClientApp`, built with Vite, served by ASP.NET Core MVC (`Views/Home/Index.cshtml` shell + `/api/*` JSON controllers).
- Persistence: EF Core In-Memory, reseeded on every run - no durable data, safe to reset between workshop sessions.
- Branches: `main` (stable), `Initial` (workshop starting point); `ModuleN` branches hold in-progress exercise work.

## Capabilities and Constraints
- Entities: `Brand`, `Car`, `Customer` - full CRUD via Vue pages/dialogs backed by `/api/brands`, `/api/cars`, `/api/customers`.
- Domain invariants enforced in rich entities (e.g. price/stock non-negative, model year not in the future, a brand with cars cannot be deleted).
- No authentication - out of scope for the workshop.
- Data does not persist across restarts (In-Memory provider by design; do not add migrations or change the provider).

## Brand Commitments
Preserve the product name "CarStore" and the domain terminology `Brand`, `Car`, `Customer` exactly as-is across any redesign or refinement - these are fixed anchors used throughout the workshop modules and vault documentation.

## Evidence on Hand
No real dealership data, testimonials, or business metrics exist or should be fabricated. Seed data (`CarStore.Domain/Seed/DataSeeder.cs`) is illustrative sample data only, regenerated on every app start.

## Product Principles
- Realism over simplicity: keep the domain/app layering authentic so Copilot exercises reflect real-world .NET architecture.
- Preserve fixed anchors (`CarStore`, `Brand`/`Car`/`Customer`) across any visual or structural change.
- No fabricated business content - only sample/seed data, clearly illustrative.
- Every module's exercise must remain reproducible: changes should not break the workshop scripts in `Modules/`.

## Accessibility & Inclusion
No project-specific accessibility requirement has been established beyond standard web accessibility practice.
