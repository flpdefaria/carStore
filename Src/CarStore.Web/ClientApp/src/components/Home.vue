<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
import { type DataTablePageEvent } from "primevue/datatable";
import Card from "primevue/card";
import Tag from "primevue/tag";
import Button from "primevue/button";
import InputText from "primevue/inputtext";
import SelectButton from "primevue/selectbutton";
import Message from "primevue/message";
import DataTableCommon, { type DataTableColumn } from "./common/table/DataTableCommon.vue";
import PoweredByBadge from "./PoweredByBadge.vue";
import { usePagedFetch } from "../composables/usePagedFetch";
import { formatCurrency, formatDate } from "../utils/format";
import { primaryButtonClass, secondaryButtonClass } from "../styles/buttonStyles";
import type { BrandDto, CarDto, CustomerDto } from "../types";

const carsApiUrl = "/api/cars";
const brandsApiUrl = "/api/brands";
const customersApiUrl = "/api/customers";

// Aggregate loads (fleet is well under the 100-row page cap) used to compute the stat cards and charts below.
const { items: cars, load: loadCars } = usePagedFetch<CarDto>(carsApiUrl);
const { items: brands, load: loadBrands } = usePagedFetch<BrandDto>(brandsApiUrl);
const { items: customersForActivity, load: loadCustomersForActivity } = usePagedFetch<CustomerDto>(customersApiUrl);

// Real, server-paged preview of the customers list (separate from the aggregate load above).
const recentRows = ref(5);
const recentFirst = ref(0);
const {
  items: recentCustomers,
  totalRecords: recentTotal,
  loading: recentLoading,
  error: recentError,
  load: loadRecentCustomers,
} = usePagedFetch<CustomerDto>(customersApiUrl);

const recentColumns: DataTableColumn[] = [
  { field: "fullName", header: "Customer", primary: true },
  { field: "email", header: "Email" },
  { field: "createdAt", header: "Joined" },
];

function onRecentPage(event: DataTablePageEvent) {
  recentFirst.value = event.first;
  recentRows.value = event.rows;
  loadRecentCustomers(event.page + 1, event.rows);
}

onMounted(() => {
  loadCars(1, 100);
  loadBrands(1, 100);
  loadCustomersForActivity(1, 100);
  loadRecentCustomers(1, recentRows.value);
});

// Stat cards - real aggregates from the fetched fleet, no fabricated deltas or history.
const carsInStock = computed(() => cars.value.reduce((sum, c) => sum + c.stock, 0));
const fleetValue = computed(() => cars.value.reduce((sum, c) => sum + c.price * c.stock, 0));
const availableCars = computed(() => cars.value.filter((c) => c.isAvailable).length);
const availabilityRate = computed(() =>
  cars.value.length ? Math.round((availableCars.value / cars.value.length) * 100) : 0,
);

// Fleet-by-brand mix: top brands by registered car count.
const totalBrandCars = computed(() => brands.value.reduce((sum, b) => sum + b.carsCount, 0));
const topBrands = computed(() => [...brands.value].sort((a, b) => b.carsCount - a.carsCount).slice(0, 5));
const brandDotClasses = ["bg-surface-700", "bg-blue-400", "bg-emerald-400", "bg-amber-400", "bg-surface-400"];

function brandSharePercent(carsCount: number): number {
  return totalBrandCars.value ? Math.round((carsCount / totalBrandCars.value) * 100) : 0;
}

// New customers over time, grouped by the selected period - real `createdAt` data, no synthetic history.
const periodOptions = ["Weekly", "Monthly", "Yearly"];
const period = ref("Monthly");

function bucketLabel(createdAt: string): string {
  const date = new Date(createdAt);
  if (period.value === "Yearly") return `${date.getFullYear()}`;
  if (period.value === "Weekly") {
    const firstDay = new Date(date.getFullYear(), 0, 1);
    const week = Math.ceil(((date.getTime() - firstDay.getTime()) / 86400000 + firstDay.getDay() + 1) / 7);
    return `Wk ${week}`;
  }
  return date.toLocaleDateString("en-US", { month: "short" });
}

