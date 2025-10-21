<template>
  <div class="publicHolidays">
    <PublicHolidaysTable />
  </div>
  <div class="vacation-days">
    <v-btn class="text-none open-leave" @click="openDialog = true"
      ><v-icon icon="mdi-plus" start></v-icon>Urlaub beantragen</v-btn
    >
  </div>
  <div :key="vacationComponents">
    <VacationStats />
    <VacationsOverview @refreshData="refreshAll" />
    <RequestVacationDialog
      v-if="openDialog"
      @close="closeDialog"
      @submit="closeAndRefresh"
      :substitutes="substitutes"
      :supervisors="supervisors"
    />
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useVacationStore } from '@/stores/vacation-store'
import { useEmployeeStore } from '@/stores/employee-store'
import PublicHolidaysTable from './PublicHolidaysTable.vue'
import VacationStats from './VacationStats.vue'
import VacationsOverview from './VacationsOverview.vue'
import RequestVacationDialog from './RequestVacationDialog.vue'

const employeeStore = useEmployeeStore()
const vacationStore = useVacationStore()

const supervisors = ref([])
const substitutes = ref([])

const openDialog = ref(false)

const vacationComponents = ref(0)

function closeDialog() {
  openDialog.value = false
}

async function closeAndRefresh() {
  closeDialog()
  await refreshData()
  forceRender()
}

function forceRender() {
  vacationComponents.value += 1
}

async function refreshData() {
  await vacationStore.actions.getVacationsByEmployeeIdDB(employeeStore.currentEmployee.id)
  employeeStore.actions.getSupervisors(employeeStore.currentEmployee.id)
  employeeStore.actions.getSubstitutes(employeeStore.currentEmployee.id)
  supervisors.value = employeeStore.allSupervisors
  substitutes.value = employeeStore.availableSubstitutes
}

function refreshAll() {
  refreshData()
  forceRender()
}

onMounted(async () => {
  await refreshData()
})
</script>

<style lang="scss" src="./VacationDays.scss" scoped />
