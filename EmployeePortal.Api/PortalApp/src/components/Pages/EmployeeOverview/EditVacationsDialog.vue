<template>
  <BaseModal
    v-if="vacationData != null"
    :dialog="editDialog"
    title="Urlaub bearbeiten"
    bannerIcon="mdi-plus"
    bannerText="Bearbeiten"
    bannerBackgroundColor="var(--status-inactive-update)"
    :formMode="true"
    :deleteButton="true"
    @close="closeDialog"
    @submit="submitEditForm"
    @delete="deleteVacation"
  >
    <template #form-content>
      <v-form ref="editForm">
        <v-grid>
          <v-row>
            <v-col>
              <v-date-input
                color="var(--dark-text)"
                v-model="vacationData.startDate"
                label="Startdatum"
                variant="outlined"
                :rules="[rules.required]"
                :error-messages="errorMessages"
                readonly
              ></v-date-input>
              <!-- <v-checkbox
                color="var(--bold-main-blue)"
                v-model="vacationData.isHalfStartDay"
                label="Halber Tag"
              ></v-checkbox> -->
            </v-col>
            <v-col>
              <v-date-input
                color="var(--dark-text)"
                v-model="vacationData.endDate"
                label="Enddatum"
                variant="outlined"
                :rules="[rules.required]"
                :error-messages="errorMessages"
                readonly
              ></v-date-input>
              <!-- <v-checkbox
                color="var(--bold-main-blue)"
                v-model="vacationData.isHalfEndDay"
                label="Halber Tag"
              ></v-checkbox> -->
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-text-field
                class="text-input"
                placeholder=""
                v-model="vacationData.description"
                label="Bemerkungen"
                variant="outlined"
                :rules="[rules.description]"
                :error-messages="errorMessages"
              >
              </v-text-field>
            </v-col>
          </v-row>

          <v-row>
            <v-col>
              <v-select
                v-model="vacationData.type"
                :items="vacationTypes"
                label="Urlaubsart"
                variant="outlined"
                :rules="[rules.required]"
                :error-messages="errorMessages"
              >
              </v-select>
            </v-col>
            <v-col>
              <v-text-field
                class="text-input"
                placeholder=""
                v-model="vacationData.vacationdays"
                label="Tage"
                variant="outlined"
                disabled
              >
              </v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-select
                v-model="itemStatus"
                :items="vacationStatus"
                label="Status"
                variant="outlined"
                :rules="[rules.required]"
                :error-messages="errorMessages"
              >
              </v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-select
                v-model="adminStatus"
                :items="vacationStatus"
                label="Status des Admins"
                variant="outlined"
                :rules="[rules.required]"
              >
              </v-select>
            </v-col>
            <v-col>
              <v-select
                v-model="employeeStatus"
                :items="vacationStatus"
                label="Status des Mitarbeiters"
                variant="outlined"
                :rules="[rules.required]"
              >
              </v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-combobox
                v-model="selectedSupervisor"
                label="Vorgesetzter"
                :items="supervisors"
                variant="outlined"
                :item-title="(item) => item.name"
                item-value="id"
                :rules="rules.supervisor"
              >
              </v-combobox>
            </v-col>
            <v-col>
              <v-select
                v-model="supervisorStatus"
                :items="vacationStatus"
                label="Status des Vorgesetzten"
                variant="outlined"
                :rules="[rules.required]"
              >
              </v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-combobox
                v-model="selectedSubstitute"
                label="Vertretung"
                :items="substitutes"
                variant="outlined"
                :item-title="(item) => item.name"
                item-value="id"
                :rules = rules.substitute
              ></v-combobox>
            </v-col>
            <v-col>
              <v-select
                v-model="substituteStatus"
                :items="vacationStatus"
                label="Status der Vertretung"
                variant="outlined"
                :rules="[rules.required]"
              >
              </v-select>
            </v-col>
          </v-row>
        </v-grid>
      </v-form>
    </template>
  </BaseModal>
</template>

<script setup>
import BaseModal from '../../BaseComponents/BaseModal.vue'
import { useVacationStore } from '@/stores/vacation-store'
import { useEmployeeStore } from '@/stores/employee-store'
import { resolveVacationStatus } from '@/Composables/resolveVacationStatus'
import { ref, onMounted } from 'vue'
import { useDateConverter } from '@/Composables/useDateConverter'

