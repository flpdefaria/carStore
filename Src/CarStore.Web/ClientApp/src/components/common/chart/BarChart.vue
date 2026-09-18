<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, shallowRef, watch } from "vue";
import * as echarts from "echarts/core";
import { BarChart as EChartsBar } from "echarts/charts";
import { GridComponent, TooltipComponent } from "echarts/components";
import { CanvasRenderer } from "echarts/renderers";

echarts.use([EChartsBar, GridComponent, TooltipComponent, CanvasRenderer]);

const props = defineProps<{
  labels: string[];
  values: number[];
  valueFormatter?: (value: number) => string;
}>();

const el = ref<HTMLDivElement>();
const chart = shallowRef<echarts.ECharts>();
let resizeObserver: ResizeObserver | undefined;

function primaryColor(): string {
  return getComputedStyle(document.documentElement).getPropertyValue("--p-primary-color").trim() || "#10b981";
}

function buildOption() {
  return {
    grid: { left: 8, right: 8, top: 16, bottom: 24, containLabel: true },
    tooltip: {
      trigger: "axis" as const,
      valueFormatter: (value: unknown) => (props.valueFormatter ? props.valueFormatter(Number(value)) : String(value)),
    },
    xAxis: {
      type: "category" as const,
      data: props.labels,
      axisLine: { lineStyle: { color: "#e5e7eb" } },
      axisTick: { show: false },
      axisLabel: { color: "#6b7280", fontSize: 11 },
    },
    yAxis: {
      type: "value" as const,
      splitLine: { lineStyle: { color: "#f3f4f6" } },
      axisLabel: { color: "#6b7280", fontSize: 11 },
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

watch(
  () => [props.labels, props.values],
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
