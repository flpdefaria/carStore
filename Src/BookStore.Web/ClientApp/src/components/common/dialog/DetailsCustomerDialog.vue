<script setup lang="ts">
import Dialog from "primevue/dialog";
import Button from "primevue/button";
import DetailField from "../form/DetailField.vue";
import { detailsDialogPt, dialogSecondaryButtonClass } from "./dialogStyles";
import { formatDate } from "../../../utils/format";
import type { CustomerDto } from "../../../types";

defineProps<{
  visible: boolean;
  customer: CustomerDto | null;
}>();

const emit = defineEmits<{ "update:visible": [value: boolean] }>();

const dialogPt = detailsDialogPt("w-[480px]");
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

    <template v-if="customer">
      <DetailField label="Full name">
        <p class="text-sm font-medium text-color">{{ customer.fullName }}</p>
      </DetailField>

      <DetailField label="Email">
        <p class="text-sm font-medium text-color">{{ customer.email }}</p>
      </DetailField>

      <DetailField label="Phone number">
        <p class="text-sm font-medium text-color">{{ customer.phoneNumber ?? "-" }}</p>
      </DetailField>

      <DetailField label="Created at">
        <p class="text-sm font-medium text-color">{{ formatDate(customer.createdAt) }}</p>
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
