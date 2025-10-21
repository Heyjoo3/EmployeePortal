<template>
  <div class="loading" v-if="isLoading">
    <loading-page />
  </div>

  <div class="layout" v-else>
    <v-layout>
      <v-app-bar color="var(--primary-blue)" prominent>
        <base-header />
      </v-app-bar>
      <v-navigation-drawer floating permanent>
        <side-bar />
      </v-navigation-drawer>
      <v-main style="margin-bottom: 100px">
        <router-view :key="$route.fullPath"></router-view>
      </v-main>
    </v-layout>
  </div>
</template>

<script setup>
import BaseHeader from './Header/BaseHeader.vue'
import SideBar from './SideBar/SideBar.vue'
import LoadingPage from '../Pages/LoadingPage/LoadingPage.vue'

import { useEmployeeStore } from '@/stores/employee-store.js'
import { useVacationStore } from '@/stores/vacation-store.js'
import { useAbsenceStore } from '@/stores/absence-store.js'
import { useSideNavStore } from '@/stores/side-nav-store.js'
import { onMounted, ref } from 'vue'

const employeeStore = useEmployeeStore()
const vacationStore = useVacationStore()
const absenceStore = useAbsenceStore()
const sideNavStore = useSideNavStore()

var isLoading = ref(true)

onMounted(async () => {
  isLoading.value = true
  const currentEmployee = employeeStore.currentEmployee
  vacationStore.state.currentEmployeeVacationData =
    await vacationStore.actions.getVacationsByEmployeeIdDB(currentEmployee.id)
  await absenceStore.actions.checkSickNoteDeadline(currentEmployee.id)
  await absenceStore.actions.getAbsencesByEmployeeIdDb(currentEmployee.id)
  await sideNavStore.actions.getEmployeesByDepartment()
  await sideNavStore.actions.getSupervisedAndSubstitutedEmployees(currentEmployee.id)
  await sideNavStore.actions.getTeams(currentEmployee.id)
  await employeeStore.actions.getEmployeesReduced()
  await vacationStore.actions.closeOpenVacationRequests(currentEmployee.id)
  isLoading.value = false
})
</script>
