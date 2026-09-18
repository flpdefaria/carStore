---
type: guide
created: 2026-09-17
updated: 2026-09-17
tags:
  - note
  - type/guide
  - area/engineering
  - component/web
---

# Sidebar Collapse/Expand on Hover

The global sidebar collapses to an icon-only rail by default and expands to the full layout on hover.

## Context

`Src/CarStore.Web/ClientApp/src/components/common/sidebar/Sidebar.vue` previously rendered a single fixed
`w-69.5` layout at all times. It now defaults to a collapsed `w-20` icon rail and expands back to the
existing `w-69.5` layout (logo text, nav labels, profile name/email) while the pointer is over the `<aside>`.

Figma node `207:2216` (file `2zyEr3S75NxHIJ5vgTFCWR`) was the requested reference for the collapsed-state
spec, but the `figma-mcp` server was not exposed as a callable tool in the session that made this change
(same limitation recorded in [[Sidebar-Logo-Icon]]), so the collapsed width/spacing could not be pulled
live. No new colors/tokens were introduced: the collapsed rail reuses the existing `surface-*` tokens,
paddings and icon sizing already established for the expanded layout. Re-verify the collapsed width and
spacing against Figma node `207:2216` next time the Figma MCP tools are available.

## Details

- State: a local `expanded` ref (`false` by default), toggled by `@mouseenter`/`@mouseleave` on the `<aside>`.
- Width: `w-20` (collapsed) / `w-69.5` (expanded), animated via `transition-[width] duration-200 ease-in-out`
  and `overflow-hidden` on the root to avoid content reflow during the transition.
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
