---
target: CarStore app (Home, Brands, Cars, Customers)
total_score: 24
max_score: 40
na_heuristics: 
p0_count: 2
p1_count: 2
target_identity: "file:/Users/felipefaria/projects/carStore/CarStore app (Home, Brands, Cars, Customers)"
timestamp: 2026-09-24T20-20-30Z
slug: carstore-app-home-brands-cars-customers
---
Method: dual-agent (A: aee959e6013234516 · B: aaf378c42b9f44aba)

## Design Health Score

| # | Heuristic | Score | Key Issue |
|---|-----------|-------|-----------|
| 1 | Visibility of System Status | 3/4 | Loading spinners exist on tables/dialogs, but no success toast after create/edit/delete — the dialog just closes silently |
| 2 | Match Between System and Real World | 4/4 | Strong automotive vocabulary throughout ("Fleet command," VIN, body type, mileage) — feels authored for a dealership, not a generic CRUD template |
| 3 | User Control and Freedom | 3/4 | Cancel/Escape/mask-dismiss work consistently; no undo after a destructive delete |
| 4 | Consistency and Standards | 3/4 | Shared `dialogStyles.ts`/`buttonStyles.ts` enforce visual consistency well, but `CarsPage.vue` has a stray literal-quote copy bug and `EditCarDialog.vue`'s mileage field contradicts its own in-code design comment |
| 5 | Error Prevention | 1/4 | Validation only fires on submit; zero `aria-invalid`, zero HTML `required` attributes, zero inline per-field feedback anywhere in the SPA source; brand-with-cars delete rule isn't pre-warned |
| 6 | Recognition Rather Than Recall | 2/4 | Edit dialogs prefill well, but the collapsed sidebar is icon-only and only expands on `mouseenter` — keyboard users never see the labels |
| 7 | Flexibility and Efficiency of Use | 1/4 | No search, sort, filter, bulk select, or keyboard shortcuts anywhere in `DataTableCommon.vue` |
| 8 | Aesthetic and Minimalist Design | 4/4 | The monochrome-plus-one-accent system (per DESIGN.md) is clean and genuinely restrained |
| 9 | Help Users Recognize/Diagnose/Recover from Errors | 2/4 | Errors surface via a single `Message` banner with raw text (e.g. "Request failed with status ${response.status}"); no remediation guidance |
| 10 | Help and Documentation | 1/4 | No tooltips, field hints, or inline help anywhere (VIN format, currency unit, etc.) |
| **Total** | | **24/40** | **Acceptable — significant improvements needed** |

No heuristics marked n/a; both Flexibility/Efficiency and Help/Documentation genuinely apply to this Operate-mode admin tool.

## Design Specificity Verdict

**LLM assessment**: CarStore reads as intentionally authored, not a generic admin template — copy is domain-specific ("Fleet command," "Fleet by brand"), fields match real automotive data, and a documented design system (`DESIGN.md`, with named rules like "One Accent," "Flat-by-Default") is genuinely followed in code. That deliberate feel is punctured by execution drift from its own spec (the `EditCarDialog` mileage field) and unexplained leftovers (a hardcoded personal identity in the sidebar, unstated BRL/pt-BR currency choice).

**Deterministic scan**: `impeccable detect --json` returned a clean scan (`[]`, exit 0) across the Vue source and Razor views — the automated detector caught none of the issues below. Every priority issue in this report was found by manual/agent review, not tooling; treat the clean scan as "no anti-pattern-library matches," not "no problems."

**Visual overlays**: Skipped. No browser automation tool is exposed in this session, so no live overlay could be injected and no user-visible highlighting exists in a browser tab. The dev server itself is reachable (`http://localhost:5045` returns 200), but nothing could be visually verified beyond source code — treat contrast, spacing-in-practice, and PrimeVue's own runtime focus styling as unverified by this run.

## Overall Impression

The bones are good — a real design system, consistent component patterns across all three CRUD entities, and confident domain-specific copy. What's missing is everything that makes an *admin tool at scale* feel trustworthy and efficient: no success feedback, no proactive prevention of the one business rule the product explicitly calls out (brand-with-cars delete), no search/sort/bulk actions, and zero accessibility affordances beyond what PrimeVue provides for free. The single biggest opportunity: close the loop on every action (toasts) and prevent the one error the domain model already knows about, before touching anything cosmetic.

## What's Working

- **Shared style modules** (`dialogStyles.ts`, `buttonStyles.ts`) give real design-token discipline — every dialog and button pulls from one source, matching `DESIGN.md`'s spec almost line for line.
- **Consistent CRUD architecture**: `BrandsPage/Table`, `CarsPage/Table`, `CustomersPage/Table` share `usePagedFetch` and `useEntityCrud` composables with identical wiring — a real system, not three one-off implementations.
- **Destructive-action confirmation is airtight**: all three delete flows (Brands, Cars, Customers) verifiably route through `ConfirmDeleteDialog.vue` before any DELETE request — no bypass found anywhere.

## Priority Issues

