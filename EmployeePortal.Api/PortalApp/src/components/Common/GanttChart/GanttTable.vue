<template>
  <div class="gantt-table">
    <table>
      <thead>
        <tr>
          <th class="name-header">Teammitglied</th>
          <th class="stats-header">Urlaub</th>
          <th class="stats-header">Genommen</th>
          <th class="stats-header">Geplant</th>
          <th class="stats-header">Übrig</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="(employee, index) in props.teamData"
          :key="employee.name"
          class="cell"
          @click="goToRoute('EmployeeOverview', employee.employeeInternalId, relation, employee.id)"
        >
          <gantt-table-row :employee="employee" :index="index" />
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import GanttTableRow from './GanttTableRow.vue'
import { useEmployeeStore } from '@/stores/employee-store'
import { goToRoute } from '@/Composables/goToRoute'

const props = defineProps({
  teamData: Array
})

const employeeStore = useEmployeeStore()

const relation = employeeStore.state.pickedRelation
</script>

<style lang="scss" src="./GanttChart.scss" scoped />
