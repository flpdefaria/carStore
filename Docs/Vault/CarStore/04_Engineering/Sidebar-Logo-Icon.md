---
type: guide
created: 2026-08-13
updated: 2026-09-18
tags:
  - note
  - type/guide
  - area/engineering
  - component/web
---

# Sidebar Logo Icon

The sidebar's top-left logo box has gone through a PrimeIcons glyph and now renders an image asset again.

## Context

The logo box in `Src/CarStore.Web/ClientApp/src/components/common/sidebar/Sidebar.vue` originally rendered
`<img :src="'/images/home/books.gif'">`, was briefly replaced with a PrimeIcons `pi-book` glyph for scalable
icon consistency, and was then swapped again to an image: `<img :src="'/images/sidebar/minicar.gif'">`,
provided by the user and saved to `Src/CarStore.Web/wwwroot/images/sidebar/minicar.gif`.

## Details

- Markup now (`Sidebar.vue`):
  ```html
  <div class="size-10 shrink-0 overflow-hidden rounded-xl">
    <img :src="'/images/sidebar/minicar.gif'" alt="CarStore" class="size-full object-cover" />
  </div>
  ```
- The `src` MUST be a dynamic `:src` binding, not a plain `src="..."` attribute: Vite's `@vitejs/plugin-vue`
  asset-URL transform tries to resolve a literal `src` as a module import, which fails to resolve for a
  root-absolute `wwwroot`-served path and breaks the build (`Rollup failed to resolve import`). Binding it as
  a JS string (`:src="'/images/...'"`) — the same pattern `Home.vue` already used for its card images — skips
  that transform.
- Sizing: on 2026-09-18, `get_design_context` was pulled for Figma node `216:3304` ("sidebar collapse"
  variant) and the box was corrected to match it exactly: no border/background wrapper — the box itself is
  `size-10 rounded-xl overflow-hidden` and the image fills it with `size-full object-cover` (previously a
  bordered `bg-surface-0` box with a smaller `size-6 object-contain` centered icon, which was invented before
  Figma access was available).
- Figma node `216:3303`/`216:3304` (file `2zyEr3S75NxHIJ5vgTFCWR`) is now confirmed for the collapsed sidebar
  — see [[Sidebar-Collapse-Expand]] for the full breakdown of what else changed in that pass.
- `wwwroot/images/home/books.gif` is still unreferenced from `ClientApp/src`; left in place pending explicit
  confirmation it's safe to remove.

## Related

- [[Engineering-Overview]]
- [[Sidebar-Collapse-Expand]]
