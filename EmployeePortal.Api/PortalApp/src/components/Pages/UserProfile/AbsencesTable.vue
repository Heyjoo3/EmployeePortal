<template>
  <v-container v-if="props.absences">
    <div class="tableHead">
      <h2 class="mt-4">Meine Abwesenheiten</h2>
      <export-button class="mt-4" @click="exportAbsences(sortedAbsences, username)" />
    </div>
    <br />
    <v-data-table
      :headers="props.headers"
      :items="sortedAbsences"
      class="elevation-1"
      hover="true"
      no-data-text="Keine Abwesenheiten verfügbar"
    >
      <template #[`item.hasSickLeave`]="{ item }">
        <v-icon v-if="item.hasSickLeave" color="var(--status-active-add)">mdi-check</v-icon>
        <v-icon v-else color="var(--status-error-delete)">mdi-close</v-icon>
      </template>
      <template v-if="props.absences.length < 10" #bottom></template>
    </v-data-table>
  </v-container>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { sortAbsencesByDate } from '@/Composables/sortAbsences'
import { exportAbsences } from '@/Composables/pdf/exportAbsences'
import ExportButton from '@/components/BaseComponents/ExportButton.vue'

const { sortAbsencesByStartDate } = sortAbsencesByDate()

const props = defineProps({
  headers: Array,
  absences: Array,
  username: String
})

const sortedAbsences = ref([])

onMounted(() => {
  sortedAbsences.value = sortAbsencesByStartDate(props.absences)
})
</script>

<style scoped>
.button {
  width: 140px;
  color: var(--white);
  background-color: var(--primary-blue);
}

.tableHead {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
