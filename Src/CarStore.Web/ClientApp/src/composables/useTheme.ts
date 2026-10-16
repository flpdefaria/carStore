import { ref } from "vue";

export type Theme = "light" | "dark";

const STORAGE_KEY = "carstore-theme";

function initialTheme(): Theme {
  let saved: string | null = null;
  try {
    saved = localStorage.getItem(STORAGE_KEY);
  } catch {
    // Storage blocked: fall through to the OS preference.
  }
  if (saved === "dark" || saved === "light") return saved;
  return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

// The inline script in _Layout.cshtml applies the same rule before first paint (no theme flash);
// this re-applies it in case the page shell was served without that script.
const theme = ref<Theme>(initialTheme());
document.documentElement.classList.toggle("app-dark", theme.value === "dark");

function apply(next: Theme) {
  document.documentElement.classList.toggle("app-dark", next === "dark");
  theme.value = next;
  try {
    localStorage.setItem(STORAGE_KEY, next);
  } catch {
    // Storage blocked (private mode): the choice simply won't persist.
  }
}

/**
 * Switches theme with a circular reveal grown from the control that triggered it (View Transitions),
 * falling back to an instant swap where unsupported or when reduced motion is requested.
 */
function toggle(event?: MouseEvent) {
  const next: Theme = theme.value === "dark" ? "light" : "dark";
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  if (!document.startViewTransition || reduceMotion) {
    apply(next);
    return;
  }

  const target = event?.currentTarget as HTMLElement | undefined;
  const rect = target?.getBoundingClientRect();
  const x = rect ? rect.left + rect.width / 2 : window.innerWidth / 2;
  const y = rect ? rect.top + rect.height / 2 : window.innerHeight / 2;
  const radius = Math.hypot(Math.max(x, window.innerWidth - x), Math.max(y, window.innerHeight - y));

  const transition = document.startViewTransition(() => apply(next));
  transition.ready.then(() => {
    document.documentElement.animate(
      { clipPath: [`circle(0px at ${x}px ${y}px)`, `circle(${radius}px at ${x}px ${y}px)`] },
      { duration: 550, easing: "cubic-bezier(0.16, 1, 0.3, 1)", pseudoElement: "::view-transition-new(root)" },
    );
  });
}

export function useTheme() {
  return { theme, toggle };
}
