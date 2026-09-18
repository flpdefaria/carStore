---
type: guide
created: 2026-09-17
updated: 2026-09-18
tags:
  - note
  - type/guide
  - area/engineering
  - component/web
---

# Sidebar Collapse/Expand on Hover

The global sidebar collapses to an icon-only rail matching Figma by default and expands to a bordered full
layout on hover.

## Context

`Src/CarStore.Web/ClientApp/src/components/common/sidebar/Sidebar.vue` previously rendered a single fixed
`w-69.5` layout at all times. It now defaults to a collapsed rail and expands back to the full layout (logo
text, nav labels, profile name/email) while the pointer is over the `<aside>`.

The collapsed state was first implemented with invented values (`figma-mcp` was not reachable at the time).
On 2026-09-18, `figma-mcp` became available: `get_design_context` was pulled for node `216:3304` (the
"sidebar collapse" variant) and then for node `219:8065` (the "sidebar expanded" variant, both in file
`2zyEr3S75NxHIJ5vgTFCWR`), and the whole component was corrected to match both exactly. The expanded pull
revealed the container background/border and the nav-item styling are actually **identical** between the two
states (only label/text visibility and width differ) — the initial collapsed-only pass had wrongly assumed
the previously-invented expanded look (bordered white card, `bg-blue-100` active pill) was intentional; it
was replaced with the real shared values below. The Code Connect auto-mapping suggested for both nodes failed
("Published component not found") since these are local Vue components, not a published Figma library —
expected, and not a blocker.

## Details

- State: a local `expanded` ref (`false` by default), toggled by `@mouseenter`/`@mouseleave` on the `<aside>`.
- Width: `w-[78px]` (collapsed) / `w-[278px]` (expanded), animated via `transition-[width] duration-200
  ease-in-out` and `overflow-hidden` on the root to avoid content reflow.
- Container background: `bg-surface-100`, no border, in **both** states (previously the collapsed/expanded
  backgrounds were assumed to differ; the expanded pull disproved that).
- Logo box: `size-10 rounded-xl overflow-hidden` with the image filling it via `object-cover` (Figma has no
  border/background wrapper around the logo, in either state).
- Nav items: `gap-3` between items (Figma: 12px, previously `gap-1.75`/7px), no border in either state.
  Background/text is the same in both states: active = `bg-surface-800` with `text-surface-0`; inactive =
  `bg-surface-100` with `text-color` (plus a `hover:bg-surface-50` affordance not shown in Figma's static
  frames, kept for interaction feedback). Only the label `<span>`'s visibility (`v-show="expanded"`) differs
  between states. Padding is `py-[10.5px] pr-[10.5px] pl-[14.5px]` — a **static** (never toggled) asymmetric
  left padding, computed so the 14px icon sits centered inside the 78px collapsed rail
  (`(78 - 2*17.5 aside-padding - 14 icon) / 2 = 14.5`); a `justify-center`/`justify-start` toggle was tried
  first but made the icon visibly slide during the width transition — using one fixed padding value for both
  states keeps the icon centered when collapsed with zero movement on hover, at the cost of the label sitting
  4px further right than the previous symmetric padding when expanded (visually negligible).
- Profile card: `bg-surface-50` + `border-surface-300` wrapper only when expanded (Figma-verified — the
  collapsed variant has no card, just the bare avatar), `justify-center`/`justify-start` toggle for the
  avatar's horizontal position.
- Avatar: `size-[35px]` when collapsed, `size-10` when expanded (Figma has `42px`; kept on the existing
  Tailwind scale value since the 2px difference is visually negligible).
- Content hidden when collapsed via `v-show="expanded"`: the "CarStore" / "Premium dealership" text block,
  each nav item's label `<span>`, and the profile card's name/email block. Icons (logo icon, nav icons,
  avatar) remain visible in both states so the rail stays usable collapsed.

## Fixes after initial pass

- **Avatar deformation**: the PrimeVue `Avatar` had no explicit size/`shrink-0`, so during the width
  transition the flex row could squeeze it into an oval. Fixed with `size-10 shrink-0` on the `Avatar` class.
- **Row height jumping on hover**: nav item labels and the logo/profile text had no `whitespace-nowrap`, so
  while the `<aside>` width was mid-transition the label text would wrap onto a second line and grow the
  row's height. Fixed by giving the logo row, each nav `router-link`, and the profile card an explicit
  fixed height (`h-10` / `h-11` / `h-19`) plus `overflow-hidden`, and adding `truncate whitespace-nowrap`
  to every label (`truncate` already implies `whitespace-nowrap` but it was applied inconsistently before).
- **Icons must stay fixed, only the rail opens/closes**: icons are the first flex child in each row with a
  fixed left padding (`p-[11.5px]`) and default `justify-start`, so their position never depends on whether
  the sibling label is shown — only the label's `v-show` and the `<aside>` width change; no `justify-center`
  was introduced for the collapsed state, since that would shift the icon horizontally between states.
- **Profile avatar centered when collapsed**: unlike the nav icons, the profile card's avatar is centered
  horizontally in the collapsed rail (`justify-center` vs `justify-start` on the card, toggled by `expanded`)
  since it sits alone in a wider card `p-[17.5px]` and looked off-center pinned to the left edge.
- **Card background/border removed when collapsed (not the avatar's own circle)**: the profile card's
  `border-surface-300`/`bg-surface-50` wrapper is only applied when `expanded`; the `Avatar` itself keeps its
  own `bg-surface-300!` circular fill at all times, so collapsed it's just the round avatar sitting on the
  rail with no surrounding card.

## Related

- [[Sidebar-Logo-Icon]]
- [[Frontend-SPA]]
