<template>
  <v-container v-if="requests">
    <h2 class="mt-4">Offene Anfragen an mich</h2>
    <br />

    <v-data-table
      :headers="headers"
      :items="requests"
      class="elevation-1"
      hover="true"
      @click:row="handleClick"
      no-data-text="Keine offenen Anfragen verfügbar"
    >
      <template v-if="requests.length < 10" #bottom></template>
    </v-data-table>
  </v-container>
</template>

<script setup>
import { goToRoute } from '@/Composables/goToRoute'
import { useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { useVacationStore } from '@/stores/vacation-store'
import { useEmployeeStore } from '@/stores/employee-store'
import { useDateConverter } from '@/Composables/useDateConverter'

const router = useRouter()
const vacationStore = useVacationStore()
const employeeStore = useEmployeeStore()
const { localizedDate } = useDateConverter()

const { currentEmployee } = employeeStore

const headers = ref([
  { title: 'Vorname', value: 'employeeFirstName', key: 'employeeFirstName' },
  { title: 'Nachname', value: 'employeeLastName', key: 'employeeLastName' },
  { title: 'Startdatum', key: 'startDate', value: (item) => `${localizedDate(item.startDate)}` },
  { title: 'Enddatum', key: 'endDate', value: (item) => `${localizedDate(item.endDate)}` },
  { title: 'Typ', key: 'status', value: (item) => `${getType(item.vacationStatus)}` }
])

const requests = ref([])

const getType = (status) => {
  if (1 <= status && status <= 3) {
    return 'Urlaub'
  } else if (4 <= status && status <= 6) {
    return 'Stornierung'
  }
}

const handleClick = (e, { item }) => {
  if (item.relation == 'ownRequest') {
    router.push({ name: 'VacationRequests' })
  } else {
    goToRoute('EmployeeOverview', item.employeeInternalId, item.relation, item.employeeId)
  }
}

onMounted(async () => {
  const response = await vacationStore.actions.getOpenRequestWithEmployee(
    currentEmployee.id,
    currentEmployee.role
  )

  requests.value = response.data
})
</script>
