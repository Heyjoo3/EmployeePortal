<template>
  <BaseModal
    :dialog="editCredentialsDialog"
    title="Logindaten bearbeiten"
    bannerIcon="mdi-account-key"
    bannerText="Bearbeiten"
    bannerBackgroundColor="var(--status-inactive-update)"
    :formMode="true"
    @close="closeDialog"
    @submit="submitEditCredentialsForm"
  >
    <template #form-content>
      <v-form ref="editForm">
        <v-row>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder="Neuer Benutzername"
              v-model="credentialsData.username"
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
              placeholder="Altes Passwort"
              v-model="credentialsData.passwordOld"
              label="Altes Passwort"
              variant="outlined"
              :rules="rules.required"
              :error-messages="errorMessages"
              :type="toggleInvisible ? 'text' : 'password'"
            >
            </v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <b style="color: var(--status-error-delete)" v-if="showAlert"
              >Fehler beim Ändern des Passwortes</b
            >
            <p v-if="showAlert">
              Es ist etwas schief gegangen. Prüfe das alte Passwort oder frage nach Unterstützung
              beim Admin
            </p>
          </v-col>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder="Neues Passwort"
              v-model="credentialsData.passwordNew"
              label="Neues Passwort"
              variant="outlined"
              :rules="rules.passwordRule"
              :error-messages="errorMessages"
              :type="toggleInvisible ? 'text' : 'password'"
            >
            </v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col></v-col>
          <v-col>
            <v-text-field
              class="text-input"
              placeholder="Wiederhole das neue Passwort"
              v-model="credentialsData.passwordRepeat"
              label="Wiederholtes Passwort"
              variant="outlined"
              :rules="rules.passwordMatch"
              :error-messages="errorMessages"
              :type="toggleInvisible ? 'text' : 'password'"
            >
            </v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col></v-col>
          <v-col>
            <v-btn
              @click="toggleInvisible = !toggleInvisible"
              variant="text"
              class="button"
              v-if="!toggleInvisible"
              prepend-icon="mdi-eye"
            >
              <p>Passwörter anzeigen</p>
            </v-btn>
            <v-btn
              @click="toggleInvisible = !toggleInvisible"
              variant="text"
              class="button"
              v-else
              prepend-icon="mdi-eye-off"
            >
              <p>Passwörter verstecken</p>
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
    </template>
  </BaseModal>
</template>

<script setup>
import BaseModal from '@/components/BaseComponents/BaseModal.vue'
import { useEmployeeStore } from '@/stores/employee-store'
import { onMounted, ref } from 'vue'

const emit = defineEmits(['closeDialog', 'submitEditCredentialsForm'])

const props = defineProps({
  employee: Object
})

const employeeStore = useEmployeeStore()

const showAlert = ref(false)
const toggleInvisible = ref(false)

const credentialsData = ref({
  username: '',
  passwordOld: '',
  passwordNew: '',
  passwordRepeat: ''
})

const errorMessages = ref('')
const editForm = ref(null)

const rules = {
  required: [(v) => !!v || 'Eingabe erforderlich'],
  usernameRule: [
    (v) => !!v || 'Benutzername ist erforderlich',
    (v) => (v && v.length >= 4) || 'Name muss mindestens 4 Zeichen lang sein',
    (v) => (v && v.length <= 24) || 'Name muss weniger als 24 Zeichen enthalten'
  ],
  passwordRule: [
    (v) => !!v || 'Eingabe ist erforderlich',
    (v) => (v && v.length >= 8) || 'Kennwort muss mindestens 8 Zeichen lang sein. ',
    (v) => /\d/.test(v) || 'Passwort muss eine Zahl enthalten',
    (v) => /[^\w\s]/.test(v) || 'Passwort muss ein Sonderzeichen enthalten',
    (v) => /[A-Z]/.test(v) || 'Passwort muss einen Großbuchstaben enthalten'
  ],
  passwordMatch: [
    (v) =>
      v.trim() === credentialsData.value.passwordNew.trim() || 'Passwörter stimmen nicht überein',
    (v) => !!v || 'Eingabe erforderlich'
  ]
}

function closeDialog() {
  emit('closeDialog')
}

async function submitEditCredentialsForm() {
  const validate = await editForm.value?.validate()

  if (!validate.valid) {
    return
  }

  const employee = props.employee

  const updateEmployee = {
    employeeId: employee.id,
    userName: credentialsData.value.username,
    passwordOld: credentialsData.value.passwordOld,
    PasswordNew: credentialsData.value.passwordNew
  }

  const response = await employeeStore.actions.updateCredentials(updateEmployee)

  if (response.isSuccessfull) {
    emit('closeDialog')
    credentialsData.value.username = ''
    credentialsData.value.passwordOld = ''
    credentialsData.value.passwordNew = ''
    credentialsData.value.passwordRepeat = ''
  } else {
    showAlert.value = true
  }
}

onMounted(() => {
  credentialsData.value.username = props.employee.userName
})
</script>

<style scoped>
.button {
  width: 100%;
  font-size: 12px;
  font-weight: 500;
  text-transform: none;
}
</style>
