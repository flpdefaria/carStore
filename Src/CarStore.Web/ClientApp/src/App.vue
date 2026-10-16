<script setup lang="ts">
import { RouterView } from "vue-router";
import Toast from "primevue/toast";
import Sidebar from "./components/common/sidebar/Sidebar.vue";
</script>

<template>
  <Toast position="bottom-right" />
  <Sidebar />
  <div class="flex h-full min-w-0 flex-1 flex-col">
    <main role="main" class="flex-1 overflow-y-auto">
      <RouterView v-slot="{ Component, route }">
        <Transition name="route" mode="out-in">
          <component :is="Component" :key="route.path" />
        </Transition>
      </RouterView>
    </main>
  </div>
</template>
