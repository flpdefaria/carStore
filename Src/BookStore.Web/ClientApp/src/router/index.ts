import { createRouter, createWebHistory } from "vue-router";
import Home from "../components/Home.vue";
import BooksPage from "../components/BooksPage.vue";
import AuthorsPage from "../components/AuthorsPage.vue";
import CustomersPage from "../components/CustomersPage.vue";

// Client-side routes for the SPA shell; the server only ever serves Home/Index and falls back
// here for any unmatched path (see Program.cs MapFallbackToController).
const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "home", component: Home },
    { path: "/books", name: "books", component: BooksPage },
    { path: "/authors", name: "authors", component: AuthorsPage },
    { path: "/customers", name: "customers", component: CustomersPage },
  ],
});

export default router;
