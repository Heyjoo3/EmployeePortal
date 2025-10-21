<template>
  <BaseModal
    :dialog="editDialog"
    title="Personalinformationen"
    bannerIcon="mdi-plus"
    bannerText="Bearbeiten"
    bannerBackgroundColor="var(--status-inactive-update)"
    :formMode="true"
    @close="closeDialog"
    @submit="submitEditForm"
  >
    <template #form-content>
      <v-form ref="createForm">
        <v-row>
          <v-col>
            <v-combobox
              v-model="employeeData.salutation"
              label="Anrede"
              :items="salutations"
              variant="outlined"
            >
            </v-combobox>
          </v-col>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder=""
              v-model="employeeData.firstName"
              label="Vorname"
              variant="outlined"
              :rules="[required]"
              :error-messages="errorMessages"
            >
            </v-text-field>
          </v-col>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder=""
              v-model="employeeData.lastName"
              label="Nachname"
              variant="outlined"
              :rules="[required]"
              :error-messages="errorMessages"
            >
            </v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder=""
              v-model="employeeData.phoneNumber"
              label="Rufnummer"
              variant="outlined"
              :error-messages="errorMessages"
              type="text"
              @keypress="validateNumberInput"
            />
          </v-col>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder=""
              v-model="employeeData.email"
              label="Email"
              variant="outlined"
              :rules="[required]"
              :error-messages="errorMessages"
            >
            </v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <!-- <v-date-input
              color="var(--dark-text)"
              placeholder=""
              v-model="employeeData.dateofBirth"
              label="Geburtsdatum"
              variant="outlined"
              readonly
            ></v-date-input> -->
            <BaseDatePicker
              @input="employeeData.dateofBirth = $event"
              label="Geburtsdatum"
              variant="outlined"
              :value="employeeData.dateofBirth"
            ></BaseDatePicker>
          </v-col>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder=""
              v-model="employeeData.employeeInternalId"
              label="Mitarbeiter ID"
              variant="outlined"
              :rules="[required]"
              :error-messages="errorMessages"
            >
            </v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col v-if="employeeData.role != 'Admin'">
            <v-combobox
              v-model="employeeData.department"
              label="Abteilung"
              :items="departments"
              variant="outlined"
            >
            </v-combobox>
          </v-col>
          <v-col>
            <v-combobox
              v-model="employeeData.role"
              label="Rolle"
              :items="['Mitarbeiter', 'Vorgesetzter', 'Administrator']"
              variant="outlined"
            >
            </v-combobox>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-combobox
              v-model="employeeData.supervisor"
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
            <v-combobox
              v-model="employeeData.substitute"
              label="Vertreter"
              :items="substitutes"
              variant="outlined"
              :item-title="(item) => item.name"
              item-value="id"
              :rules="rules.substitute"
            >
            </v-combobox>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder=""
              v-model="employeeData.userName"
              label="Benutzername"
              variant="outlined"
              :rules="rules.usernameRule"
              :error-messages="errorMessages"
            >
            </v-text-field>
          </v-col>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder="Eingaben hier ersetzen das aktuelle Passwort"
              v-model="employeeData.password"
              :append-icon="show1 ? 'mdi-eye' : 'mdi-eye-off'"
              :type="show1 ? 'text' : 'password'"
              label="Passwort"
              variant="outlined"
              :rules="rules.passwordRule"
              @click:append="show1 = !show1"
              :error-messages="errorMessages"
            >
            </v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-select
              class="text-numberValue"
              placeholder=""
              v-model="employeeData.sickNoteDeadLine"
              :error-messages="errorMessages"
              :rules="[required]"
              variant="outlined"
              label="Abgabefrist AU "
              :items="[
                { title: '1. Tag', value: 1 },
                { title: '2. Tag', value: 2 },
                { title: '3. Tag', value: 3 },
                { title: '4. Tag', value: 4 }
              ]"
            ></v-select>
          </v-col>
          <v-col>
            <v-number-input
              class="text-numberValue"
              placeholder=""
              v-model="employeeData.annualVacation"
              label="Jährliche Urlaubstage"
              variant="outlined"
              :rules="[required]"
              :error-messages="errorMessages"
            >
            </v-number-input>
          </v-col>
        </v-row>
      </v-form>
    </template>
  </BaseModal>
</template>

<script setup>
import { onMounted, ref, computed } from 'vue'
import { goToRoute } from '@/Composables/goToRoute'
import { useDateConverter } from '@/Composables/useDateConverter'
import { resolveSalutation } from '@/Composables/resolveSalutation'
import { useEmployeeStore } from '@/stores/employee-store'
import BaseModal from '../../BaseComponents/BaseModal.vue'
import BaseDatePicker from '../../BaseComponents/BaseDatePicker.vue'

const emit = defineEmits(['closeDialog', 'reloadEmployee'])

const props = defineProps({
  employee: Object
})

const employeeStore = useEmployeeStore()

const salutations = ref(resolveSalutation().salutations)
const departments = ref(employeeStore.state.departments)
const supervisors = computed(() => employeeStore.state.supervisors)
const substitutes = computed(() => employeeStore.state.substitutes)

const createForm = ref(null)

const errorMessages = ref('')
const show1 = ref(false)