const activityBuckets = computed(() => {
  const sorted = [...customersForActivity.value].sort(
    (a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime(),
  );
  const buckets = new Map<string, number>();
  for (const customer of sorted) {
    const label = bucketLabel(customer.createdAt);
    buckets.set(label, (buckets.get(label) ?? 0) + 1);
  }
  return Array.from(buckets, ([label, count]) => ({ label, count }));
});
const maxBucketCount = computed(() => Math.max(1, ...activityBuckets.value.map((b) => b.count)));
</script>

<template>
  <div class="flex flex-col gap-4">
    <header class="flex flex-wrap items-start justify-between gap-4">
      <div class="flex flex-col gap-1">
        <div class="flex items-center gap-1.75 text-xs text-muted-color">
          <span>Overview</span>
          <span class="flex items-center gap-1">
            <span class="size-1.5 rounded-full bg-emerald-500"></span>
            Live
          </span>
        </div>
        <h1 class="text-3xl font-black text-surface-700">Fleet command</h1>
        <p class="text-sm text-muted-color">Monitor stock exposure, fleet value, and recent customer activity.</p>
      </div>
      <div class="flex items-center gap-1.75">
        <InputText placeholder="Search cars, brands, customers" class="w-64!" />
        <Button text rounded severity="secondary" aria-label="Notifications">
          <i class="pi pi-bell" />
        </Button>
      </div>
    </header>

    <div class="grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-4">
      <Card class="!rounded-xl !shadow-sm">
        <template #content>
          <div class="flex items-center justify-between">
            <span class="text-xs text-muted-color">Fleet value</span>
            <i class="pi pi-wallet rounded-md bg-surface-100 p-1.75 text-sm text-surface-700" />
          </div>
          <p class="mt-2 text-2xl font-bold text-surface-700">{{ formatCurrency(fleetValue) }}</p>
        </template>
      </Card>

      <Card class="!rounded-xl !shadow-sm">
        <template #content>
          <div class="flex items-center justify-between">
            <span class="text-xs text-muted-color">Cars in stock</span>
            <i class="pi pi-car rounded-md bg-surface-100 p-1.75 text-sm text-surface-700" />
          </div>
          <p class="mt-2 text-2xl font-bold text-surface-700">{{ carsInStock }}</p>
        </template>
      </Card>

      <Card class="!rounded-xl !shadow-sm">
        <template #content>
          <div class="flex items-center justify-between">
            <span class="text-xs text-muted-color">Registered customers</span>
            <i class="pi pi-address-book rounded-md bg-surface-100 p-1.75 text-sm text-surface-700" />
          </div>
          <p class="mt-2 text-2xl font-bold text-surface-700">{{ recentTotal }}</p>
          <Tag value="Live" severity="info" class="mt-2 !text-xs" />
        </template>
      </Card>

      <Card class="!rounded-xl !shadow-sm">
        <template #content>
          <div class="flex items-center justify-between">
            <span class="text-xs text-muted-color">Availability rate</span>
            <i class="pi pi-check-circle rounded-md bg-surface-100 p-1.75 text-sm text-surface-700" />
          </div>
          <p class="mt-2 text-2xl font-bold text-surface-700">{{ availabilityRate }}%</p>
          <Tag
            :value="availabilityRate >= 50 ? 'High' : 'Low'"
            :severity="availabilityRate >= 50 ? 'success' : 'warn'"
            class="mt-2 !text-xs"
          />
        </template>
      </Card>
    </div>

    <div class="grid grid-cols-1 gap-3 xl:grid-cols-3">
      <Card class="!rounded-xl !shadow-sm xl:col-span-2">
        <template #content>
          <div class="flex items-center justify-between">
            <p class="text-sm font-semibold text-color">New customers</p>
            <SelectButton v-model="period" :options="periodOptions" :allow-empty="false" class="text-xs" />
          </div>
          <div class="mt-4 flex h-40 items-end gap-3">
            <div v-for="bucket in activityBuckets" :key="bucket.label" class="flex flex-1 flex-col items-center gap-1.75">
              <div
                class="w-full rounded-t-md bg-surface-700"
                :style="{ height: `${Math.max(4, (bucket.count / maxBucketCount) * 100)}%` }"
              ></div>
              <span class="text-xs text-muted-color">{{ bucket.label }}</span>
            </div>
            <p v-if="!activityBuckets.length" class="w-full text-center text-sm text-muted-color">No customer activity yet.</p>
          </div>
        </template>
      </Card>

      <Card class="!rounded-xl !shadow-sm">
        <template #content>
          <div class="flex items-center justify-between">
            <p class="text-sm font-semibold text-color">Fleet by brand</p>
          </div>
          <ul class="mt-4 flex flex-col gap-2.5">
            <li v-for="(brand, index) in topBrands" :key="brand.id" class="flex items-center gap-1.75 text-sm">
              <span class="size-2 shrink-0 rounded-full" :class="brandDotClasses[index]"></span>
              <span class="min-w-0 flex-1 truncate text-color">{{ brand.name }}</span>
              <span class="shrink-0 text-xs text-muted-color">{{ brandSharePercent(brand.carsCount) }}%</span>
            </li>
            <li v-if="!topBrands.length" class="text-sm text-muted-color">No brands registered yet.</li>
          </ul>
          <Button :as="RouterLink" to="/brands" :class="secondaryButtonClass + ' mt-4 w-full justify-center'">
            View brands
            <i class="pi pi-arrow-right text-xs" />
          </Button>
        </template>
      </Card>
    </div>

    <Card class="!rounded-xl !shadow-sm">
      <template #content>
        <div class="flex items-center justify-between">
          <p class="text-sm font-semibold text-color">Recent customers</p>
          <Button :as="RouterLink" to="/customers" :class="secondaryButtonClass">
            View all
            <i class="pi pi-arrow-right text-xs" />
          </Button>
        </div>
        <Message v-if="recentError" severity="error" :closable="false" class="mt-3">{{ recentError }}</Message>
        <div class="mt-3">
          <DataTableCommon
            :value="recentCustomers"
            :columns="recentColumns"
            :loading="recentLoading"
            :total-records="recentTotal"
            :first="recentFirst"
            :rows="recentRows"
            :rows-per-page-options="[5, 10]"
            @page="onRecentPage"
          >
            <template #col-createdAt="{ data }">
              <span class="text-xs text-muted-color">{{ formatDate(data.createdAt) }}</span>
            </template>
          </DataTableCommon>
        </div>
      </template>
    </Card>

    <div class="flex items-center justify-between">
      <PoweredByBadge />
      <Button :as="RouterLink" to="/cars" :class="primaryButtonClass">
        <i class="pi pi-car" />
        Manage fleet
      </Button>
    </div>
  </div>
</template>

