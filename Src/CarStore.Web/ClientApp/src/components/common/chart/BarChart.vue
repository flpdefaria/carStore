<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, shallowRef, watch } from "vue";
import * as echarts from "echarts/core";
import { BarChart as EChartsBar } from "echarts/charts";
import { GridComponent, TooltipComponent } from "echarts/components";
import { CanvasRenderer } from "echarts/renderers";
import { useTheme } from "../../../composables/useTheme";

echarts.use([EChartsBar, GridComponent, TooltipComponent, CanvasRenderer]);

const props = defineProps<{
  labels: string[];
  values: number[];
  valueFormatter?: (value: number) => string;
  /** CSS custom property for the bar color; defaults to the PrimeVue primary. */
  colorVar?: string;
}>();

const el = ref<HTMLDivElement>();
const chart = shallowRef<echarts.ECharts>();
let resizeObserver: ResizeObserver | undefined;

function themeColor(token: string, fallback: string): string {
  return getComputedStyle(document.documentElement).getPropertyValue(token).trim() || fallback;
}

function primaryColor(): string {
  return props.colorVar ? themeColor(props.colorVar, "#2f6f9f") : themeColor("--p-primary-color", "#10b981");
}

function buildOption() {
  const axisLineColor = themeColor("--p-surface-300", "#e5e7eb");
  const splitLineColor = themeColor("--p-surface-100", "#f3f4f6");
  const axisLabelColor = themeColor("--p-text-muted-color", "#6b7280");

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  return {
    animation: !reduceMotion,
    animationDuration: 500,
    animationEasing: "cubicOut" as const,
    animationDurationUpdate: 300,
    grid: { left: 8, right: 8, top: 16, bottom: 24, containLabel: true },
    tooltip: {
      trigger: "axis" as const,
      backgroundColor: themeColor("--p-surface-0", "#ffffff"),
      borderColor: axisLineColor,
      textStyle: { color: themeColor("--p-text-color", "#334155") },
      valueFormatter: (value: unknown) => (props.valueFormatter ? props.valueFormatter(Number(value)) : String(value)),
    },
    xAxis: {
      type: "category" as const,
      data: props.labels,
      axisLine: { lineStyle: { color: axisLineColor } },
      axisTick: { show: false },
      axisLabel: { color: axisLabelColor, fontSize: 11 },
    },
    yAxis: {
      type: "value" as const,
      splitLine: { lineStyle: { color: splitLineColor } },
      axisLabel: { color: axisLabelColor, fontSize: 11 },
    },
    series: [
      {
        type: "bar" as const,
        data: props.values,
        barMaxWidth: 32,
        itemStyle: { color: primaryColor(), borderRadius: [4, 4, 0, 0] },
      },
    ],
  };
}

onMounted(() => {
  if (!el.value) return;
  chart.value = echarts.init(el.value);
  chart.value.setOption(buildOption());
  resizeObserver = new ResizeObserver(() => chart.value?.resize());
  resizeObserver.observe(el.value);
});

// Colors are read from CSS tokens at option-build time, so a theme switch rebuilds the option.
const { theme } = useTheme();

watch(
  () => [props.labels, props.values, theme.value],
  () => chart.value?.setOption(buildOption()),
  { deep: true },
);

onBeforeUnmount(() => {
  resizeObserver?.disconnect();
  chart.value?.dispose();
});
</script>

<template>
  <div ref="el" class="h-40 w-full"></div>
</template>
