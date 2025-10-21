<template>
  <div>
    <v-list-subheader> Vetrete ich</v-list-subheader>
    <v-list-item
      prepend-icon="mdi-account-supervisor"
      title="Übersicht"
      value="substituted"
      @click="goToRoute('OverviewPage', 'substituted', 'substitute')"
    >
    </v-list-item>
    <v-list-item
      v-for="employee in props.substituted"
      :title="employee.name"
      prepend-icon="mdi-account"
      @click="goToRoute('EmployeeOverview', employee.employeeInternalId, 'substitute', employee.id)"
      :key="employee.id"
    ></v-list-item>
  </div>
</template>

<script setup>
import { goToRoute } from '@/Composables/goToRoute'

const props = defineProps({
  substituted: Array
})
</script>
