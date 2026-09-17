<script setup lang="ts">
import { RouterView } from "vue-router";
import Sidebar from "./components/common/sidebar/Sidebar.vue";
</script>

<template>
  <Sidebar />
  <div class="flex h-full min-w-0 flex-1 flex-col">
    <main role="main" class="flex-1 overflow-y-auto">
      <RouterView />
    </main>
  </div>
</template>
