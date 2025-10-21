<template>
  <BaseModal
    title="Urlaub beantragen"
    bannerIcon="mdi-plus"
    bannerText="Erstellen"
    :formMode="true"
    @submit="submitVacationForm"
    @close="closeDialog"
  >
    <template #form-content>
      <v-form class="add-form" ref="form">
        <v-row>
          <v-col>
            <v-date-input
              ref="start-date-input"
              color="var(--dark-text)"
              class="text-input"
              placeholder=""
              v-model="vacationData.startDate"
              label="Startdatum"
              variant="outlined"
              :min="minDate"
              :rules="rules.startDate"
              :error-messages="errorMessages"
              readonly
            >
            </v-date-input>
            <!-- TODO: HalfDay checkBox hidden (Ticket 8165) -->
            <!-- <v-checkbox
              color="var(--bold-main-blue)"
              v-model="vacationData.isHalfStartDay"
              label="Halber Tag"
            ></v-checkbox> -->
          </v-col>
          <v-col>
            <v-date-input
              ref="end-date-input"
              color="var(--dark-text)"
              placeholder=""
              v-model="vacationData.endDate"
              label="Enddatum"
              variant="outlined"
              :min="vacationData.startDate ? vacationData.startDate : minDate"
              :rules="rules.endDate"
              :error-messages="errorMessages"
              readonly
            ></v-date-input>
            <!-- TODO: HalfDay checkBox hidden (Ticket 8165)-->
            <!-- <v-checkbox 
              color="var(--bold-main-blue)"
              v-model="vacationData.isHalfEndDay"
              label="Halber Tag"
              v-if="
                new Date(vacationData.startDate).getTime() !==
                new Date(vacationData.endDate).getTime()
              "
            ></v-checkbox> -->
          </v-col>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.supervisor"
              label="Vorgesetzter"
              :items="props.supervisors"
              variant="outlined"
              :rules="rules.supervisor"
              :item-title="(item) => item.name"
              item-value="id"
              :error-messages="errorMessages"
            >
            </v-combobox>
          </v-col>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.substitute"
              label="Vertreter"
              :items="props.substitutes"
              variant="outlined"
              :item-title="(item) => item.name"
              item-value="id"
              :error-messages="errorMessages"
              :rules="rules.substitute"
            >
            </v-combobox>
          </v-col>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.type"
              label="Typ"
              :items="leavetypes"
              variant="outlined"
              :rules="rules.type"
              :error-messages="errorMessages"
            >
            </v-combobox>
          </v-col>
        </v-row>
        <v-row>
          <v-textarea
            color="var(--dark-text)"
            v-model="vacationData.description"
            label="Beschreibung"
            variant="outlined"
            rounded
            :rules="rules.description"
            :error-messages="errorMessages"
          ></v-textarea>
        </v-row>
      </v-form>
    </template>
  </BaseModal>
</template>

<script setup>
import BaseModal from '@/components/BaseComponents/BaseModal.vue'
import { onMounted, ref, watch } from 'vue'
import { useVacationStore } from '@/stores/vacation-store'
import { useEmployeeStore } from '@/stores/employee-store.js'
import { useDateConverter } from '@/Composables/useDateConverter'
import { storeToRefs } from 'pinia'

const employeeStore = useEmployeeStore()

const { convertToGermanISO } = useDateConverter()
const { currentEmployee } = storeToRefs(employeeStore)
const { actions } = useVacationStore()

const form = ref(null)
const leaveList = ref([])
const leavetypes = ref(['Jahresurlaub', 'Sonderurlaub'])
const minDate = ref(new Date().toISOString().substring(0, 10))
const minEndDate = ref(null) // Store min value for end date

const props = defineProps({
  supervisors: Array,
  substitutes: Array
})
const emit = defineEmits(['close', 'submit'])

const vacationData = ref({
  employeeId: currentEmployee.value.id,
  startDate: null,
  endDate: null,
  supervisor: null,
  substitute: null,
  type: 'Jahresurlaub',
  vacationDays: null,
  description: null,
  isHalfStartDay: false,
  isHalfEndDay: false
})

const errorMessages = ref('')

