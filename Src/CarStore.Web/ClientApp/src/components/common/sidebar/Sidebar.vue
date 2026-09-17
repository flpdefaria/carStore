<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";
import Avatar from "primevue/avatar";

const route = useRoute();

const navItems = [
  { key: "home", label: "Home", icon: "pi-home", to: "/" },
  { key: "cars", label: "Cars", icon: "pi-car", to: "/cars" },
  { key: "brands", label: "Brands", icon: "pi-building", to: "/brands" },
  { key: "customers", label: "Customers", icon: "pi-address-book", to: "/customers" },
];

const activeKey = computed(() => route.name as string);
</script>

<template>
  <aside
    class="flex h-full w-69.5 shrink-0 flex-col gap-1.75 rounded-xl border border-surface-300 bg-surface-0 px-[17.5px] py-[17.5px]"
  >
    <div class="flex w-full items-center gap-1.75 py-[17.5px]">
      <div class="flex size-10 shrink-0 items-center justify-center rounded-lg border border-surface-300 bg-surface-0">
        <i class="pi pi-car text-2xl text-surface-700" />
      </div>
      <div class="flex min-w-0 flex-1 flex-col">
        <p class="truncate text-[22px] font-black leading-none text-surface-700">CarStore</p>
        <p class="text-xs text-muted-color">Premium dealership</p>
      </div>
    </div>

    <div class="h-px w-full bg-surface-300"></div>

    <nav class="flex w-full flex-col gap-1.75 py-[17.5px]">
      <router-link
        v-for="item in navItems"
        :key="item.key"
        :to="item.to"
        class="flex w-full items-center gap-1.75 rounded-lg border border-surface-300 p-[11.5px] text-sm font-medium no-underline text-color"
        :class="item.key === activeKey ? 'bg-blue-100' : 'bg-surface-0 hover:bg-surface-50'"
      >
        <i class="pi text-sm" :class="item.icon" />
        <span>{{ item.label }}</span>
      </router-link>
    </nav>

    <div class="flex-1"></div>

    <div class="flex w-full items-center gap-1.75 rounded-xl border border-surface-300 bg-surface-50 p-[17.5px]">
      <Avatar icon="pi pi-user" shape="circle" class="bg-surface-300! text-surface-700!" />
      <div class="flex min-w-0 flex-1 flex-col">
        <p class="truncate text-sm font-semibold text-color">Felipe Faria</p>
        <p class="truncate text-xs text-muted-color">felipe@email.com</p>
      </div>
    </div>
  </aside>
</template>

