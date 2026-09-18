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
dashboard, first styled after a reference screenshot (a generic crypto-portfolio dashboard UI), then
progressively corrected against real Figma nodes (file `2zyEr3S75NxHIJ5vgTFCWR`) as they became reachable.
Rebuilt entirely with real data from the existing `/api/cars`, `/api/brands`, `/api/customers` endpoints —
no fabricated metrics, deltas, or history.

## Layout

- Header: "Fleet command" title + description only (the "Overview/Live" eyebrow, decorative search
  `InputText`, and notification bell `Button` from the first pass were removed per user request — they had
  no backing functionality).
- Stat card row (`Card` x4: fleet value, cars in stock, registered customers, availability rate), corrected
  on 2026-09-18 against Figma node `217:6263` ("card cars" component set): `!bg-surface-50` card background
  (not the default white), `text-sm font-medium` muted label, a `size-9 bg-surface-0 rounded-md` icon box
  wrapping a `text-xl` icon (not a bare padded icon), and a `text-[32px] font-bold` value (Figma's exact
  32px, off Tailwind's default scale). All four values are `computed()` from the fetched `CarDto[]`/
  `CustomerDto[]` — no deltas/history, since Figma's card also does not show any real trend data beyond the
  static tags.
- "New customers" panel: an ECharts `BarChart` (see `components/common/chart/BarChart.vue`, added
  separately per the `echarts-chart-build` skill) grouping real `CustomerDto.createdAt` values by
  week/month/year via a `SelectButton` period toggle (`period` ref drives `bucketLabel()`). This replaced an
  earlier CSS-only bar chart from before ECharts was installed.
- "Fleet by brand" panel: top 5 `BrandDto` by `carsCount`, shown as a dot + name + share-of-fleet percentage
  + progress bar, linking to `/brands`.
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
