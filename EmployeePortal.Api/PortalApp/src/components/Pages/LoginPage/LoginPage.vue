<template>
  <v-layout>
    <v-app-bar color="var(--primary-blue)" prominent>
      <base-header :logIn="true" />
    </v-app-bar>
    <v-main class="body">
      <v-sheet class="sheet pa-12">
        <v-form
          v-if="updatePasswordMask"
          v-model="form"
          ref="loginform"
          @submit.prevent="loginAuthenticate"
        >
          <v-col>
            <h2>Mitarbeiter Portal</h2>
          </v-col>
          <v-col>
            <v-row>
              <!-- v-model.trim to trim any whitespaces in text fields -->
              <v-text-field
                v-model.trim="credentials.userName"
                label="Benutzername"
                variant="solo-filled"
                :rules="rules.usernameRule"
                lazy-validation
              ></v-text-field>
            </v-row>
            <v-row>
              <v-text-field
                v-model.trim="credentials.password"
                label="Kennwort"
                variant="solo-filled"
                :type="showPassword ? 'text' : 'password'"
                :append-inner-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :rules="rules.passwordRule"
                @click:append-inner="showPassword = !showPassword"
                lazy-validation
              ></v-text-field>
            </v-row>
            <span v-if="error">Fehler: Benutzername oder Kennwort nicht korrekt</span>
            <v-row class="justify-end">
              <v-btn
                size="small"
                type="submit"
                id="login"
                :disabled="!isFormValid"
                :loading="loading"
              >
                Anmelden
              </v-btn>
            </v-row>
          </v-col>
        </v-form>
        <update-password-mask
          v-if="!updatePasswordMask"
          :loggedEmployee="responseLoadUser.data"
          :oldPassword="credentials.password"
          @passwordUpdated="returnToLogin()"
        />
      </v-sheet>
    </v-main>
  </v-layout>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useEmployeeStore } from '../../../stores/employee-store.js'
import { useRouter } from 'vue-router'
import BaseHeader from '@/components/Layout/Header/BaseHeader.vue'
// eslint-disable-next-line no-unused-vars
import UpdatePasswordMask from './UpdatePasswordMask.vue'

const router = useRouter()
const employeeStore = useEmployeeStore()
const { actions } = employeeStore
const credentials = ref({
  userName: null,
  password: null
})
const responseLoadUser = ref(null)

const form = ref(false)
const loginform = ref(null)
const error = ref(false)
const showPassword = ref(false)
const updatePasswordMask = ref(true)

const loginAuthenticate = async () => {
  loginform.value?.validate()

  if (loginform.value.isValid) {
    loading.value = true
    try {
      const responseCredentials = await actions.loginEmployee(credentials)
      if (responseCredentials.data.isSuccessfull) {
        localStorage.setItem('jwtToken', responseCredentials.data.token)
        localStorage.setItem('role', responseCredentials.data.data.role)
        responseLoadUser.value = await employeeStore.actions.getLoggedEmployee()

        if (responseLoadUser.value.isSuccessfull) {
          if (responseLoadUser.value.data.forceNewPassword) {
            updatePasswordMask.value = false
          } else {
            credentials.value = {
              userName: null,
              password: null
            }
            await router.push({ name: 'UserProfile' })
          }
        } else {
          loading.value = false
          error.value = true
        }
      } else {
        loading.value = false
        error.value = true
      }
    } catch (err) {
      console.error('Beim Anmelden ist ein Fehler aufgetreten: ', err.code)
      loading.value = false
      error.value = true
    } finally {
      loading.value = false
    }
  }
}

const isValiduserName = computed(() => credentials.value.userName != null)
const isValidpassword = computed(() => credentials.value.password != null)
const isFormValid = computed(() => isValiduserName.value && isValidpassword.value)
const loading = ref(false)
const rules = {
  usernameRule: [(v) => !!v || 'Benutzername ist erforderlich'],
  passwordRule: [(v) => !!v || 'Passwort ist erforderlich']
}

const returnToLogin = () => {
  credentials.value = {
    userName: null,
    password: null
  }
  updatePasswordMask.value = true
}
</script>

<style lang="scss" src="./LoginPage.scss" scoped />
