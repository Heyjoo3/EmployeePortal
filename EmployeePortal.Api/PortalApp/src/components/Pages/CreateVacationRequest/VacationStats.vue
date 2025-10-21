<template>
  <div class="vacation-days">
    <h2>Urlaubstage</h2>
    <i>Betriebsurlaube werden hier mit einberechnet</i>
    <v-row>
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
import { useEmployeeStore } from '@/stores/employee-store'
import { calculateVacationData } from '@/Composables/calculateVacationData'

const employeeStore = useEmployeeStore()
const currentEmployee = ref(employeeStore.currentEmployee)

const employeeVacationDayData = await calculateVacationData(currentEmployee.value.id)
</script>

<style lang="scss" src="./VacationDays.scss" scoped />
