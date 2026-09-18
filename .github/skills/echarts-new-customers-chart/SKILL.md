---
name: echarts-new-customers-chart
description: "One-shot setup skill: installs Apache ECharts into ClientApp and replaces the hand-rolled 'New customers' bars on the Home dashboard (Home.vue) with a real ECharts bar chart. Use only when this install/wiring has not been done yet - verify first (`echarts` in package.json dependencies, `components/common/chart/BarChart.vue` exists). Not a general charting skill: for ANY other chart (new type, new page, or a change to this one after the initial build), use `echarts-chart-build` instead - it owns all ongoing ECharts work and keeps every chart in the app on the same library/conventions."
argument-hint: "None required - this skill always targets ECharts install + the Home.vue New customers card"
---

# ECharts install + New Customers chart

Installs `echarts` in `Src/CarStore.Web/ClientApp` and wires a real chart into the "New customers" card on
`Home.vue`, replacing the manual `<div>`-bars implementation. Based on the official ECharts handbook
("Get Started"): https://echarts.apache.org/handbook/en/get-started/ - use the modular/tree-shaken import
style it documents (`echarts/core` + only the chart type, components and renderer actually used), not the
full `echarts` bundle, to keep `wwwroot/dist/main.js` as small as possible.

## Preconditions - check before doing anything

1. `grep '"echarts"' Src/CarStore.Web/ClientApp/package.json` - if present, the package is already installed; skip the install step.
2. `test -f Src/CarStore.Web/ClientApp/src/components/common/chart/BarChart.vue` - if it exists, the chart component already exists; skip creating it and go straight to wiring/verification.
3. If both already exist and `Home.vue`'s "New customers" card already renders `<BarChart ...>`, there is nothing left to do - report that and stop.

## Procedure

1. **Install.** From `Src/CarStore.Web/ClientApp`: `npm install echarts`. This adds `echarts` to `dependencies` in `package.json` (and `package-lock.json`) - do not add it to `devDependencies`, it ships in the bundle.
2. **Create the reusable chart component** at `src/components/common/chart/BarChart.vue` (this is a `common/<category>/` component per `frontend.instructions.md` - reusable, not page-specific):
   - Import only what's needed, per the handbook's tree-shaking guidance:
     ```ts
     import * as echarts from "echarts/core";
     import { BarChart as EChartsBar } from "echarts/charts";
     import { GridComponent, TooltipComponent } from "echarts/components";
     import { CanvasRenderer } from "echarts/renderers";
     echarts.use([EChartsBar, GridComponent, TooltipComponent, CanvasRenderer]);
     ```
   - Props: `labels: string[]`, `values: number[]`, optional `valueFormatter?: (value: number) => string`.
   - Lifecycle: `echarts.init()` on a template `ref` div in `onMounted`; `chart.setOption(...)` again in a `watch` on `[labels, values]` (`deep: true`) so it updates when data changes; a `ResizeObserver` on the container calling `chart.resize()`; `chart.dispose()` + `resizeObserver.disconnect()` in `onBeforeUnmount`. Never re-`init()` on every data change - only `setOption`.
   - Color: read the app's live PrimeVue primary token instead of a hardcoded hex, so the bar automatically matches the active theme: `getComputedStyle(document.documentElement).getPropertyValue("--p-primary-color").trim()`.
   - Axis/grid styling: keep it minimal (thin gray axis line, light gray split lines, small muted labels) to match the PrimeVue/Tailwind surface tokens used elsewhere in `Home.vue` - do not import an ECharts theme file.
   - Root template element: a single `<div ref="el" class="h-40 w-full">` (no fixed pixel height - h-40 matches the bars it replaces).
3. **Wire it into `Home.vue`**, inside the "New customers" `Card` (the one with the `SelectButton` for Weekly/Monthly/Yearly):
   - `import BarChart from "./common/chart/BarChart.vue";`
   - Derive two plain arrays from the existing `activityBuckets` computed (`label`/`count` pairs already grouped by the selected period) - do not duplicate the bucketing logic, only add `activityLabels`/`activityValues` computed properties that map over it.
   - Replace the manual bars markup with `<BarChart v-if="activityBuckets.length" :labels="activityLabels" :values="activityValues" />`, keeping the existing "No customer activity yet." empty state for when there's no data.
4. **Verify.** From `Src/CarStore.Web/ClientApp`: `npx vue-tsc --noEmit`, then `npm run build`; fix any error. Commit the regenerated `wwwroot/dist/` alongside the source change (same rule as every other ClientApp change in this repo).
5. **Load the app** (`dotnet run --project Src/CarStore.Web`, `http://localhost:5045`) and confirm the New customers card renders a real bar chart that responds to the Weekly/Monthly/Yearly toggle and to window resize.

## Rules

- Import from `echarts/core` + individual chart/component/renderer modules - never `import * as echarts from "echarts"` (pulls in every chart type and inflates the bundle).
- Never hardcode a bar color; read `--p-primary-color` so it tracks the PrimeVue theme.
- Never duplicate the `activityBuckets` grouping/period logic that already exists in `Home.vue` - only adapt its output into `labels`/`values` arrays for the chart prop contract.
- `BarChart.vue` is generic (labels + values in, no domain knowledge of "customers") so it can be reused for a future chart elsewhere in the app - do not name it `NewCustomersChart.vue` or bake customer-specific text into it.
- This skill's job ends once ECharts is installed and this one chart is wired in. Any further chart work - a second chart, a different chart type, a chart on another page, or restyling this one - goes through `echarts-chart-build`, not this skill.
- Follow the vault workflow used by the rest of this repo's agents/skills: `/vault-search` before, `/vault-write` after, recording the new dependency and the component added.
