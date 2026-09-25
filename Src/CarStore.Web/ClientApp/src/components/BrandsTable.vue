<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { type DataTablePageEvent } from "primevue/datatable";
import Message from "primevue/message";
import DataTableCommon, { type DataTableColumn } from "./common/table/DataTableCommon.vue";
import ConfirmDeleteDialog from "./common/dialog/ConfirmDeleteDialog.vue";
import EditBrandDialog, { type EditBrandPayload } from "./common/dialog/EditBrandDialog.vue";
import DetailsBrandDialog from "./common/dialog/DetailsBrandDialog.vue";
import { usePagedFetch } from "../composables/usePagedFetch";
import { useEntityCrud } from "../composables/useEntityCrud";
import { formatDate } from "../utils/format";
import type { BrandDto } from "../types";

const props = defineProps<{
  apiUrl: string;
}>();

const rows = ref(10);
const first = ref(0);
const { items: brands, totalRecords, loading, error, load } = usePagedFetch<BrandDto>(props.apiUrl);

const columns: DataTableColumn[] = [
  { field: "name", header: "Name", primary: true },
  { field: "country", header: "Country" },
  { field: "foundedDate", header: "Founded" },
  { field: "yearsInBusiness", header: "Years in business" },
  { field: "carsCount", header: "Cars" },
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
} = useEntityCrud<BrandDto, EditBrandPayload>({
  apiUrl: props.apiUrl,
  entityLabel: "brand",
  reload: () => load(Math.floor(first.value / rows.value) + 1, rows.value),
});

const bulkDeleteBlockedBrands = computed(() => bulkDeleteTargets.value.filter((brand) => brand.carsCount > 0));

onMounted(() => load(1, rows.value));

defineExpose({ reload: () => load(1, rows.value) });
</script>


<template>
  <Message v-if="error" severity="error" :closable="false" class="mb-4">{{ error }}</Message>
  <DataTableCommon
    :value="brands"
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
    <template #col-foundedDate="{ data }">
      <span class="text-xs text-muted-color">{{ formatDate(data.foundedDate) }}</span>
    </template>
  </DataTableCommon>

  <ConfirmDeleteDialog
    v-model:visible="deleteDialogVisible"
    title="Delete Brand"
    message="Are you sure you want to delete this brand?"
    :details="
      deleteTarget
        ? [
            { label: 'Name', value: deleteTarget.name },
            { label: 'Country', value: deleteTarget.country },
            { label: 'Cars', value: String(deleteTarget.carsCount) },
          ]
        : []
    "
    :confirm-disabled="!!deleteTarget && deleteTarget.carsCount > 0"
    :warning="
      deleteTarget && deleteTarget.carsCount > 0
        ? `This brand has ${deleteTarget.carsCount} car(s) registered. Remove or reassign them before deleting the brand.`
        : null
    "
    :loading="deleteLoading"
    :error="deleteError"
    @confirm="onDeleteConfirm"
  />

  <ConfirmDeleteDialog
    v-model:visible="bulkDeleteDialogVisible"
    title="Delete Brands"
    :message="`Are you sure you want to delete ${bulkDeleteTargets.length} brand(s)?`"
    :details="bulkDeleteTargets.map((brand) => ({ label: brand.name, value: `${brand.carsCount} car(s)` }))"
    :confirm-disabled="bulkDeleteBlockedBrands.length > 0"
    :warning="
      bulkDeleteBlockedBrands.length > 0
        ? `${bulkDeleteBlockedBrands.map((brand) => brand.name).join(', ')} still ${bulkDeleteBlockedBrands.length === 1 ? 'has' : 'have'} cars registered. Remove or reassign them before deleting.`
        : null
    "
    :loading="bulkDeleteLoading"
    :error="bulkDeleteError"
    @confirm="onBulkDeleteConfirm"
  />

  <EditBrandDialog
    v-model:visible="editDialogVisible"
    :brand="editTarget"
    :loading="editLoading"
    :error="editError"
    @submit="onEditSubmit"
  />

  <DetailsBrandDialog v-model:visible="detailsDialogVisible" :brand="detailsTarget" />
</template>

