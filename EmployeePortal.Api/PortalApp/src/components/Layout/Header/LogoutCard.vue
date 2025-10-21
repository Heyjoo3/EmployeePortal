<template>
  <v-overlay location-strategy="static" scroll-strategy="none">
    <v-card class="logout-card" title="Möchten sie sich wirklich abmelden?">
      <v-row class="justify-center">
        <v-btn @click="actions.logout()">Ja</v-btn>
        <v-btn @click="$emit('closeLogoutCard')">Nein</v-btn>
      </v-row>
    </v-card>
  </v-overlay>
</template>

<script setup>
import { useEmployeeStore } from '@/stores/employee-store.js'

const employeeStore = useEmployeeStore()
const { actions } = employeeStore
</script>

<style lang="scss" src="./LogoutCard.scss" scoped />
