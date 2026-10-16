import { reactive } from "vue";

export type RowEffect = "created" | "updated" | "leaving";

// Short-lived per-row animation flags, keyed by "<apiUrl>:<id>" so each entity table only picks up
// its own rows. The CRUD composables set them; DataTableCommon turns them into `row-fx-*` classes.
const effects = reactive(new Map<string, RowEffect>());

const key = (scope: string, id: unknown) => `${scope}:${id}`;

export function flagRows(scope: string, ids: unknown[], effect: RowEffect, duration: number) {
  for (const id of ids) effects.set(key(scope, id), effect);
  setTimeout(() => {
    for (const id of ids) if (effects.get(key(scope, id)) === effect) effects.delete(key(scope, id));
  }, duration);
}

export function rowEffectClass(scope: string | undefined, id: unknown): string | undefined {
  if (!scope) return undefined;
  const effect = effects.get(key(scope, id));
  return effect ? `row-fx-${effect}` : undefined;
}

/** Plays the exit on the given rows before the caller reloads them away. */
export function playRowExit(scope: string, ids: unknown[]): Promise<void> {
  flagRows(scope, ids, "leaving", 2000);
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  return new Promise((resolve) => setTimeout(resolve, reduceMotion ? 250 : 380));
}
