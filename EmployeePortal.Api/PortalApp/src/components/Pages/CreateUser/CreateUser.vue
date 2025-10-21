<template>
  <div class="create-user">
    <v-form v-model="form" ref="createForm" @submit.prevent="submitForm">
      <FormRow>
        <template v-slot:firstSlot>
          <combobox-input
            v-model="employeeData.department"
            :items="departmentsItems"
            :required="required"
            label="Abteilung"
          />
        </template>

        <template v-slot:secondSlot>
          <v-select
            v-model="employeeData.role"
            :items="
              employeeStore.allRoles.map((role) => {
                switch (role) {
                  case 'Employee':
                    return 'Mitarbeiter'
                  case 'Supervisor':
                    return 'Vorgesetzter'
                  case 'Admin':
                    return 'Administrator'
                  default:
                    return role
                }
              })
            "
            label="Rolle"
            variant="solo-filled"
            :rules="[required]"
            :error-messages="errorMessages"
          ></v-select>
        </template>

        <template v-slot:thirdSlot>
          <v-combobox
            v-model="employeeData.supervisor"
            label="Vorgesetzter"
            :items="supervisors"
            variant="solo-filled"
            item-value="id"
            :item-title="(item) => item.name"
            :rules="rules.supervisor"
          />
        </template>

        <template v-slot:fourthSlot>
          <v-combobox
            v-model="employeeData.substitute"
            label="Vertreter"
            :items="substitutes"
            variant="solo-filled"
            item-value="id"
            :item-title="(item) => item.name"
            :rules="rules.substitute"
          />
        </template>
      </FormRow>

      <FormRow>
        <template v-slot:firstSlot>
          <combobox-input
            v-model="employeeData.salutation"
            :items="items"
            :required="required"
            label="Anrede"
          />
        </template>

        <template v-slot:secondSlot>
          <text-input
            v-model="employeeData.firstname"
            :required="required"
            :error-messages="errorMessages"
            :label="'Vorname'"
          />
        </template>

        <template v-slot:thirdSlot>
          <text-input
            v-model="employeeData.lastname"
            :required="required"
            :error-messages="errorMessages"
            :label="'Nachname'"
          />
        </template>
      </FormRow>

      <FormRow>
        <template v-slot:firstSlot>
          <DateInput v-model="employeeData.dateofBirth" :minDate="minDate" :required="required" />
        </template>

        <template v-slot:secondSlot>
          <text-input
            v-model="employeeData.email"
            :required="required"
            :error-messages="errorMessages"
            :label="'Email'"
            :prepend-icon="'mdi-email'"
            :placeholder="'beispiel@contechnet.de'"
          />
        </template>
      </FormRow>

      <FormRow>
        <template v-slot:firstSlot>
          <text-input
            v-model.number="employeeData.employeeInternalId"
            :required="required"
            :error-messages="errorMessages"
            :label="'Mitarbeiter ID'"
            :type="'number'"
            :prepend-icon="'mdi-identifier'"
          />
        </template>

        <template v-slot:secondSlot>
          <text-input
            v-model.number="employeeData.phoneNumber"
            :required="required"
            :error-messages="errorMessages"
            :label="'Rufnummer'"
            :type="'number'"
            :placeholder="'Rufnummer ohne Landesvorwahl'"
            :prepend-icon="'mdi-cellphone'"
          />
        </template>
      </FormRow>

      <FormRow>
        <template v-slot:firstSlot>
          <v-select
            placeholder=""
            v-model="employeeData.sickNoteDeadLine"
            :error-messages="errorMessages"
            :rules="[required]"
            variant="solo-filled"
            label="Abgabefrist AU"
            :prepend-icon="'mdi-calendar-clock'"
            :items="[
              { title: '1. Tag', value: 1 },
              { title: '2. Tag', value: 2 },
              { title: '3. Tag', value: 3 },
              { title: '4. Tag', value: 4 }
            ]"
          ></v-select>
        </template>

        <template v-slot:secondSlot>
          <v-number-input
            class="text-numberValue"
            placeholder=""
            v-model="employeeData.annualVacation"
            label="Jährliche Urlaubstage"
            variant="solo-filled"
            :prepend-icon="'mdi-calendar-month  '"
            :rules="[required]"
            :error-messages="errorMessages"
          >
          </v-number-input>
        </template>
      </FormRow>
      <FormRow>
        <template v-slot:firstSlot>
          <TextInput
            v-model="employeeData.username"
            :rules="rules.usernameRule"
            :error-messages="errorMessages"
            :label="'Benutzername'"
            :prepend-icon="'mdi-account'"
          />
        </template>

        <template v-slot:secondSlot>
          <PasswordField
            v-model="employeeData.password"
            :rules="rules"
            :error-messages="errorMessages"
            @toggleAlert="toggleAlert($event)"
          />
        </template>
      </FormRow>
      <FormRow>
        <template v-slot:secondSlot>
          <v-alert class="copy-alert" v-show="showAlert" variant="tonal" type="success">
            Passwort in die Zwischenablage kopiert
          </v-alert>
        </template>
      </FormRow>

      <FormActions :form="form" :loading="loading" />
    </v-form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useEmployeeStore } from '@/stores/employee-store.js'
