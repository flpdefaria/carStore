import { onBeforeUnmount, ref, watch, type Ref } from "vue";

const easeOutExpo = (t: number) => (t === 1 ? 1 : 1 - Math.pow(2, -10 * t));

// Eases a displayed number toward `source` so a value that arrives from the API reads as a
// settling count rather than a jump. Interrupted tweens restart from the number currently shown;
// with prefers-reduced-motion the value is applied immediately.
export function useTweenedNumber(source: Ref<number>, duration = 600, delay = 0) {
  const display = ref(source.value);
  let frame = 0;
  let timer = 0;

  watch(source, (target) => {
    cancelAnimationFrame(frame);
    clearTimeout(timer);
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
      display.value = target;
      return;
    }
    timer = window.setTimeout(() => {
      const from = display.value;
      const start = performance.now();
      const step = (now: number) => {
        const t = Math.min((now - start) / duration, 1);
        display.value = from + (target - from) * easeOutExpo(t);
        if (t < 1) frame = requestAnimationFrame(step);
      };
      frame = requestAnimationFrame(step);
    }, delay);
  });

  onBeforeUnmount(() => {
    cancelAnimationFrame(frame);
    clearTimeout(timer);
  });
  return display;
}
