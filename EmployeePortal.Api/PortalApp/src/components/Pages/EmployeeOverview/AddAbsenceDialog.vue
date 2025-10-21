<template>
  <BaseModal
    :dialog="addDialog"
    title="Abwesenheit hinzufügen"
    bannerIcon="mdi-plus"
    bannerText="Hinzufügen"
    :formMode="true"
    text-color="var(--dark-text)"
    @close="closeDialog"
    @submit="submitAddForm"
  >
    <template #form-content>
      <v-form class="add-form" ref="form">
        <v-row>
          <v-col>
            <v-row>
              <v-col>
                <v-date-input
                  color="var(--dark-text)"
                  placeholder="dd.mm.yyyy"
                  v-model="absenceData.startDate"
                  label="Startdatum"
                  variant="outlined"
                  :rules="[rules.required]"
                  :error-messages="errorMessages"
                  readonly
                ></v-date-input>
              </v-col>
              <v-col>
                <v-date-input
                  color="var(--dark-text)"
                  placeholder="dd.mm.yyyy"
                  v-model="absenceData.endDate"
                  label="Enddatum"
                  variant="outlined"
                  :rules="[rules.required]"
                  :error-messages="errorMessages"
                  :min="minEndDate"
                  readonly
                ></v-date-input>
              </v-col>
            </v-row>
            <v-row>
              <v-text-field
                class="text-input"
                placeholder=""
                v-model="absenceData.remarks"
                label="Bemerkungen"
                variant="outlined"
                :rules="[rules.remarks]"
                :error-messages="errorMessages"
              >
              </v-text-field>
            </v-row>
          </v-col>
          <v-col-auto class="ml-15">
            <v-select
              v-model="absenceData.absenceType"
              :items="absenceTypes"
              label="Grund"
              variant="outlined"
              :rules="[rules.required]"
              :error-messages="errorMessages"
            ></v-select>
            <v-checkbox
              class="checkbox"
              v-model="absenceData.hasSickLeave"
              label="AU vorhanden"
              v-if="absenceData.absenceType === 'Krankheit'"
            >
            </v-checkbox>
            <v-checkbox
              class="checkbox"
              v-model="absenceData.hasStartedShift"
              label="Nach Arbeitsantritt"
            ></v-checkbox>
          </v-col-auto>
        </v-row>
      </v-form>
    </template>
  </BaseModal>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useAbsenceStore } from '@/stores/absence-store'
import { useVacationStore } from '@/stores/vacation-store'
import { useDateConverter } from '@/Composables/useDateConverter'
import { resolveAbsenceType } from '@/Composables/resolveAbsenceType'
import { calculateSickDays } from '@/Composables/calculateSickDays'
import BaseModal from '../../BaseComponents/BaseModal.vue'

const vacationStore = useVacationStore()
const absenceStore = useAbsenceStore()
const { convertToGermanISO } = useDateConverter()
const { absenceTypes, stringToEnum } = resolveAbsenceType()

const emit = defineEmits(['closeDialog', 'reloadAbsences'])

const props = defineProps({
  employee: Object
})

const rules = {
  required: (value) => !!value || 'Eingabe erforderlich',
  remarks: (value) => value == null || value.length <= 500 || 'Maximal 500 Zeichen'
}

var minEndDate = computed(() => {
  if (absenceData.value.startDate === null) {
    return new Date().toISOString().substring(0, 10)
  }
  const date = new Date(absenceData.value.startDate)
  date.setDate(date.getDate() + 1)
  return new Date(date).toISOString().substring(0, 10)
})

const absenceData = ref({
  startDate: null,
  endDate: null,
  hasSickLeave: false,
  hasStartedShift: false,
  remarks: '',
  duration: 0,
  employeeId: props.employee.id,
  absenceType: null
})

const form = ref(null)

function closeDialog() {
  emit('closeDialog')
}

async function submitAddForm() {
  const validate = await form.value.validate()

  if (validate.valid) {
    if (absenceData.value.startDate > absenceData.value.endDate) {
      return
    }

    absenceData.value.startDate = convertToGermanISO(absenceData.value.startDate)
    absenceData.value.endDate = convertToGermanISO(absenceData.value.endDate)
    absenceData.value.duration = calculateSickDays(absenceData.value)
    absenceData.value.absenceType = stringToEnum(absenceData.value.absenceType)

    if (absenceData.value.hasSickLeave === false) {
      absenceData.value.hasSickLeave = null
    }

    const existsAlready = await absenceStore.actions.checkExistingAbsence(absenceData.value)

    if (existsAlready) {
      alert('Es existiert bereits eine Abwesenheit für diesen Zeitraum')
      absenceData.value.startDate = null
      absenceData.value.endDate = null
      return
    }

    const response = await absenceStore.actions.createAbsence(absenceData.value)

    if (response.data.isSuccessfull) {
      if (absenceData.value.absenceType == 2 && absenceData.value.hasSickLeave === true) {
        const vacations = await vacationStore.actions.getVacationsByEmployeeIdDB(
          absenceData.value.employeeId
        )

        for (let i = 0; i < vacations.length; i++) {
          let vacation = vacations[i]
          if (
            (absenceData.value.startDate >= vacation.startDate &&
              absenceData.value.startDate <= vacation.endDate) ||
            (absenceData.value.endDate >= vacation.startDate &&
              absenceData.value.endDate <= vacation.endDate) ||
            (absenceData.value.startDate <= vacation.startDate &&
              absenceData.value.endDate < vacation.endDate) ||
            (absenceData.value.startDate > vacation.startDate &&
              absenceData.value.endDate >= vacation.endDate)
          ) {
            const response = await vacationStore.actions.editVacation(vacation)

            if (response.isSuccessfull) {
              absenceData.value.isCredited = true
            }
          } else {
            absenceData.value.isCredited = false
          }
        }
      }
    }

    absenceData.value.startDate = null
    absenceData.value.endDate = null
    absenceData.value.hasSickLeave = false
    absenceData.value.hasStartedShift = false
    absenceData.value.remarks = ''
    absenceData.value.duration = 0
    absenceData.value.absenceType = null

    emit('reloadAbsences')
  }
}
</script>

<style lang="scss" src="./EmployeeOverview.scss" scoped />
