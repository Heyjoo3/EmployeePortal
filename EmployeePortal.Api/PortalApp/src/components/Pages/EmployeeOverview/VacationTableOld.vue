<template>
  <div class="vacation-table">
    <div style="display: flex; align-items: center; justify-content: space-between; width: 100%">
      <h2 style="margin-right: 10px">Urlaubsübersicht</h2>
    </div>

    <v-card v-if="vac != null && !isloading">
      <v-data-table
        :headers="vacationList"
        :items="vac"
        v-if="!isloading"
        hover="true"
        no-data-text="Keine Urlaube verfügbar"
      >
        <!-- Bearbeitungsrechte -->

        <template #[`item.supervisorStatus`]="{ item }">
          <select
            v-model="item.supervisorStatus"
            class="edit-status"
            v-if="
              canEditStatus(item.supervisorStatus, item.status, item.supervisor, item.startDate)
            "
            @change="saveStatus(item)"
          >
            <option
              v-for="option in getOptions(item.supervisorStatus, item.status)"
              :value="option.value"
              :key="option.value"
            >
              {{ option.text }}
            </option>
          </select>

          <select
            class="edit-status"
            disabled
            v-else-if="
              !canEditStatus(item.supervisorStatus, item.status, item.supervisor, item.startDate)
            "
          >
            <option>Kein Handlungsbedarf</option>
          </select>
        </template>

        <template #[`item.substituteStatus`]="{ item }">
          <select
            v-model="item.substituteStatus"
            class="edit-status"
            v-if="
              canEditStatus(item.substituteStatus, item.status, item.substitute, item.startDate)
            "
            @change="saveStatus(item)"
          >
            <option
              v-for="option in getOptions(item.substituteStatus, item.status)"
              :value="option.value"
              :key="option.value"
            >
              {{ option.text }}
            </option>
          </select>

          <select
            class="edit-status"
            disabled
            v-else-if="
              !canEditStatus(item.substituteStatus, item.status, item.substitute, item.startDate)
            "
          >
            <option>Kein Handlungsbedarf</option>
          </select>
        </template>

        <template #[`item.adminStatus`]="{ item }">
          <select
            v-model="item.adminStatus"
            class="edit-status"
            v-if="
              canEditStatus(
                item.adminStatus,
                item.status,
                item.admin,
                item.startDate,
                item.substitute
              )
            "
            @change="saveStatus(item)"
          >
            <option
              v-for="option in getOptions(item.adminStatus, item.status)"
              :value="option.value"
              :key="option.value"
            >
              {{ option.text }}
            </option>
          </select>

          <select
            v-else-if="canEditDoubleRole(item)"
            class="edit-status"
            v-model="item.substituteStatus"
            @change="saveAdminSubstituteStatus(item)"
          >
            <option
              v-for="option in getOptions(item.adminStatus, item.status)"
              :value="option.value"
              :key="option.value"
            >
              {{ option.text }}
            </option>
          </select>
          <select
            class="edit-status"
            disabled
            v-else-if="!canEditStatus(item.adminStatus, item.status, item.admin, item.startDate)"
          >
            <option>Kein Handlungsbedarf</option>
          </select>
        </template>
        <template #[`item.edit`]="{ item }">
          <v-btn
            id="adminEdit-btn"
            class="ma-2"
            size="small"
            color="var(--status-error-delete)"
            icon="mdi-information"
            variant="text"
            @click="showDetailsEdit(item)"
            v-if="props.relationship == 'admin'"
          ></v-btn>
        </template>
        <template v-if="vacations.length < 10" #bottom> </template>
      </v-data-table>
    </v-card>
    <h3 v-else>Es besteht kein Handlungsbedarf</h3>
  </div>
</template>

<script setup>
import { useVacationStore } from '@/stores/vacation-store'
import { onMounted, computed, ref } from 'vue'
import { useDateConverter } from '@/Composables/useDateConverter'
import { useEmployeeStore } from '@/stores/employee-store'
import { sortVacationsByDate } from '@/Composables/sortVacationsByDate'
import { resolveVacationStatus } from '@/Composables/resolveVacationStatus'

const vacationStore = useVacationStore()
const { localizedDate } = useDateConverter()
const { actions } = vacationStore
const { sortVacationsByStartDate } = sortVacationsByDate()
const { enumToString } = resolveVacationStatus()

const employeeStore = useEmployeeStore()

const emit = defineEmits(['reloadVacations', 'openEditVacationDialog'])

const props = defineProps({
  vacations: Array,
  relationship: String
})

const baseColumns = [
  {
    title: 'Startdatum',
    key: 'startDate',
    align: 'center',
    sortable: true,
    value: (leave) => `${localizedDate(leave.startDate)}`
  },
  {
    title: 'Enddatum',
    key: 'endDate',
    align: 'center',
    sortable: false,
    value: (leave) => `${localizedDate(leave.endDate)}`
  },
  {
    title: 'Vorgesetzter',
    key: 'supervisor',
    align: 'center',
    sortable: false,
    value: (leave) => `${setName(leave.supervisor)}`
  },
  {
    title: 'Vertreter',
    key: 'substitute',
    align: 'center',
    sortable: false,
    value: (leave) => `${setName(leave.substitute)}`
  },
  { title: 'Urlaubsart', key: 'type', align: 'center', sortable: false },
  { title: 'Beschreibung', key: 'description', align: 'center', sortable: false },
  { title: 'Anzahl der Tage', key: 'vacationdays', align: 'center', sortable: false },
  {
    title: 'Status',
    key: 'status',
    align: 'center',
    sortable: false,
    value: (leave) => `${enumToString(leave.status)}`
  }
]

