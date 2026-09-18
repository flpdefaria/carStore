---
type: guide
created: 2026-09-18
updated: 2026-09-18
tags:
  - note
  - type/guide
  - area/engineering
  - component/web
---

# Home Dashboard

`Home.vue` (route `/`) was rewritten from a two-card "Cars/Brands" landing page into a small analytics
dashboard, styled after a reference screenshot the user provided (a generic crypto-portfolio dashboard UI,
not a Figma node) and rebuilt entirely with real data from the existing `/api/cars`, `/api/brands`,
`/api/customers` endpoints — no fabricated metrics, deltas, or history.

## Layout

- Header: "Overview / Live" eyebrow, "Fleet command" title, description, a decorative search `InputText`
  and a notification bell `Button` (both visual only — no backend endpoint exists for search or
  notifications, so neither is wired to any behavior).
- Stat card row (`Card` x4): fleet value (`sum(price * stock)`), cars in stock (`sum(stock)`), registered
  customers count, and availability rate (`available / total * 100`) — all `computed()` from the fetched
  `CarDto[]`/`CustomerDto[]`.
- "New customers" panel: a CSS-only bar chart (plain `div` heights, no charting library — none is in
  `package.json` and adding one is out of this component's scope) grouping real `CustomerDto.createdAt`
  values by week/month/year via a `SelectButton` period toggle (`period` ref drives `bucketLabel()`).
- "Fleet by brand" panel: top 5 `BrandDto` by `carsCount`, shown as a dot + name + share-of-fleet percentage
  list, linking to `/brands`.
- "Recent customers": a real lazy-paged `DataTableCommon` bound to `/api/customers` (separate
  `usePagedFetch` instance from the one used for the chart's aggregate load), rows defaulting to 5.
- Footer: kept the existing `PoweredByBadge`, added a "Manage fleet" CTA to `/cars`.

## Data loading

Three `usePagedFetch` instances: one each for `cars` and `brands` loaded with `pageSize=100` (the fleet is
well under the paging API's 100-row cap) purely to compute the aggregates above — this is a one-time
dashboard load, not a paginated list UI, so it does not conflict with the "no client-side pagination"
convention. A fourth instance drives the real lazy-paged "Recent customers" table.

## Related

- [[Frontend-SPA]]
- [[Pagination]]
