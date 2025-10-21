<template>
  <div>
    <v-list-subheader>Teams</v-list-subheader>
    <v-list-group v-for="item in teams" :key="item.department">
      <template v-slot:activator="{ props }">
        <v-list-item
          v-bind="props"
          prepend-icon="mdi-account-group"
          :title="item.department"
          :value="item.department"
        ></v-list-item>
      </template>
      <v-list-item
        title="Übersicht"
        :value="item.department"
        @click="goToRoute('OverviewPage', item.department)"
      ></v-list-item>
      <v-list-item
        v-for="employee in item.employees"
        :title="employee.name"
        @click="
          goToRoute(
            'EmployeeOverview',
            employee.employeeInternalId,
            currentEmployee.role,
            employee.id
          )
        "
        :key="employee.employeeInternalId"
      ></v-list-item>
    </v-list-group>
  </div>
</template>

<script setup>
import { goToRoute } from '@/Composables/goToRoute'
import { useSideNavStore } from '@/stores/side-nav-store.js'
import { useEmployeeStore } from '@/stores/employee-store'
import { storeToRefs } from 'pinia'

const sideNavStore = useSideNavStore()
const employeeStore = useEmployeeStore()

const teams = storeToRefs(sideNavStore).teams
const currentEmployee = storeToRefs(employeeStore).currentEmployee
</script>
