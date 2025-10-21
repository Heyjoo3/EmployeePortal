<template>
  <base-modal v-model="dialog" @close="closeDialog" @submit="submitForm" title="Urlaub hinzufügen">
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
              readonly
              :rules="rules.required"
              :error-messages="errorMessages"
            >
            </v-date-input>
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
              readonly
              :rules="rules.required"
              :error-messages="errorMessages"
            ></v-date-input>
          </v-col>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.type"
              label="Urlaubsart"
              :items="leavetypes"
              variant="outlined"
              :rules="rules.required"
              :error-messages="errorMessages"
            ></v-combobox>
          </v-col>
        </v-row>
        <!-- <v-row>
          <v-col>
            <v-checkbox
              color="var(--bold-main-blue)"
              v-model="vacationData.isHalfStartDay"
              label="Halber Tag"
            ></v-checkbox>
          </v-col>
          <v-col>
            <v-checkbox
              color="var(--bold-main-blue)"
              v-model="vacationData.isHalfEndDay"
              label="Halber Tag"
              v-if="
                new Date(vacationData.startDate).getTime() !==
                new Date(vacationData.endDate).getTime()
              "
            ></v-checkbox>
          </v-col>
        </v-row> -->
        <v-row>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.supervisor"
              label="Vorgesetzter"
              :items="supervisors"
              :item-title="(item) => item.name"
              item-value="id"
              variant="outlined"
              :rules="rules.supervisor"
              :error-messages="errorMessages"
            ></v-combobox>
          </v-col>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.substitute"
              label="Vertreter"
              :items="substitutes"
              :item-title="(item) => item.name"
              item-value="id"
              variant="outlined"
              :rules="rules.substitute"
              :error-messages="errorMessages"
            ></v-combobox>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-textarea
              color="var(--dark-text)"
              class="text-input"
              v-model="vacationData.comment"
              label="Kommentar"
              variant="outlined"
              :rules="rules.description"
              :error-messages="errorMessages"
            ></v-textarea>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <p>Urlaub genehmigt:</p>
            <p>Vertreter und Vorgestzter haben zugestimmt.</p>
            <p>
              Wenn Admin den Urlaubgenehmeigt hat, dann ist der Status des Urlaubnehmenden
              "Stornierung offen", falls die Bestätigung des Admins fehlt, dann ist der Status des
              Urlaubnehmenden "Nicht relevant"
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.status"
              label="Urlaubsststatus"
              :items="status"
              variant="outlined"
              :rules="rules.required"
              :error-messages="errorMessages"
            ></v-combobox>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.employeeStatus"
              label="Status des Urlaubnehmenden"
              :items="status"
              variant="outlined"
              :rules="rules.required"
              :error-messages="errorMessages"
            ></v-combobox>
          </v-col>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.substituteStatus"
              label="Vertreterstatus"
              :items="status"
              variant="outlined"
              :rules="rules.required"
              :error-messages="errorMessages"
            ></v-combobox>
          </v-col>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.supervisorStatus"
              label="Vorgesetzenstatus"
              :items="status"
              variant="outlined"
              :rules="rules.required"
              :error-messages="errorMessages"
            ></v-combobox>
          </v-col>
          <v-col>
            <v-combobox
              color="var(--dark-text)"
              v-model="vacationData.adminStatus"
              label="Adminstatus"
              :items="status"
              variant="outlined"
              :rules="rules.required"
              :error-messages="errorMessages"
            ></v-combobox>
          </v-col>
        </v-row>
      </v-form>
    </template>
  </base-modal>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import BaseModal from '@/components/BaseComponents/BaseModal.vue'
import { useEmployeeStore } from '@/stores/employee-store'
import { useVacationStore } from '@/stores/vacation-store'
import { resolveVacationStatus } from '@/Composables/resolveVacationStatus'
import { useDateConverter } from '@/Composables/useDateConverter'

const employeeStore = useEmployeeStore()
const { actions } = useVacationStore()

const props = defineProps({
  employee: Object
})

const emit = defineEmits(['close', 'reloadVacations'])

