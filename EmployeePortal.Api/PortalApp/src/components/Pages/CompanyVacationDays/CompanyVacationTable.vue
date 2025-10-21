<template>
  <v-card>
    <v-data-table
      :headers="companyVacationList"
      :items="props.companyVacations"
      hover="true"
      no-data-text="Keine Betriebsurlaube verfügbar"
    >
      <template #[`item.isHalfStartDay`]="{ item }">
        <v-icon v-if="item.isHalfStartDay">mdi-check</v-icon>
      </template>
      <template #[`item.isHalfEndDay`]="{ item }">
        <v-icon v-if="item.isHalfEndDay">mdi-check</v-icon>
      </template>
      <template #[`item.delete`]="{ item }">
        <v-btn
          class="ma-2"
          size="small"
          color="var(--status-error-delete)"
          icon="mdi-delete"
          variant="text"
          @click="$emit('deleteCompanyVacation', item.vacationId)"
        ></v-btn>
      </template>
      <template v-if="props.companyVacations.length < 10" #bottom> </template>
    </v-data-table>
  </v-card>
</template>

<script setup>
import { ref } from 'vue'
import { useDateConverter } from '@/Composables/useDateConverter'

const { localizedDate } = useDateConverter()

const props = defineProps({
  companyVacations: Array
})

const companyVacationList = ref([
  {
    title: 'Startdatum',
    key: 'startDate',
    align: 'center',
    sortable: true,
    value: (leave) => `${localizedDate(leave.startDate)}`
  },
  { title: 'Halber Tag', key: 'isHalfStartDay', align: 'center', sortable: false },
  {
    title: 'Enddatum',
    key: 'endDate',
    align: 'center',
    sortable: true,
    value: (leave) => `${localizedDate(leave.endDate)}`
  },
  { title: 'Halber Tag', key: 'isHalfEndDay', align: 'center', sortable: false },
  { title: 'Anzahl der Tage', key: 'vacationdays', align: 'center', sortable: false },
  { align: 'center', key: 'delete', sortable: false, value: 'delete' }
])
</script>