const roleSpecificColumns = {
  supervisor: [
    {
      title: 'Mein Status',
      key: 'supervisorStatus',
      align: 'center',
      sortable: false,
      value: (leave) => `${enumToString(leave.supervisorStatus)}`
    }
  ],
  admin: [
    {
      title: 'Mein Status',
      key: 'adminStatus',
      align: 'center',
      sortable: false,
      value: (leave) => `${enumToString(leave.adminStatus)}`
    }
  ],
  substitute: [
    {
      title: 'Mein Status',
      key: 'substituteStatus',
      align: 'center',
      sortable: false,
      value: (leave) => `${enumToString(leave.substituteStatus)}`
    }
  ]
}

const vacationList = computed(() => {
  const specificColumns = roleSpecificColumns[props.relationship] || []
  return [
    ...baseColumns,
    ...specificColumns,
    { align: 'center', key: 'edit', sortable: false, value: 'edit' }
  ]
})

const setName = (id) => {
  if (id == null) {
    return 'Kein Vertreter'
  }
  const name = employeeStore.actions.getNameById(id)
  return name
}

const showDetailsEdit = (item) => {
  if (employeeStore.currentEmployee.role === 'Admin') {
    emit('openEditVacationDialog', item)
  }
}

const canEditStatus = (relationshipStatus, status, id, startDate, substitute = null) => {
  if (employeeStore.currentEmployee.id == substitute) return false

  let editStatus =
    (id === employeeStore.currentEmployee.id || props.relationship == 'admin') &&
    new Date(startDate) >= new Date().setHours(0, 0, 0, 0) &&
    (status === 1 || // Urlaub genehmigt
      (relationshipStatus === 2 && status === 2) || // Urlaub abgelehnt von eingeloggtem User
      (status === 3 && relationshipStatus !== 0) || // Urlaubs offen und Status des eingeloggten Users ist nicht 0
      status === 4 || // Stornierung genehmigt
      (status === 5 && relationshipStatus === 5) || // Stornierung abgelehnt von eingeloggtem User
      (status === 6 && relationshipStatus !== 0 && relationshipStatus !== 1)) // Stornierung offen und Status des eingeloggten Users ist nicht 0 oder 1 (besonders wichtig für admin)

  return editStatus
}

const canEditDoubleRole = (item) => {
  let editStatus =
    item.substitute == employeeStore.currentEmployee.id &&
    ((item.status == 5 && item.adminStatus == 5) || // Stornierung abgelehnt von eingeloggtem User
      (item.status == 6 && item.adminStatus !== 0 && item.status !== 1) || // Stornierung offen und Status des eingeloggten Users ist nicht 0 oder 1 (besonders wichtig für admin)
      (item.status == 4 && item.adminStatus == 4) || // Stornierung genehmigt
      (item.status == 1 && item.adminStatus == 1) || // Urlaub genehmigt
      (item.status == 2 && item.adminStatus == 2) || // Urlaub abgelehnt von eingeloggtem User
      item.status == 3) // Urlaubs offen und Status des eingeloggten Users ist nicht 0

  return editStatus
}

const saveStatus = async (item) => {
  const statusMap = {
    admin: item.adminStatus,
    supervisor: item.supervisorStatus,
    substitute: item.substituteStatus
  }

  const status = statusMap[props.relationship]
  if (status >= 1 && status <= 3) {
    await actions.acceptVacation(item)
  } else if (status >= 4 && status <= 6) {
    await actions.cancelVacation(item)
  }
  emit('reloadVacations')
}

async function saveAdminSubstituteStatus(item) {
  item.adminStatus = item.substituteStatus

  if (item.substituteStatus >= 1 && item.substituteStatus <= 3) {
    await actions.acceptVacation(item)
  } else if (item.substituteStatus >= 4 && item.substituteStatus <= 6) {
    await actions.cancelVacation(item)
  }
  emit('reloadVacations')
}

function getOptions(specificStatus, status) {
  switch (status) {
    case 0:
      return [{ value: null, text: 'Kein Handlungsbedarf' }]
    case 1:
      return [
        { value: 1, text: 'Urlaub Genehmigt' },
        { value: 4, text: 'Stornierung Genehmigt' }
      ]
    case 2:
      return [
        { value: 1, text: 'Urlaub Genehmigt' },
        { value: 2, text: 'Urlaub Abgelehnt' },
        { value: 3, text: 'Urlaub Offen' }
      ]
    case 3:
      return [
        { value: 1, text: 'Urlaub Genehmigt' },
        { value: 2, text: 'Urlaub Abgelehnt' },
        { value: 3, text: 'Urlaub Offen' }
      ]
    case 4:
      return [{ value: 4, text: 'Stornierung Genehmigt' }]
    case 5:
      return [
        { value: 4, text: 'Stornierung Genehmigt' },
        { value: 5, text: 'Stornierung Abgelehnt' },
        { value: 6, text: 'Stornierung Offen' }
      ]
    case 6:
      return [
        { value: 4, text: 'Stornierung Genehmigt' },
        { value: 5, text: 'Stornierung Abgelehnt' },
        { value: 6, text: 'Stornierung Offen' }
      ]
    default:
      return [{ value: null, text: 'Kein Handlungsbedarf' }]
  }
}

const vac = ref(null)
var isloading = ref(false)

onMounted(async () => {
  isloading.value = true
  vac.value = props.vacations
  vac.value = sortVacationsByStartDate(vac.value)
  isloading.value = false
})
</script>

<style lang="scss" src="./EmployeeOverview.scss" scoped />
