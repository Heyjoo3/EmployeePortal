<template>
  <div class="vacation-table">
    <div style="display: flex; align-items: center; justify-content: space-between; width: 100%">
      <h2 style="margin-right: 10px">Urlaubsübersicht</h2>
    </div>

    <v-card v-if="vac != null && !isloading">
      <v-data-table :headers="vacationList" :items="vac" v-if="!isloading" hover="true">
        <template #[`item.supervisorStatus`]="{ item }">
          <status-select
            :item="item"
            :relationship="props.relationship"
            :roleId="item.supervisor"
            :roleStatus="item.supervisorStatus"
            :disabled="false"
            :substituteId="0"
            @updateStatus="saveStatus"
            no-data-text="Keine Urlaube verfügbar"
          ></status-select>
        </template>

        <template #[`item.substituteStatus`]="{ item }">
          <status-select
            :item="item"
            :relationship="props.relationship"
            :roleId="item.substitute"
            :roleStatus="item.substituteStatus"
            :disabled="false"
            :substituteId="0"
            @updateStatus="saveStatus"
          ></status-select>
        </template>

        <template #[`item.adminStatus`]="{ item }">
          <status-select
            :item="item"
            :relationship="props.relationship"
            :roleId="item.substitute"
            :roleStatus="item.substituteStatus"
            :disabled="false"
            :substituteId="item.substitute"
            @updateStatus="saveAdminSubstituteStatus"
          ></status-select>
        </template>

        <template #[`item.adminStatusCheckBox`]="{ item }">
          <status-admin-checkbox
            v-if="item.status == 1 || item.status == 2 || item.status == 3"
            :item="item"
            :relationship="props.relationship"
            :true-value="1"
            @updateStatus="saveStatusAdmin"
          ></status-admin-checkbox>
          <status-admin-checkbox
            v-if="item.status == 4 || item.status == 5 || item.status == 6"
            :item="item"
            :relationship="props.relationship"
            :true-value="4"
            @updateStatus="saveStatusAdmin"
          ></status-admin-checkbox>
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
          <v-btn
            id="adminEdit-btn"
            class="ma-2"
            size="small"
            color="var(--status-error-delete)"
            icon="mdi-information"
            variant="text"
            @click="showDetailsModal(item)"
            v-if="props.relationship !== 'admin'"
          ></v-btn>
        </template>
        <template v-if="vacations.length < 10" #bottom> </template>
      </v-data-table>
      <vacation-detail-modal
        v-if="showDetails"
        :item="vacation"
        @closeDialog="showDetails = false"
      />
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
import StatusSelect from './StatusSelect.vue'
import StatusAdminCheckbox from './StatusAdminCheckbox.vue'
import VacationDetailModal from '../CreateVacationRequest/VacationDetailModal.vue'

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
      title: 'Vorgesetztenstatus',
      key: 'supervisorStatus',
      align: 'center',
      sortable: false,
      value: (leave) => `${enumToString(leave.supervisorStatus)}`
    }
  ],
  admin: [
    {
      title: 'Vertreterstatus',
      key: 'adminStatus',
      align: 'center',
      sortable: false,
      value: (leave) => `${enumToString(leave.substituteStatus)}`
    }
  ],
  substitute: [
    {
      title: 'Vertreterstatus',
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
    {
      title: 'Vom Admin bearbeitet',
      align: 'center',
      key: 'adminStatusCheckBox',
      sortable: false,
      value: 'adminStatus'
    },
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

const saveStatus = async (item) => {
  const statusMap = {
    admin: item.adminStatus,
    supervisor: item.supervisorStatus,
    substitute: item.substituteStatus
  }

  const status = statusMap[props.relationship]
  if (status >= 1 && status < 3) {
    await actions.acceptVacation(item)
  } else if (status >= 4 && status < 6) {
    await actions.cancelVacation(item)
  }
  emit('reloadVacations')
}

const saveStatusAdmin = async (item) => {
  if (item.status === 1) {
    item.adminStatus = 1
    await actions.acceptVacation(item)
  } else if (item.status === 4) {
    item.adminStatus = 4
    await actions.cancelVacation(item)
  }

  emit('reloadVacations')
}

async function saveAdminSubstituteStatus(item) {
  const status = item.substituteStatus
  if (status >= 1 && status < 3) {
    await actions.acceptVacation(item)
  } else if (status >= 4 && status < 6) {
    await actions.cancelVacation(item)
  }
  emit('reloadVacations')
}

const showDetails = ref(false)
var vacation = ref(null)

function showDetailsModal(item) {
  if (props.relationship !== 'admin') {
    vacation.value = item
    showDetails.value = true
  }
}

const vac = ref(null)
var isloading = ref(false)

onMounted(async () => {
  isloading.value = true
  var companyVacation = await actions.getCompanyVacations()
  var vacations = props.vacations.concat(companyVacation)
  vac.value = sortVacationsByStartDate(vacations)
  isloading.value = false
})
</script>

<style lang="scss" src="./EmployeeOverview.scss" scoped />