**[P0] EditCarDialog's mileage field contradicts its own design spec**
- **Why it matters**: `Src/CarStore.Web/ClientApp/src/components/common/dialog/EditCarDialog.vue` line 49 has a comment stating the Figma design shows mileage as read-only/dimmed in Edit mode, but the template (lines 144-146) renders it as a fully editable `InputNumber`. This is a shipped defect against the app's own documented intent, not a style debate.
- **Fix**: Bind `:disabled="true"` and `opacity-50` on the mileage `InputNumber` in edit mode, matching the comment.
- **Suggested command**: `/impeccable polish`

**[P0] Literal quote marks render in the Cars page description**
- **Why it matters**: `CarsPage.vue` line 42 wraps its `description` prop value in an extra pair of literal double quotes, unlike the equivalent Brands/Customers pages — stray `"…"` characters will render in the UI, undermining the authored-copy quality that's otherwise a strength.
- **Fix**: Strip the stray embedded quotes from the string literal.
- **Suggested command**: `/impeccable clarify`

**[P1] No proactive warning for the brand-deletion domain rule**
- **Why it matters**: `PRODUCT.md` states a brand with cars cannot be deleted, but `ConfirmDeleteDialog` shows the same generic "Are you sure?" copy regardless — users only discover the constraint after a failed API call. A known, documented rule is being enforced reactively instead of preventively.
- **Fix**: When the target brand has `carsCount > 0`, disable the Delete action or show an inline explanation in the confirm dialog instead of letting the request fail.
- **Suggested command**: `/impeccable harden`

**[P1] No success feedback after create/edit/delete, anywhere**
- **Why it matters**: Confirmed by both assessments — dialogs close silently on success with no toast/snackbar in the entire app. This is a Heuristic 1 gap and flattens the peak-end effect on every completed task.
- **Fix**: Add a PrimeVue Toast ("Car created," "Brand updated," "Customer removed") on every successful mutation.
- **Suggested command**: `/impeccable delight`

**[P2] No search, sort, filter, or bulk actions in any table**
- **Why it matters**: `DataTableCommon.vue` exposes pagination only. For an inventory-management tool, finding one VIN among many means paging manually, and removing several rows means N separate confirm-dialog round trips.
- **Fix**: Enable PrimeVue's built-in sortable columns and a global filter input; add a selection column with bulk delete.
- **Suggested command**: `/impeccable optimize`

## Persona Red Flags

**Alex (impatient power user) — managing Cars:**
- No keyboard shortcut to open "New car"; the action is mouse-only.
- No multi-select/bulk delete — removing 20 discontinued cars means 20 separate confirm-dialog round trips.
- Row actions require two clicks (open the `pi-ellipsis-v` menu, then pick Edit) instead of an inline quick-edit affordance.
- No search or column sort — finding one VIN means paging through the list manually.

**Sam (accessibility-dependent) — creating/editing/deleting a Car:**
- `DataTableCommon.vue` line 158: the row-actions button has `aria-haspopup="true"` but no `aria-label` on its icon-only `pi-ellipsis-v` glyph — confirmed independently by both assessments; a screen reader announces an unlabeled button. (Zero `aria-label` usages exist anywhere in the SPA source.)
- `Sidebar.vue`: nav labels only appear on `mouseenter`/`mouseleave` — a keyboard-only user tabbing through nav links never triggers expansion, so labels stay hidden from the accessibility tree.
- Validation errors surface only as a bottom-of-form banner, not tied to the offending input via `aria-invalid`/`aria-describedby` — confirmed via grep: zero `aria-invalid` occurrences in app source. A screen-reader user has no way to know which field caused a failure.

**Jordan (first-timer, brief):**
- The only affordance for row actions is an unlabeled `pi-ellipsis-v` icon button with no tooltip — a first-time user may not realize each row even has Edit/Delete/Details options.

## Minor Observations

- `Sidebar.vue` hardcodes "Felipe Faria" / a personal email as the logged-in user, despite `PRODUCT.md` stating there's no auth — looks like a development leftover in a teaching artifact.
- Currency is hardcoded to `BRL`/`pt-BR` with no stated locale policy in `PRODUCT.md`/`DESIGN.md`.
- The Price `InputNumber` has no `:min="0"` while Stock and Mileage do, despite the domain invariant that price must be non-negative.
- `DataTableCommon.vue` has no custom empty-state template; empty tables fall back to PrimeVue's default "No available data" text, inconsistent with `Home.vue`'s custom-authored empty-state copy.
- No skeleton loaders anywhere — only spinner/`:loading` states.

## Questions to Consider

- If the domain explicitly forbids deleting a brand with cars, why wait for a server error instead of disabling that action in the UI when `carsCount > 0`?
- The "One Accent" design rule is followed almost too faithfully — with zero success toasts and validation reduced to one banner message, does the interface read as *quiet and confident*, or just *quiet*?
- Is the hardcoded "Felipe Faria" identity in the sidebar a deliberate workshop touch, or a leftover that should become a generic seeded persona?