const closeDialog = () => {
  dialog.value = false
  emit('closeDialog')
}

const dialog = ref(false)
const form = ref(null)

const leavetypes = ref(['Jahresurlaub', 'Sonderurlaub'])

const vacationData = ref({
  startDate: null,
  endDate: null,
  supervisor: null,
  vacationType: null,
  comment: null,
  isHalfStartDay: false,
  isHalfEndDay: false,
  substituteStatus: null,
  supervisorStatus: null,
  adminStatus: null
})

const rules = {
  required: [(v) => !!v || 'Pflichtfeld'],
  supervisor: [(v) => (!!v && typeof v == 'object') || 'Bitte wähle deinen Vorgesetzten'],
  substitute: [
    (v) =>
      v === undefined ||
      v === null ||
      typeof v === 'object' ||
      'Bitte wähle einen der möglichen Vertreter'
  ],
  description: [(v) => !v || v.length <= 500 || 'Maximal 500 Zeichen erlaubt'],
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
  ]
}

const errorMessages = ref([])

const substitutes = ref([])
const supervisors = ref([])

const status = ref(resolveVacationStatus().vacationStatuses)

const submitForm = async () => {
  const validate = await form.value.validate()

  if (!validate.valid) {
    return
  }

  const tempStartDate = vacationData.value.startDate
  const tempEndDate = vacationData.value.endDate

  vacationData.value.startDate = useDateConverter().convertToGermanISO(vacationData.value.startDate)
  vacationData.value.endDate = useDateConverter().convertToGermanISO(vacationData.value.endDate)

  const existingVacations = actions.checkExistingVacation(vacationData.value)

  if (existingVacations.length > 0) {
    errorMessages.value = 'Es existiert bereits ein Urlaub in diesem Zeitraum'
    return
  }
  vacationData.value.vacationDays = 0

  if (vacationData.value.substitute === null) {
    vacationData.value.substitute = null
  } else {
    vacationData.value.substitute = vacationData.value.substitute.id
  }

  vacationData.value.supervisor = vacationData.value.supervisor.id

  vacationData.value.employeeId = props.employee.id

  vacationData.value.adminStatus = resolveVacationStatus().stringToEnum(
    vacationData.value.adminStatus
  )
  vacationData.value.substituteStatus = resolveVacationStatus().stringToEnum(
    vacationData.value.substituteStatus
  )
  vacationData.value.supervisorStatus = resolveVacationStatus().stringToEnum(
    vacationData.value.supervisorStatus
  )
  vacationData.value.employeeStatus = resolveVacationStatus().stringToEnum(
    vacationData.value.employeeStatus
  )
  vacationData.value.status = resolveVacationStatus().stringToEnum(vacationData.value.status)

  const response = await actions.adminCreateVacation(vacationData)

  if (response.isSuccessfull) {
    emit('reloadVacations')
  } else {
    resolveSubstitute()
    resolveSupervisor()
    vacationData.value.startDate = tempStartDate
    vacationData.value.endDate = tempEndDate
  }
}

function resolveSubstitute() {
  if (props.employee.substitute !== null) {
    vacationData.value.substitute = substitutes.value.find(
      (substitute) => substitute.id === props.employee.substitute
    )
  } else {
    vacationData.value.substitute = null
  }
}

function resolveSupervisor() {
  if (props.employee.supervisor !== null) {
    vacationData.value.supervisor = supervisors.value.find(
      (supervisor) => supervisor.id === props.employee.supervisor
    )
  } else {
    vacationData.value.supervisor = null
  }
}

onMounted(() => {
  dialog.value = true
  employeeStore.actions.getSupervisors(props.employee.id)
  employeeStore.actions.getSubstitutes()
  supervisors.value = employeeStore.allSupervisors
  substitutes.value = employeeStore.allSubstitutes

  resolveSubstitute()
  resolveSupervisor()

  vacationData.value.status = status.value[1]
  vacationData.value.employeeStatus = status.value[6]
  vacationData.value.substituteStatus = status.value[1]
  vacationData.value.supervisorStatus = status.value[1]
  vacationData.value.adminStatus = status.value[1]
})
</script>
