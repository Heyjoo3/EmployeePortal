<template>
  <div :key="componentKey">
    <AddHolidayForm @add-holiday="handleAddHoliday($event)" />
    <EditHolidays @delete-holiday="handleDeleteHoliday($event)" />
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import AddHolidayForm from './AddHolidayForm.vue'
import EditHolidays from './EditHolidays.vue'
import { useVacationStore } from '@/stores/vacation-store'
import { storeToRefs } from 'pinia'

// Data and store
const vacationStore = useVacationStore()
const { actions } = vacationStore
const { allHolidayYears } = storeToRefs(vacationStore)
var componentKey = ref(0)

// Methods
const handleAddHoliday = async (holiday) => {
  await actions.createPublicHoliday(holiday)
  refreshData()
  forceReload()
}

const handleDeleteHoliday = async (id) => {
  const holidayDate = vacationStore.state.holidayObjects.find((holiday) => holiday.id === id).date
  await actions.deletePublicHoliday(id)
  await actions.updateVacationsAfterPublicHolidayDeletion(holidayDate)
  refreshData()
  forceReload()
}

function forceReload() {
  componentKey.value += 1
}

const refreshData = async () => {
  await actions.getAllPublicHolidayYears()
  await actions.getPublicHolidays(allHolidayYears)
}

onMounted(() => {
  refreshData()
})
</script>