const rules = {
  startDate: [
    (v) => !!v || 'Startdatum darf nicht leer sein',
    (v) => {
      const today = new Date()
      today.setHours(0, 0, 0, 0)
      const startDate = new Date(useDateConverter().convertGermanDateToISO(v))
      startDate.setHours(0, 0, 0, 0)
      return startDate >= today || 'Startdatum darf nicht in der Vergangenheit liegen'
    }
  ],
  endDate: [
    (v) => !!v || 'Enddatum darf nicht leer sein',
    (v) => {
      if (!vacationData.value.startDate) return true
      const startDate = vacationData.value.startDate
      const endDate = new Date(useDateConverter().convertGermanDateToISO(v))
      startDate.setHours(0, 0, 0, 0)
      endDate.setHours(0, 0, 0, 0)
      return endDate >= startDate || 'Enddatum darf nicht vor dem Startdatum liegen'
    },
    (v) => {
      const startDate = vacationData.value.startDate
      const endDate = new Date(useDateConverter().convertGermanDateToISO(v))
      return (
        startDate.getFullYear() === endDate.getFullYear() ||
        'Enddatum darf nicht in einem anderen Jahr liegen'
      )
    }
  ],
  supervisor: [(v) => (!!v && typeof v == 'object') || 'Bitte wähle deinen Vorgesetzten'],
  substitute: [
    (v) =>
      v === undefined ||
      v === null ||
      typeof v === 'object' ||
      'Bitte wähle einen der möglichen Vertreter'
  ],
  type: [(v) => !!v || 'Wähle die Art des Urlaubs'],
  description: [(v) => !v || v.length <= 500 || 'Maximal 500 Zeichen erlaubt']
}

function closeDialog() {
  emit('close')
}

const submitVacationForm = async () => {
  const validate = await form.value.validate()

  if (!validate.valid) {
    return
  }

  const tempEndDate = vacationData.value.endDate
  const tempStartDate = vacationData.value.startDate

  vacationData.value.startDate = convertToGermanISO(vacationData.value.startDate)
  vacationData.value.endDate = convertToGermanISO(vacationData.value.endDate)

  const existingVacations = actions.checkExistingVacation(vacationData.value)

  if (existingVacations) {
    alert('Der Urlaub überschneidet sich mit einem bereits beantragtem Urlaub')
    vacationData.value.startDate = tempStartDate
    vacationData.value.endDate = tempEndDate
    return
  }

  vacationData.value.vacationDays = 0

  if (vacationData.value.substitute === null) {
    vacationData.value.substitute = null
  } else {
    vacationData.value.substitute = vacationData.value.substitute.id
  }

  vacationData.value.supervisor = vacationData.value.supervisor.id

  const response = await actions.createVacation(vacationData)

  if (response.isSuccessfull) {
    emit('submit')
    leaveList.value = []
  } else {
    resolveSubstitute()
    resolveSupervisor()
    vacationData.value.startDate = tempStartDate
    vacationData.value.endDate = tempEndDate
  }
}

function resolveSubstitute() {
  if (currentEmployee.value.substitute !== null) {
    vacationData.value.substitute = props.substitutes.find(
      (substitute) => substitute.id === currentEmployee.value.substitute
    )
  }
}

function resolveSupervisor() {
  if (currentEmployee.value.supervisor !== null) {
    vacationData.value.supervisor = props.supervisors.find(
      (supervisor) => supervisor.id === currentEmployee.value.supervisor
    )
  }
}

function IsDateValid(date) {
  return date && !isNaN(new Date(date).getTime())
}

watch(
  () => vacationData.value.startDate,
  (newStartDate) => {
    if (IsDateValid(newStartDate)) {
      minEndDate.value = newStartDate
      if (
        !vacationData.value.endDate ||
        new Date(vacationData.value.endDate) < new Date(newStartDate)
      ) {
        vacationData.value.endDate = newStartDate
      }
    } else {
      minEndDate.value = minDate
    }
  }
)

onMounted(() => {
  resolveSubstitute()
  resolveSupervisor()
})
</script>

<style lang="scss" src="./VacationDays.scss" scoped />
