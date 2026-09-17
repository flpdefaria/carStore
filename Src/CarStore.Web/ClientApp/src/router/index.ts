import { createRouter, createWebHistory } from "vue-router";
import Home from "../components/Home.vue";
import CarsPage from "../components/CarsPage.vue";
import BrandsPage from "../components/BrandsPage.vue";
import CustomersPage from "../components/CustomersPage.vue";

// Client-side routes for the SPA shell; the server only ever serves Home/Index and falls back
// here for any unmatched path (see Program.cs MapFallbackToController).
const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "home", component: Home },
    { path: "/cars", name: "cars", component: CarsPage },
    { path: "/brands", name: "brands", component: BrandsPage },
    { path: "/customers", name: "customers", component: CustomersPage },
  ],
});

export default router;
