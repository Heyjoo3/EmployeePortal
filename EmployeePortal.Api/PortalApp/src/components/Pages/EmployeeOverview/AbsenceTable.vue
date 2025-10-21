<template>
  <div class="absence-table" v-if="isLoaded">
    <div class="absence-title" v-if="props.employee">
      <h2>Abwesenheiten</h2>
      <i
        >Ab dem {{ props.employee.sickNoteDeadLine }}. Tag ist eine AU im Krankheitsfall
        erforderlich.</i
      >
    </div>

    <v-card v-if="allAbsences != null">
      <v-data-table
        :headers="absenceList"
        :items="sortedAbsences"
        hover="true"
        no-data-text="Keine Abwesenheiten verfügbar"
      >
        <template #[`item.hasSickLeave`]="{ item }">
          <v-icon v-if="item.hasSickLeave" color="var(--status-active-add)">mdi-check</v-icon>
          <!-- Type 2 means sick leave -->
          <v-icon
            v-else-if="!item.hasSickLeave && item.absenceType == 2"
            color="var(--status-error-delete)"
            >mdi-close</v-icon
          >
          <v-icon v-else color="white">mdi-close</v-icon>
        </template>

        <template #[`item.edit`]="{ item }" v-if="relationship == 'admin'">
          <v-btn
            @click="editAbsence(item)"
            id="edit-btn"
            class="ma-2"
            size="small"
            color="var(--status-inactive-update)"
            icon="mdi-pencil"
            variant="text"
          >
          </v-btn>
          <v-btn
            @click="deleteAbsence(item)"
            id="delete-btn"
            class="ma-2"
            size="small"
            color="var(--status-error-delete)"
            icon="mdi-delete"
            variant="text"
          >
          </v-btn>
        </template>
        <template v-if="props.absences.length < 10" #bottom> </template>
      </v-data-table>
    </v-card>
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import { useDateConverter } from '@/Composables/useDateConverter'
import { resolveAbsenceType } from '@/Composables/resolveAbsenceType'
import { useAbsenceStore } from '@/stores/absence-store'
import { useVacationStore } from '@/stores/vacation-store'
import { useEmployeeStore } from '@/stores/employee-store'
import { sortAbsencesByDate } from '@/Composables/sortAbsences'

const props = defineProps({
  employee: Object,
  relationship: Object,
  absences: Array
})

const emit = defineEmits(['reloadAbsences', 'openEditAbsenceDialog'])

const absenceStore = useAbsenceStore()
const vacationStore = useVacationStore()
const employeeStore = useEmployeeStore()

const { localizedDate } = useDateConverter()
const { enumToString } = resolveAbsenceType()
const { sortAbsencesByStartDate } = sortAbsencesByDate()

const loggedUserRole = ref('')

const baseList = [
  {
    title: 'Startdatum',
    key: 'startDate',
    align: 'center',
    sortable: true,
    value: (item) => `${localizedDate(item.startDate)}`
  },
  {
    title: 'Enddatum',
    key: 'endDate',
    align: 'center',
    sortable: true,
    value: (item) => `${localizedDate(item.endDate)}`
  },
  { title: 'Anzahl der Tage', key: 'duration', align: 'center', sortable: false }
]

const absenceList = ref([])

const allAbsences = ref(props.absences)
const sortedAbsences = ref(sortAbsencesByStartDate(allAbsences.value))

watch(
  () => props.absences,
  (newValue) => {
    allAbsences.value = newValue
    sortedAbsences.value = sortAbsencesByStartDate(allAbsences.value)
  }
)

function editAbsence(item) {
  emit('openEditAbsenceDialog', item)
}

async function deleteAbsence(item) {
  const absenceVacationOverlap = await vacationStore.actions.checkForAbsenceOverlaps(item)

  await absenceStore.actions.deleteAbsence(item.absenceId)

  for (const vacation of absenceVacationOverlap) {
    if (
      vacation.description !== null &&
      vacation.description.includes('Krankheit verkürzt Urlaubsdauer')
    ) {
      vacation.description = vacation.description.replace('Krankheit verkürzt Urlaubsdauer', '')
    }
    await vacationStore.actions.editVacation(vacation)
  }

  emit('reloadAbsences')
}

const isLoaded = ref(false)

onMounted(() => {
  loggedUserRole.value = employeeStore.currentEmployee.role

  absenceList.value = [
    ...baseList,
    ...(loggedUserRole.value !== 'Employee'
      ? [
          {
            title: 'Art',
            key: 'absenceType',
            align: 'center',
            sortable: true,
            value: (item) => `${enumToString(item.absenceType)}`
          },
          { title: 'AU vorhanden', key: 'hasSickLeave', align: 'center', sortable: false },
          { title: 'Bemerkungen', key: 'remarks', align: 'center', sortable: false },
          { align: 'center', key: 'edit', sortable: false, value: 'edit' }
        ]
      : [])
  ]

  isLoaded.value = true
})
</script>
