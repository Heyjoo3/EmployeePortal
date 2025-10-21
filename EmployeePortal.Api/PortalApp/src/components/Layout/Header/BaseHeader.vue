<template>
  <v-app-bar-title
    ><img src="@/assets/ctn_logo-claim_weiß.png" class="header-image"
  /></v-app-bar-title>
  <v-spacer></v-spacer>
  <div class="logout" v-if="!props.logIn">
    <span v-if="userData != null"> {{ userData.Name + ' ' + userData.LastName }}</span>
    <header-button @openLogoutCard="logoutCardOpen = true" />
    <logout-card
      @closeLogoutCard="logoutCardOpen = false"
      class="align-center justify-center"
      v-model="logoutCardOpen"
    >
    </logout-card>
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useEmployeeStore } from '@/stores/employee-store.js'
import LogoutCard from './LogoutCard.vue'
import HeaderButton from './HeaderButton.vue'

const employeeStore = useEmployeeStore()
const { currentEmployee } = storeToRefs(employeeStore)

const props = defineProps({
  logIn: Boolean
})

const logoutCardOpen = ref(false)

const employeeData = currentEmployee.value
const userData = ref(null)

watch(
  employeeData,
  (newVal) => {
    if (newVal) {
      userData.value = {
        Name: newVal.firstName,
        LastName: newVal.lastName
      }
    } else {
      userData.value = null
    }
  },
  { immediate: true }
)

onMounted(() => {
  if (currentEmployee.value) {
    userData.value = {
      Name: currentEmployee.value.firstName,
      LastName: currentEmployee.value.lastName
    }
  }
})
</script>

<style lang="scss" src="./BaseHeader.scss" scoped />
