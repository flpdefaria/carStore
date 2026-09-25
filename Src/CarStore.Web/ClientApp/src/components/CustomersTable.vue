<script setup lang="ts">
import { onMounted, ref } from "vue";
import { type DataTablePageEvent } from "primevue/datatable";
import Message from "primevue/message";
import DataTableCommon, { type DataTableColumn } from "./common/table/DataTableCommon.vue";
import ConfirmDeleteDialog from "./common/dialog/ConfirmDeleteDialog.vue";
import EditCustomerDialog, { type EditCustomerPayload } from "./common/dialog/EditCustomerDialog.vue";
import DetailsCustomerDialog from "./common/dialog/DetailsCustomerDialog.vue";
import { usePagedFetch } from "../composables/usePagedFetch";
import { useEntityCrud } from "../composables/useEntityCrud";
import { formatDate } from "../utils/format";
import type { CustomerDto } from "../types";

const props = defineProps<{
  apiUrl: string;
}>();

const rows = ref(10);
const first = ref(0);
const { items: customers, totalRecords, loading, error, load } = usePagedFetch<CustomerDto>(props.apiUrl);

const columns: DataTableColumn[] = [
  { field: "fullName", header: "Full name", primary: true },
  { field: "email", header: "Email" },
  { field: "phoneNumber", header: "Phone number" },
  { field: "createdAt", header: "Created at" },
];

function onPage(event: DataTablePageEvent) {
  first.value = event.first;
  rows.value = event.rows;
  load(event.page + 1, event.rows);
}

const {
  deleteDialogVisible,
  deleteTarget,
  deleteLoading,
  deleteError,
  onDeleteRequest,
  onDeleteConfirm,
  bulkDeleteDialogVisible,
  bulkDeleteTargets,
  bulkDeleteLoading,
  bulkDeleteError,
  onBulkDeleteRequest,
  onBulkDeleteConfirm,
  editDialogVisible,
  editTarget,
  editLoading,
  editError,
  onEditRequest,
  onEditSubmit,
  detailsDialogVisible,
  detailsTarget,
  onDetailsRequest,
} = useEntityCrud<CustomerDto, EditCustomerPayload>({
  apiUrl: props.apiUrl,
  entityLabel: "customer",
  reload: () => load(Math.floor(first.value / rows.value) + 1, rows.value),
});

onMounted(() => load(1, rows.value));

defineExpose({ reload: () => load(1, rows.value) });
</script>

<template>
  <Message v-if="error" severity="error" :closable="false" class="mb-4">{{ error }}</Message>
  <DataTableCommon
    :value="customers"
    :columns="columns"
    :loading="loading"
    :total-records="totalRecords"
    :first="first"
    :rows="rows"
    confirm-details
    confirm-delete
    confirm-edit
    bulk-delete
    @page="onPage"
    @delete="onDeleteRequest"
    @edit="onEditRequest"
    @details="onDetailsRequest"
    @bulk-delete="onBulkDeleteRequest"
  >
    <template #col-createdAt="{ data }">
      <span class="text-xs text-muted-color">{{ formatDate(data.createdAt) }}</span>
    </template>
  </DataTableCommon>

  <ConfirmDeleteDialog
    v-model:visible="deleteDialogVisible"
    title="Delete Customer"
    message="Are you sure you want to delete this customer?"
    :details="deleteTarget ? [{ label: 'Full name', value: deleteTarget.fullName }, { label: 'Email', value: deleteTarget.email }] : []"
    :loading="deleteLoading"
    :error="deleteError"
    @confirm="onDeleteConfirm"
  />

  <ConfirmDeleteDialog
    v-model:visible="bulkDeleteDialogVisible"
    title="Delete Customers"
    :message="`Are you sure you want to delete ${bulkDeleteTargets.length} customer(s)?`"
    :details="bulkDeleteTargets.map((customer) => ({ label: customer.fullName, value: customer.email }))"
    :loading="bulkDeleteLoading"
    :error="bulkDeleteError"
    @confirm="onBulkDeleteConfirm"
  />

  <EditCustomerDialog
    v-model:visible="editDialogVisible"
    :customer="editTarget"
    :loading="editLoading"
    :error="editError"
    @submit="onEditSubmit"
  />

  <DetailsCustomerDialog v-model:visible="detailsDialogVisible" :customer="detailsTarget" />
</template>