const rules = {
  usernameRule: [
    (v) => !!v || 'Feldeingabe erforderlich',
    (v) => (v && v.length >= 4) || 'Benutzername muss mindestens 4 Zeichen lang sein',
    (v) => (v && v.length <= 24) || 'Benutzername muss weniger als 24 Zeichen enthalten'
  ],
  passwordRule: [
    (v) => !v || !!v || 'Eingabe ist erforderlich',
    (v) => !v || (v && v.length >= 8) || 'Kennwort muss mindestens 8 Zeichen lang sein. ',
    (v) => !v || /\d/.test(v) || 'Passwort muss eine Zahl enthalten',
    (v) => !v || /[^\w\s]/.test(v) || 'Passwort muss ein Sonderzeichen enthalten',
    (v) => !v || /[A-Z]/.test(v) || 'Passwort muss einen Großbuchstaben enthalten'
  ],
  supervisor: [
    (v) =>
      v === undefined ||
      v === null ||
      typeof v === 'object' ||
      'Bitte wähle einen der möglichen Vorgesetzten'
  ],
  substitute: [
    (v) =>
      v === undefined ||
      v === null ||
      typeof v === 'object' ||
      'Bitte wähle einen der möglichen Vertreter'
  ]
}

const required = (value) => {
  return !!value || 'Feldeingabe erforderlich'
}

const employeeData = ref({
  salutation: '',
  firstName: '',
  lastName: '',
  department: '',
  email: '',
  phoneNumber: '',
  dateofBirth: '',
  role: '',
  employeeInternalId: '',
  supervisor: '',
  substitute: '',
  annualVacation: '',
  sickNoteDeadLine: '',
  userName: '',
  password: '',
  ...props.employee
})

function closeDialog() {
  emit('closeDialog')
}

async function submitEditForm() {
  const validate = await createForm.value?.validate()

  if (!validate.valid) {
    return
  }

  const salutation = resolveSalutation().stringToEnum(employeeData.value.salutation)
  const tempSupervisor = employeeData.value.supervisor
  const tempSubstitute = employeeData.value.substitute

  if (employeeData.value.dateofBirth != typeof Date) {
    employeeData.value.dateofBirth = new Date(employeeData.value.dateofBirth)
  }

  // employeeData.value.dateofBirth = useDateConverter().localizedDate(employeeData.value.dateofBirth)
  employeeData.value.dateofBirth = useDateConverter().convertToGermanISO(
    employeeData.value.dateofBirth
  )

  employeeData.value.salutation = salutation

  if (employeeData.value.supervisor === null) {
    employeeData.value.supervisor = null
  } else {
    employeeData.value.supervisor = employeeData.value.supervisor.id
  }

  if (employeeData.value.substitute === null) {
    employeeData.value.substitute = null
  } else {
    employeeData.value.substitute = employeeData.value.substitute.id
  }

  switch (employeeData.value.role) {
    case 'Mitarbeiter':
      employeeData.value.role = 'Employee'
      break
    case 'Vorgesetzter':
      employeeData.value.role = 'Supervisor'
      break
    case 'Administrator':
      employeeData.value.role = 'Admin'
      break
    default:
      break
  }

  const forcePasswordUpdate =
    employeeData.value.password === '' || employeeData.value.password === undefined ? false : true

  const responseUpdateUser = await employeeStore.actions.updateEmployee(
    employeeData,
    forcePasswordUpdate
  )

  if (responseUpdateUser.isSuccessfull) {
    if (employeeData.value.employeeInternalId != props.employee.employeeInternalId) {
      employeeStore.state.pickedEmployeeId = employeeData.value.employeeInternalId
      goToRoute('EmployeeOverview', parseInt(employeeData.value.employeeInternalId), 'admin')
    } else {
      emit('reloadEmployee')
    }
  } else {
    console.error('Fehler beim Speichern der Daten')
  }

  employeeData.value.dateofBirth = useDateConverter().localizedDate(employeeData.value.dateofBirth)
  employeeData.value.salutation = resolveSalutation().enumToString(employeeData.value.salutation)
  employeeData.value.supervisor = tempSupervisor
  employeeData.value.substitute = tempSubstitute
}

function validateNumberInput(event) {
  if (!/^\d$/.test(event.key)) {
    event.preventDefault()
  }
}

onMounted(async () => {
  await employeeStore.actions.getEmployeesReduced()
  employeeStore.actions.getSupervisors()
  employeeStore.actions.getSubstitutes()

  Object.assign(employeeData.value, {
    salutation: resolveSalutation().enumToString(props.employee.salutation),
    firstName: props.employee.firstName,
    lastName: props.employee.lastName,
    department: props.employee.department,
    email: props.employee.email,
    phoneNumber: props.employee.phoneNumber,
    dateofBirth: new Date(props.employee.dateofBirth),
    role: (() => {
      switch (props.employee.role) {
        case 'Employee':
          return 'Mitarbeiter'
        case 'Supervisor':
          return 'Vorgesetzter'
        case 'Admin':
          return 'Administrator'
        default:
          return props.employee.role
      }
    })(),
    employeeInternalId: props.employee.employeeInternalId,
    supervisor: props.employee.supervisor,
    substitute: props.employee.substitute,
    annualVacation: props.employee.annualVacation,
    userName: props.employee.userName,
    password: props.employee.password,
    sickNoteDeadLine: props.employee.sickNoteDeadLine
  })

  if (employeeData.value.supervisor === null) {
    employeeData.value.supervisor = { id: null, name: '' }
  } else {
    employeeData.value.supervisor = supervisors.value.find(
      (supervisor) => supervisor.id === employeeData.value.supervisor
    )
  }

  if (employeeData.value.substitute === null || employeeData.value.substitute === undefined) {
    employeeData.value.substitute = { id: null, name: '' }
  } else {
    employeeData.value.substitute = substitutes.value.find(
      (substitute) => substitute.id === employeeData.value.substitute
    )
    // dieses console.log bitte nicht löschen, es ist das equivalent zu coconut.jpg
    // console.log(employeeData.value.substitute)
  }
})
</script>

<style lang="scss" src="./EmployeeOverview.scss" scoped />
