<template>
  <v-form ref="editForm" @submit.prevent="submitPasswordUpdate">
    <b style="color: var(--status-error-delete)" v-if="showAlert"
      >Fehler beim Ändern des Passwortes</b
    >
    <p v-if="showAlert">
      Es ist etwas schief gegangen. Prüfe das alte Passwort oder frage nach Unterstützung beim Admin
    </p>
    <v-text-field
      class="text-input"
      placeholder="Neues Passwort"
      v-model.trim="newPassword"
      label="Neues Passwort"
      variant="solo-filled"
      :rules="rules.passwordRule"
      :error-messages="errorMessages && errorMessageRepeat"
      :type="showNewPassword ? 'text' : 'password'"
      :append-inner-icon="showNewPassword ? 'mdi-eye' : 'mdi-eye-off'"
      @click:append-inner="showNewPassword = !showNewPassword"
    />
    <v-text-field
      class="text-input"
      placeholder="Wiederhole das neue Passwort"
      v-model.trim="confirmPassword"
      label="Passwort wiederholen"
      variant="solo-filled"
      :rules="rules.passwordMatch"
      :error-messages="errorMessages"
      :type="showConfirmPassword ? 'text' : 'password'"
      :append-inner-icon="showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'"
      @click:append-inner="showConfirmPassword = !showConfirmPassword"
    />
    <v-row class="justify-end">
      <v-btn size="small" type="submit" id="login" :loading="loading"> Ändern </v-btn>
    </v-row>
  </v-form>
</template>

<script setup>
import { ref, defineEmits } from 'vue'
import { useEmployeeStore } from '@/stores/employee-store'

const emit = defineEmits(['passwordUpdated'])
const employeeStore = useEmployeeStore()

const newPassword = ref('')
const confirmPassword = ref('')
const errorMessages = ref([])
const errorMessageRepeat = ref([])
const editForm = ref(null)
const showAlert = ref(false)
const showNewPassword = ref(false)
const showConfirmPassword = ref(false)

const props = defineProps({
  loggedEmployee: Object,
  oldPassword: String
})

const rules = {
  passwordRule: [
    (v) => !!v || 'Eingabe ist erforderlich',
    (v) => (v && v.length >= 8) || 'Kennwort muss mindestens 8 Zeichen lang sein. ',
    (v) => /\d/.test(v) || 'Passwort muss eine Zahl enthalten',
    (v) => /[^\w\s]/.test(v) || 'Passwort muss ein Sonderzeichen enthalten',
    (v) => /[A-Z]/.test(v) || 'Passwort muss einen Großbuchstaben enthalten'
  ],
  passwordMatch: [
    (v) => v.trim() === newPassword.value.trim() || 'Passwörter stimmen nicht überein',
    (v) => !!v || 'Eingabe erforderlich'
  ]
}

const loading = ref(false)

async function submitPasswordUpdate() {
  const validate = await editForm.value?.validate()

  if (!validate.valid) {
    return
  }

  const updateEmployee = {
    employeeId: props.loggedEmployee.id,
    userName: props.loggedEmployee.userName,
    passwordOld: props.oldPassword,
    passwordNew: newPassword.value
  }

  const response = await employeeStore.actions.updateCredentials(updateEmployee)

  if (response.isSuccessfull) {
    newPassword.value = ''
    confirmPassword.value = ''
    emit('passwordUpdated')
  } else {
    if (response.data.errorType === 1) {
      errorMessageRepeat.value = [response.message]
    }
    showAlert.value = true
  }
}
</script>
