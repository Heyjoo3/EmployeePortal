<template>
  <v-list density="compact" nav v-model="isOpen">
    <v-list-subheader>Home</v-list-subheader>
    <v-list-item
      prepend-icon="mdi-account"
      title="Persönliche Information"
      value="basicInfo"
      @click="goToRoute('UserProfile')"
    ></v-list-item>
    <v-list-subheader>Urlaube</v-list-subheader>
    <v-list-item
      prepend-icon="mdi-label"
      title="Beantragen"
      value="vacation"
      @click="goToRoute('VacationRequests')"
    ></v-list-item>
    <v-list-item
      prepend-icon="mdi-rocket-launch-outline"
      title="Mein Onboarding"
      value="onboarding"
      @click="goToRoute('OnboardingPlan')"
    ></v-list-item>

    <admin-tabs v-if="userData.Role === 'Admin'" :allTeams="allTeams" />
    <div v-else>
      <supervised-tab v-if="supervised.length > 0" :supervised="supervised" />
      <substitude-tab v-if="substituted.length > 0" :substituted="substituted" />
      <team-overview />
    </div>
  </v-list>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { goToRoute } from '@/Composables/goToRoute'
import { useEmployeeStore } from '@/stores/employee-store.js'
import { useSideNavStore } from '@/stores/side-nav-store.js'
import { useVacationStore } from '@/stores/vacation-store.js'
import { storeToRefs } from 'pinia'
import AdminTabs from './Tabs/AdminTabs.vue'
import SupervisedTab from './Tabs/SupervisedTab.vue'
import SubstitudeTab from './Tabs/SubstitudeTab.vue'
import TeamOverview from './TeamOverview.vue'

const employeeStore = useEmployeeStore()
const sideNavStore = useSideNavStore()
const vacationStore = useVacationStore()

const { currentEmployee } = storeToRefs(employeeStore)

const isOpen = ref([])

const employeeData = currentEmployee.value

const allTeams = storeToRefs(sideNavStore).employeesByDepartment
const supervised = storeToRefs(sideNavStore).supervised
const substituted = storeToRefs(sideNavStore).substituted

const userData = computed(() => ({
  Role: employeeData.role
}))

onMounted(async () => {
  if (userData.value.Role === 'Admin') {
    for (let i = 0; i < allTeams.value.length; i++) {
      for (let j = 0; j < allTeams.value[i].employees.length; j++) {
        await vacationStore.actions.closeOpenVacationRequests(allTeams.value[i].employees[j].id)
      }
    }
  } else {
    for (let i = 0; i < supervised.value.length; i++) {
      await vacationStore.actions.closeOpenVacationRequests(supervised.value[i].id)
    }
    for (let i = 0; i < substituted.value.length; i++) {
      await vacationStore.actions.closeOpenVacationRequests(substituted.value[i].id)
    }
  }
})
</script>
