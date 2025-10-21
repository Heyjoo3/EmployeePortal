<template>
  <div class="user-profile">
    <v-container fluid>
      <v-row>
        <v-col>
          <div class="personnel-avatar">
            <img src="/src/assets/contact.png" width="280" height="272" />
          </div>
          <div class="edit-buttons">
            <v-btn v-if="role === 'Admin'" width="280" @click="openEditDialog"
              >Mitarbeiter bearbeiten
            </v-btn>
            <v-btn v-if="role === 'Admin'" width="280" @click="openAddAbsenceDialog"
              >Mitarbeiter abwesend melden
            </v-btn>
          </div>
        </v-col>
        <v-col>
          <v-row>
            <v-col>
              <div class="property-label mt-4">Titel</div>
              <div class="property-value">{{ salutation }}</div>
            </v-col>
            <v-col>
              <div class="property-label mt-4">Name</div>
              <div class="property-value">{{ employee.firstName + ' ' + employee.lastName }}</div>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <div class="property-label mt-4">Email</div>
              <div class="property-value">{{ employee.email }}</div>
            </v-col>
            <v-col>
              <div class="property-label mt-4">Rufnummer</div>
              <div class="property-value">{{ employee.phoneNumber || '-' }}</div>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <div class="property-label mt-4">Abteilung</div>
              <div class="property-value">{{ employee.department }}</div>
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
              <div class="property-value">{{ supstituteName }}</div>
            </v-col>
          </v-row>
          <v-row>
            <v-col v-if="role === 'Admin'">
              <div class="property-label mt-4">Geburtsdatum</div>
              <div class="property-value">{{ dateofBirth }}</div>
            </v-col>
            <v-col v-if="role === 'Admin'">
              <div class="property-label mt-4">Krankheitstage dieses Jahr</div>
              <div class="property-value">{{ sickDaysThisYear || '-' }}</div>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <div class="property-label mt-4">Mitarbeiter ID</div>
              <div class="property-value">{{ employee.employeeInternalId }}</div>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useEmployeeStore } from '@/stores/employee-store.js'
import { useAbsenceStore } from '@/stores/absence-store'
import { storeToRefs } from 'pinia'
import { useDateConverter } from '@/Composables/useDateConverter'
import { resolveSalutation } from '@/Composables/resolveSalutation'

const props = defineProps(['employeeInternalId', 'employee'])
const emit = defineEmits(['openEditDialog'])

const employeeStore = useEmployeeStore()
// const absenceStore = useAbsenceStore()
const absenceActions = useAbsenceStore().actions
const { currentEmployee } = storeToRefs(employeeStore)
const role = ref(currentEmployee.value.role)
const { localizedDate } = useDateConverter()
const dateofBirth = ref(localizedDate(props.employee.dateofBirth))
const salutation = ref(resolveSalutation().enumToString(props.employee.salutation))
const supervisors = ref(employeeStore.state.supervisors)
const substitutes = ref(employeeStore.state.substitutes)
const sickDaysThisYear = ref()

// map supervisor id to supervisor name
var supervisor = ref(
  supervisors.value.find((supervisor) => supervisor.id === props.employee.supervisor)
)
var supervisorName = ref()

// map substitute id to substitute name
var substitute = ref(
  substitutes.value.find((substitute) => substitute.id === props.employee.substitute)
)
var supstituteName = ref()

function openEditDialog() {
  emit('openEditDialog')
}

function openAddAbsenceDialog() {
  emit('openAddAbsenceDialog')
}

onMounted(async () => {
  supervisor.value = supervisors.value.find(
    (supervisor) => supervisor.id === props.employee.supervisor
  )

  if (supervisor.value === undefined || supervisor.value === null) {
    supervisorName.value = '-'
  } else {
    supervisorName.value = supervisor.value.name
  }

  substitute.value = substitutes.value.find(
    (substitute) => substitute.id === props.employee.substitute
  )

  if (substitute.value === undefined || substitute.value === null) {
    supstituteName.value = '-'
  } else {
    supstituteName.value = substitute.value.name
  }

  if (role.value === 'Admin') {
    sickDaysThisYear.value = await absenceActions.getSickDaysFromCurrentYear(props.employee.id)
    console.log('id und sickDaysThisYear.value: ', props.employee.id, sickDaysThisYear.value)
  }
})

const translatedRole = computed(() => {
  switch (props.employee.role) {
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

<style lang="scss" src="./EmployeeOverview.scss" scoped />
