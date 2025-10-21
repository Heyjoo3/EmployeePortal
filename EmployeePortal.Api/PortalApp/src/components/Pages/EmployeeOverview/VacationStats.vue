<template>
  <div class="user-profile">
    <h2>Urlaubstage</h2>
    <i>Betriebsurlaube werden hier berechnet</i>
    <v-row v-if="employeeVacationDayData">
      <v-col>
        <div class="property-label mt-4">Urlaubstage</div>
        <div class="property-value">
          {{ employeeVacationDayData.AnnualVacation }}
        </div>
      </v-col>
      <v-col>
        <div class="property-label mt-4">Genommener Urlaub</div>
        <div class="property-value">
          {{ employeeVacationDayData.ApprovedVacationsBeforeCurrentDay }}
        </div>
      </v-col>
      <v-col>
        <div class="property-label mt-4">Geplanter Urlaub</div>
        <div class="property-value">
          {{ employeeVacationDayData.VacationsAfterCurrentDay }}
        </div>
      </v-col>
      <v-col>
        <div class="property-label mt-4">Resturlaub</div>
        <div class="property-value">
          {{ employeeVacationDayData.RemainingVacationDays }}
        </div>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { calculateVacationData } from '@/Composables/calculateVacationData'
import { onMounted } from 'vue'

const props = defineProps({
  employee: Object
})

let employeeVacationDayData = ref(null)

onMounted(async () => {
  employeeVacationDayData.value = await calculateVacationData(props.employee.id)
})
</script>

<style lang="scss" src="./EmployeeOverview.scss" scoped />
