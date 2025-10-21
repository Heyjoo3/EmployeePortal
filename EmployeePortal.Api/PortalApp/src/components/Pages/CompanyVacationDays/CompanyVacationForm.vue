<template>
  <v-form ref="companyVacationForm" :key="formKey">
    <v-row>
      <v-col>
        <v-date-input
          color="blue"
          placeholder="dd.mm.yyyy"
          v-model="Vacation.startDate"
          label="Startdatum"
          variant="solo-filled"
          :min="minDateStart"
          :rules="rules.startDate"
          readonly
        />
        <v-checkbox
          v-model="Vacation.isHalfStartDay"
          label="Halber Tag"
          color="var(--bold-main-blue)"
          class="half-day-checkbox"
        />
      </v-col>
      <v-col>
        <v-date-input
          color="blue"
          placeholder="dd.mm.yyyy"
          v-model="Vacation.endDate"
          label="Enddatum"
          variant="solo-filled"
          :min="minDateEnd"
          :rules="rules.endDate"
          readonly
        />
        <v-checkbox
          v-model="Vacation.isHalfEndDay"
          label="Halber Tag"
          color="var(--bold-main-blue)"
          class="half-day-checkbox"
        />
      </v-col>
    </v-row>
    <v-btn class="add-button" @click="submitCompanyVacationForm" text="Betriebsurlaub hinzufügen" />
  </v-form>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { useVacationStore } from '@/stores/vacation-store'
import { useDateConverter } from '@/Composables/useDateConverter'

const { convertToGermanISO } = useDateConverter()
const vacationStore = useVacationStore()
const { actions } = vacationStore

const emit = defineEmits(['updateCompanyVacations'])

const formKey = ref(0)
const companyVacationForm = ref(null)

const Vacation = ref({
  startDate: null,
  endDate: null,
  isHalfStartDay: false,
  isHalfEndDay: false,
  type: 'Betriebsurlaub',
  vacationdays: 0,
  supervisor: null,
  substitute: null,
  Description: null,
  Status: 1,
  adminStatus: 1,
  supervisorStatus: 1,
  substituteStatus: 1,
  employeeId: null
})

const minDateStart = ref(new Date().toISOString().substring(0, 10))

const minDateEnd = computed(() => {
  if (!Vacation.value.startDate) {
    return minDateStart.value
  }
  const date = new Date(Vacation.value.startDate)
  date.setDate(date.getDate() + 1)
  return date.toISOString().substring(0, 10)
})

watch(
  () => [Vacation.value.startDate, Vacation.value.endDate],
  ([newStartDate, newEndDate]) => {
    if (newEndDate && newStartDate) {
      const endDate = new Date(newEndDate)
      const startDate = new Date(newStartDate)
      if (endDate < startDate) {
        Vacation.value.startDate = new Date(newEndDate)
      }
    }
  },
  { deep: true }
)

const rules = {
  startDate: [(v) => !!v || 'Startdatum darf nicht leer sein'],
  endDate: [(v) => !!v || 'Enddatum darf nicht leer sein']
}

const submitCompanyVacationForm = async () => {
  const isValid = await companyVacationForm.value.validate()

  if (!isValid.valid) {
    resetForm()
    return
  }

  Vacation.value.startDate = convertToGermanISO(Vacation.value.startDate)
  Vacation.value.endDate = convertToGermanISO(Vacation.value.endDate)
  const vacationExists = actions.checkExistingCompanyVacation(Vacation.value)

  if (vacationExists) {
    alert('Betriebsurlaub überlappt sich mit bestehendem Betriebsurlaub!')
    resetForm()
    return
  }

  Vacation.value.vacationdays = 0
  await actions.createVacation(Vacation)
  await actions.checkCompanyVacationOverlaps(Vacation.value)
  emit('updateCompanyVacations')

  resetForm()
}

const resetForm = () => {
  Vacation.value = {
    startDate: null,
    endDate: null,
    isHalfStartDay: false,
    isHalfEndDay: false,
    type: 'Betriebsurlaub',
    vacationdays: 0,
    supervisor: null,
    substitute: null,
    Description: null,
    Status: 1,
    adminStatus: 1,
    supervisorStatus: 1,
    substituteStatus: 1,
    employeeId: null
  }
  formKey.value++
}
</script>

<style lang="scss" src="./CompanyVacation.scss" scoped />
