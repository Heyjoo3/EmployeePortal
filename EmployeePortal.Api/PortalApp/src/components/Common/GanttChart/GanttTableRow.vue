<template>
  <td class="name">
    <v-icon
      icon="mdi-exclamation-thick"
      color="var(--status-inactive-update)"
      v-if="employee.needsNotification"
    ></v-icon>
    {{ employee.name }}
  </td>
  <td class="stats">
    {{ annualVacation }}
  </td>

  <td class="stats">
    {{ approvedVacationsBeforeCurrentDay }}
  </td>
  <td class="stats">
    {{ vacationsAfterCurrentDay }}
  </td>

  <td class="stats">
    {{ remainingVacationDays }}
  </td>
</template>

<script setup>
import { onMounted, ref } from 'vue'

const props = defineProps({
  employee: Object,
  index: Number
})

let annualVacation = ref(props.employee.vacationStats.AnnualVacation)
let approvedVacationsBeforeCurrentDay = ref(
  props.employee.vacationStats.ApprovedVacationsBeforeCurrentDay
)
let vacationsAfterCurrentDay = ref(props.employee.vacationStats.VacationsAfterCurrentDay)
let remainingVacationDays = ref(props.employee.vacationStats.RemainingVacationDays)

onMounted(() => {
  annualVacation.value = props.employee.vacationStats.AnnualVacation
  approvedVacationsBeforeCurrentDay.value =
    props.employee.vacationStats.ApprovedVacationsBeforeCurrentDay
  vacationsAfterCurrentDay.value = props.employee.vacationStats.VacationsAfterCurrentDay
  remainingVacationDays.value = props.employee.vacationStats.RemainingVacationDays
})
</script>

<style lang="scss" src="./GanttChart.scss" scoped />
