<template>
  <v-row>
    <v-col>
      <div class="property-label mt-4">Titel</div>
      <div class="property-value">{{ props.userData.title }}</div>
    </v-col>
    <v-col>
      <div class="property-label mt-4">Name</div>
      <div class="property-value">{{ props.userData.name }} {{ props.userData.lastName }}</div>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <div class="property-label mt-4">Rufnummer</div>
      <div class="property-value">{{ props.userData.phoneNumber }}</div>
    </v-col>
    <v-col>
      <div class="property-label mt-4">Email</div>
      <div class="property-value">{{ props.userData.email }}</div>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <div class="property-label mt-4">Abteilung</div>
      <div class="property-value">{{ props.userData.department }}</div>
    </v-col>
    <v-col>
      <div class="property-label mt-4">Rolle</div>
      <div class="property-value">{{ translatedRole }}</div>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <div class="property-label mt-4">Vorgesetzter</div>
      <div class="property-value">{{ supervisorName }}</div>
    </v-col>
    <v-col>
      <div class="property-label mt-4">Vertreter</div>
      <div class="property-value">{{ substituteName }}</div>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <div class="property-label mt-4">Mitarbeiter ID</div>
      <div class="property-value">{{ props.userData.employeeInternalId }}</div>
    </v-col>
    <v-col>
      <div class="property-label mt-4">Geburtsdatum</div>
      <div class="property-value">{{ dateofBirth }}</div>
    </v-col>
  </v-row>
</template>

<script setup>
import { onMounted, ref, computed } from 'vue'
import { useEmployeeStore } from '@/stores/employee-store'
import { useDateConverter } from '@/Composables/useDateConverter'

const props = defineProps({
  userData: Array
})

const employeeStore = useEmployeeStore()

let supervisor = null
let substitute = null

const supervisorName = ref(null)
const substituteName = ref(null)

const dateofBirth = ref(null)

onMounted(() => {
  supervisor = employeeStore.actions.getNameById(props.userData.supervisor)
  substitute = employeeStore.actions.getNameById(props.userData.substitute)

  supervisorName.value = supervisor
  substituteName.value = substitute

  dateofBirth.value = useDateConverter().localizedDate(props.userData.dateofBirth)
})

const translatedRole = computed(() => {
  switch (props.userData.role) {
    case 'Employee':
      return 'Mitarbeiter'
    case 'Supervisor':
      return 'Vorgesetzter'
    case 'Admin':
      return 'Administrator'
    default:
      return props.userData.role
  }
})
</script>

<style lang="scss" src="./ProfileDetails.scss" scoped />
