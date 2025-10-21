<template>
  <div class="overviewPage">
    <h1 v-if="departmentName == 'substituted'">Vertrete ich</h1>
    <h1 v-else-if="departmentName == 'supervised'">Mir Unterstellt</h1>
    <h1 v-else>{{ departmentName }}</h1>

    <GanttContainer :refresh="refresh" />
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useEmployeeStore } from '@/stores/employee-store'
import GanttContainer from '../../Common/GanttChart/GanttContainer.vue'

const employeeStore = useEmployeeStore()
const departmentName = ref(employeeStore.state.pickedDepartment)
const refresh = ref(false)

watch(
  () => employeeStore.state.pickedDepartment,
  (newValue) => {
    departmentName.value = newValue
    refresh.value = !refresh.value
  }
)
</script>

<style scoped>
.overviewPage {
  width: 90%;
  margin: auto;
  margin-top: 50px;
}
</style>
