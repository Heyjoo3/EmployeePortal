<template>
  <BaseModal
    :title="`Abwesenheit bearbeiten`"
    :bannerIcon="`mdi-plus`"
    :bannerText="`Bearbeiten`"
    bannerBackgroundColor="var(--status-inactive-update)"
    :saveButtonText="`Speichern`"
    :formMode="true"
    @close="closeDialog"
    @submit="submitEditForm"
  >
    <template #form-content>
      <v-form class="add-form" ref="editForm">
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
              label="Art"
              variant="outlined"
              :rules="[rules.required]"
              :error-messages="errorMessages"
            ></v-select>
            <v-checkbox
              class="checkbox"
              v-model="absenceData.hasSickLeave"
              label="AU vorhanden"
              v-if="absenceData.absenceType == 'Krankheit'"
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
import { onMounted, ref } from 'vue'
import { useAbsenceStore } from '@/stores/absence-store'
import { useVacationStore } from '@/stores/vacation-store'
import { useDateConverter } from '@/Composables/useDateConverter'
import { resolveAbsenceType } from '@/Composables/resolveAbsenceType'
import { calculateSickDays } from '@/Composables/calculateSickDays'
import BaseModal from '../../BaseComponents/BaseModal.vue'

const absenceStore = useAbsenceStore()
const vacationStore = useVacationStore()
const { convertToGermanISO } = useDateConverter()
const { absenceTypes, stringToEnum, enumToString } = resolveAbsenceType()

const props = defineProps({
  absence: Object
})

const emit = defineEmits(['closeDialog', 'reloadAbsences'])

const editForm = ref(null)

const absenceData = ref({
  startDate: new Date(props.absence.startDate),
  endDate: new Date(props.absence.endDate),
  remarks: props.absence.remarks,
  absenceType: enumToString(props.absence.absenceType),
  hasSickLeave: props.absence.hasSickLeave,
  hasStartedShift: props.absence.hasStartedShift,
  ...props.absence
})

const errorMessages = ref('')
const minEndDate = ref('')
const rules = {
  required: (value) => !!value || 'Eingabe erforderlich',
  remarks: (value) => value == null || value.length <= 500 || 'Maximal 500 Zeichen'
}

function closeDialog() {
  emit('closeDialog')
}

async function submitEditForm() {
  const validate = await editForm.value.validate()

  if (!validate.valid) {
    return
  }

  if (absenceData.value.startDate > absenceData.value.endDate) {
    return
  }

  absenceData.value.startDate = convertToGermanISO(absenceData.value.startDate)
  absenceData.value.endDate = convertToGermanISO(absenceData.value.endDate)
  absenceData.value.duration = calculateSickDays(absenceData.value)
  absenceData.value.absenceType = stringToEnum(absenceData.value.absenceType)

  const existsAlready = await absenceStore.actions.checkExistingAbsence(absenceData.value)

  if (existsAlready) {
    alert('Es existiert bereits eine Abwesenheit für diesen Zeitraum')
    absenceData.value.startDate = null
    absenceData.value.endDate = null
    return
  }

  const response = await absenceStore.actions.updateAbsence(absenceData.value)

  if (response.isSuccessfull) {
    if (absenceData.value.absenceType === 2) {
      const vacations = await vacationStore.actions.getVacationsByEmployeeIdDB(
        absenceData.value.employeeId
      )

      if (absenceData.value.hasSickLeave) {
        if (!props.absence.isCredited) {
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
              await vacationStore.actions.editVacation(vacation)
            }
          }
          absenceData.value.isCredited = true
        }
      } else {
        if (props.absence.isCredited) {
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
              await vacationStore.actions.editVacation(vacation)
            } else {
              absenceData.value.isCredited = false
            }
          }
        } else {
          for (let i = 0; i < vacations.length; i++) {
            let vacation = vacations[i]
            await vacationStore.actions.editVacation(vacation)
          }
        }
      }
    }
  }
  emit('reloadAbsences')
}

onMounted(() => {
  absenceData.value.startDate = new Date(props.absence.startDate)
  absenceData.value.endDate = new Date(props.absence.endDate)
  absenceData.value.absenceType = enumToString(props.absence.absenceType)
})
</script>

<style lang="scss" src="./EmployeeOverview.scss"></style>
