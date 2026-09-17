<script setup lang="ts">
import Dialog from "primevue/dialog";
import Button from "primevue/button";
import DetailField from "../form/DetailField.vue";
import { detailsDialogPt, dialogSecondaryButtonClass } from "./dialogStyles";
import type { BrandDto } from "../../../types";

defineProps<{
  visible: boolean;
  brand: BrandDto | null;
}>();

const emit = defineEmits<{ "update:visible": [value: boolean] }>();

// API dates are ISO strings ("yyyy-MM-ddTHH:mm:ss"); slicing avoids a local-timezone shift.
function formatIsoDate(value: string) {
  return value.slice(0, 10);
}

const dialogPt = detailsDialogPt("w-[520px]");
</script>

<template>
  <Dialog
    :visible="visible"
    @update:visible="(value: boolean) => emit('update:visible', value)"
    modal
    dismissable-mask
    :draggable="false"
    :pt="dialogPt"
  >
    <template #header>
      <span class="text-[21px] font-bold text-color">Details</span>
    </template>

    <template v-if="brand">
      <DetailField label="Name">
        <p class="text-sm font-medium text-color">{{ brand.name }}</p>
      </DetailField>

      <div class="h-px w-full bg-surface-300 my-3.5" />

      <div class="flex w-full items-start gap-1.75">
        <DetailField label="Country">
          <p class="text-sm font-medium text-color">{{ brand.country }}</p>
        </DetailField>
        <DetailField label="Years in business">
          <p class="text-sm font-medium text-color">{{ brand.yearsInBusiness }}</p>
        </DetailField>
      </div>

      <div class="flex w-full items-start gap-1.75">
        <DetailField label="Founded date">
          <p class="text-sm font-medium text-color">{{ formatIsoDate(brand.foundedDate) }}</p>
        </DetailField>
        <DetailField label="Number of cars">
          <p class="text-sm font-medium text-color">{{ brand.carsCount }}</p>
        </DetailField>
      </div>

      <DetailField label="Cars">
        <ul v-if="brand.cars.length" class="ml-[21px] list-disc text-sm font-medium text-color">
          <li v-for="car in brand.cars" :key="car.model">
            {{ car.model }} <span class="text-muted-color">({{ car.modelYear }})</span>
          </li>
        </ul>
        <p v-else class="text-sm font-medium text-muted-color">No cars yet.</p>
      </DetailField>

      <div class="h-px w-full bg-surface-300 my-3.5" />

      <DetailField label="Description">
        <p class="text-sm font-medium text-color">{{ brand.description }}</p>
      </DetailField>
    </template>

    <template #footer>
      <Button
        label="Close"
        severity="secondary"
        outlined
        :class="dialogSecondaryButtonClass"
        @click="emit('update:visible', false)"
      />
    </template>
  </Dialog>
</template>