const props = defineProps({
  vacation: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['closeDialog', 'reloadVacations'])

const rules = {
  required: [(v) => !!v || 'Feld darf nicht leer sein'],
  description: [(v) => (v && v.length <= 500) || 'Maximal 500 Zeichen'],
  supervisor: [(v) => (!!v && typeof v == 'object' ) || 'Bitte wähle einen Vorgesetzten'],
  substitute: [(v) => (v === undefined || v === null || typeof v === 'object' )|| 'Bitte wähle einen der möglichen Vertreter']
}

const vacationTypes = [
  'Jahresurlaub',
  'Sonderurlaub',
]

const vacationStore = useVacationStore()
const employeeStore = useEmployeeStore()

const editDialog = ref(null)
const editForm = ref(null)

var vacationData = ref(null)

let supervisors = ref([])
let substitutes = ref([])

let selectedSupervisor = ref(null)
let selectedSubstitute = ref(null)

let adminStatus = ref(null)
let substituteStatus = ref(null)
let supervisorStatus = ref(null)
let employeeStatus = ref(null)
let itemStatus = ref(null)

const vacationStatus = ref(resolveVacationStatus().vacationStatuses)

function closeDialog() {
  editDialog.value = false
  emit('closeDialog')
}

async function submitEditForm() {
  const validate = await editForm.value.validate()

  if (!validate.valid) {
    return
  }

  vacationData.value.adminStatus = resolveVacationStatus().stringToEnum(adminStatus.value)
  vacationData.value.substituteStatus = resolveVacationStatus().stringToEnum(substituteStatus.value)
  vacationData.value.supervisorStatus = resolveVacationStatus().stringToEnum(supervisorStatus.value)
  vacationData.value.employeeStatus = resolveVacationStatus().stringToEnum(employeeStatus.value)
  vacationData.value.status = resolveVacationStatus().stringToEnum(itemStatus.value)

  if (selectedSubstitute.value == null) {
    vacationData.value.substitute = null
  } else {
    vacationData.value.substitute = selectedSubstitute.value.id
  }
  if (selectedSupervisor.value == null) {
    vacationData.value.supervisor = null
  } else {
    vacationData.value.supervisor = selectedSupervisor.value.id
  }

  const tempStartDate = vacationData.value.startDate
  const tempEndDate = vacationData.value.endDate

  vacationData.value.startDate = useDateConverter().convertToGermanISO(vacationData.value.startDate)
  vacationData.value.endDate = useDateConverter().convertToGermanISO(vacationData.value.endDate)

  const response = await vacationStore.actions.editVacation(vacationData.value)

  if (response.isSuccessfull) {
    editDialog.value = false
    emit('reloadVacations')
  }

  vacationData.value.startDate = tempStartDate
  vacationData.value.endDate = tempEndDate
}

async function deleteVacation() {
  const response = await vacationStore.actions.deleteVacation(vacationData.value.vacationId)
  if (response.data.isSuccessfull) {
    editDialog.value = false
    emit('reloadVacations')
  }
}

onMounted(async () => {
  vacationData.value = props.vacation
  vacationData.value.startDate = new Date(vacationData.value.startDate)
  vacationData.value.endDate = new Date(vacationData.value.endDate)

  adminStatus.value = resolveVacationStatus().enumToString(vacationData.value.adminStatus)
  substituteStatus.value = resolveVacationStatus().enumToString(vacationData.value.substituteStatus)
  supervisorStatus.value = resolveVacationStatus().enumToString(vacationData.value.supervisorStatus)
  employeeStatus.value = resolveVacationStatus().enumToString(vacationData.value.employeeStatus)
  itemStatus.value = resolveVacationStatus().enumToString(vacationData.value.status)

  employeeStore.actions.getSupervisors()
  employeeStore.actions.getSubstitutes()
  supervisors.value = employeeStore.state.supervisors
  substitutes.value = employeeStore.state.substitutes

  if (vacationData.value.substitute != null) {
    selectedSubstitute.value = employeeStore.state.substitutes.find(
      (substitute) => substitute.id === vacationData.value.substitute
    )
  }
  if (vacationData.value.supervisor != null) {
    selectedSupervisor.value = employeeStore.state.supervisors.find(
      (supervisor) => supervisor.id === vacationData.value.supervisor
    )
  }

  editDialog.value = true
})
</script>
