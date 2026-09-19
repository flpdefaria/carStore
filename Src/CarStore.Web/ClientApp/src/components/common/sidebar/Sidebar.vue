<script setup lang="ts">
import { computed, ref } from "vue";
import { useRoute } from "vue-router";
import Avatar from "primevue/avatar";

const route = useRoute();

const navItems = [
  { key: "home", label: "Home", icon: "pi-home", to: "/" },
  { key: "cars", label: "Cars", icon: "pi-car", to: "/cars" },
  { key: "brands", label: "Brands", icon: "pi-building", to: "/brands" },
  { key: "customers", label: "Customers", icon: "pi-id-card", to: "/customers" },
];

const activeKey = computed(() => route.name as string);

// collapsed by default; hovering the rail expands it back to the full layout
const expanded = ref(false);
</script>

<template>
  <aside
    class="flex h-full shrink-0 flex-col gap-1.75 overflow-hidden rounded-xl bg-surface-100 px-[17.5px] py-[17.5px] transition-[width] duration-200 ease-in-out"
    :class="expanded ? 'w-[278px]' : 'w-[78px]'"
    @mouseenter="expanded = true"
    @mouseleave="expanded = false"
  >
    <div class="flex h-10 w-full items-center gap-1.75 overflow-hidden py-[17.5px]">
      <div class="size-10 shrink-0 overflow-hidden rounded-xl">
        <img :src="'/images/sidebar/minicar.gif'" alt="CarStore" class="size-full object-cover" />
      </div>
      <div v-show="expanded" class="flex min-w-0 flex-1 flex-col">
        <p class="truncate whitespace-nowrap text-[22px] font-black leading-none text-surface-700">CarStore</p>
        <p class="truncate whitespace-nowrap text-xs text-muted-color">Premium dealership</p>
      </div>
    </div>

    <nav class="flex w-full flex-col gap-3 py-[17.5px]">
      <router-link
        v-for="item in navItems"
        :key="item.key"
        :to="item.to"
        class="flex h-10 w-full items-center gap-1.75 overflow-hidden rounded-lg py-[10.5px] pl-[14.5px] pr-[10.5px] text-sm font-medium no-underline"
        :class="item.key === activeKey ? 'bg-surface-800 text-surface-0' : 'bg-surface-100 text-color hover:bg-surface-50'"
      >
        <i class="pi shrink-0 text-sm" :class="item.icon" />
        <span v-show="expanded" class="truncate whitespace-nowrap">{{ item.label }}</span>
      </router-link>
    </nav>

    <div class="flex-1"></div>

    <div
      class="flex h-19 w-full items-center gap-1.75 overflow-hidden rounded-xl p-[17.5px]"
      :class="expanded ? 'justify-start border border-surface-300 bg-surface-50' : 'justify-center'"
    >
      <Avatar
        icon="pi pi-user"
        shape="circle"
        class="shrink-0 bg-surface-300! text-surface-700!"
        :class="expanded ? 'size-10' : 'size-[35px]'"
      />
      <div v-show="expanded" class="flex min-w-0 flex-1 flex-col">
        <p class="truncate whitespace-nowrap text-sm font-semibold text-color">Felipe Faria</p>
        <p class="truncate whitespace-nowrap text-xs text-muted-color">felipe@email.com</p>
      </div>
    </div>
  </aside>
</template>