import { useSideNavStore } from '@/stores/side-nav-store'
import { resolveSalutation } from '@/Composables/resolveSalutation'

import FormRow from './FormRow.vue'
import TextInput from './Forms/TextInput.vue'
import ComboboxInput from './Forms/ComboboxInput.vue'
import DateInput from './Forms/DateInput.vue'
import PasswordField from './Forms/PasswordField.vue'
import FormActions from './FormActions.vue'

const sideNavStore = useSideNavStore()
const employeeStore = useEmployeeStore()
const { actions } = employeeStore

// Refs
const form = ref(false)
const createForm = ref(null)
const loading = ref(false)
const errorMessages = ref('')
const items = resolveSalutation().salutations
const supervisors = ref([])
const substitutes = ref([])
const departmentsItems = ref([])
const showAlert = ref(false)

const employeeData = ref({
  employeeInternalId: null,
  salutation: null,
  firstname: null,
  lastname: null,
  dateofBirth: null,
  email: null,
  username: null,
  password: null,
  phoneNumber: null,
  department: null,
  role: null,
  supervisor: null,
  substitute: null,
  annualVacation: null,
  sickNoteDeadLine: null
})

// Computed properties
const rules = {
  usernameRule: [
    (v) => !!v || 'Benutzername ist erforderlich',
    (v) => (v && v.length >= 4) || 'Name muss mindestens 4 Zeichen lang sein',
    (v) => (v && v.length <= 24) || 'Name muss weniger als 24 Zeichen enthalten'
  ],
  passwordRule: [
    (v) => !!v || 'Kennwort ist erforderlich',
    (v) => (v && v.length >= 8) || 'Kennwort muss mindestens 8 Zeichen lang sein.',
    (v) => /\d/.test(v) || 'Passwort muss eine Zahl enthalten',
    (v) => /[^\w\s]/.test(v) || 'Passwort muss ein Sonderzeichen enthalten',
    (v) => /[A-Z]/.test(v) || 'Passwort muss einen Großbuchstaben enthalten'
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

// Methods
const { stringToEnum } = resolveSalutation()

const submitForm = async () => {
  createForm.value?.validate()
  const salutation = stringToEnum(employeeData.value.salutation)

  if (createForm.value.isValid && salutation !== undefined) {
    loading.value = true
    employeeData.value.salutation = salutation

    // It should not be nullable, but we cant create the first user with no supervisor otherwise
    if (employeeData.value.supervisor !== null) {
      employeeData.value.supervisor = employeeData.value.supervisor.id
    }
    if (employeeData.value.substitute !== null) {
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
    const create = await actions.create(employeeData)

    if (create.isSuccessfull) {
      resetForm()
      await sideNavStore.actions.getEmployeesByDepartment()
    }

    loading.value = false
  }
}

const toggleAlert = (value) => {
  if (value) {
    showAlert.value = value
    setTimeout(() => {
      showAlert.value = false
    }, 1500)
  } else {
    showAlert.value = value
  }
}

const resetForm = () => {
  employeeData.value = {
    employeeInternalId: null,
    salutation: null,
    firstname: null,
    lastname: null,
    dateofBirth: null,
    email: null,
    username: null,
    password: null,
    phoneNumber: null,
    department: null,
    role: null,
    supervisor: null,
    annualVacation: null,
    sickNoteDeadLine: null
  }
}

const required = (value) => {
  return !!value || 'Feldeingabe erforderlich'
}

onMounted(async () => {
  actions.getRoles()
  await actions.getEmployeesReduced()
  substitutes.value = actions.getSubstitutes()
  supervisors.value = actions.getSupervisors()
  departmentsItems.value = employeeStore.allDepartments
})
</script>

<style lang="scss" src="./CreateUser.scss" scoped />
