<template>
  <contact-info
    v-if="pickedEmployee"
    @openEditDialog="openEditContactDialog"
    @openAddAbsenceDialog="openAddAbsenceDialog"
    :employeeInternalId="employeeInternalId"
    :employee="pickedEmployee"
    :key="contactInfoKey"
  ></contact-info>
  <vacation-stats
    :key="vacationStatsKey"
    v-if="pickedEmployee"
    :employee="pickedEmployee"
  ></vacation-stats>
  <v-btn
    v-if="role === 'Admin'"
    @click="openAdminAddVacationDialog"
    class="add-vacation-button"
    color="primary"
    dark
  >
    Urlaub hinzufügen
  </v-btn>
  <vacation-table
    v-if="vacations"
    :vacations="vacations"
    @reload-vacations="reloadVacations"
    @openEditVacationDialog="openEditVacationDialog"
    :relationship="relationship"
    :key="vacationTableKey"
  ></vacation-table>
  <absence-table
    v-if="
      absences &&
      (role == 'Admin' ||
        (pickedEmployee.supervisor &&
          pickedEmployee.supervisor === employeeStore.state.currentGuid))
    "
    @reloadAbsences="reloadAbsences"
    @openEditAbsenceDialog="openEditAbsenceDialog"
    :employee="pickedEmployee"
    :relationship="relationship"
    :absences="absences"
    :key="absenceTableKey"
  ></absence-table>
  <add-absence-dialog
    v-if="pickedEmployee && relationship == 'admin'"
    v-model="addDialog"
    @closeDialog="handleDialogClose"
    @reloadAbsences="reloadAbsences"
    :employee="pickedEmployee"
  ></add-absence-dialog>
  <edit-contact-dialog
    v-if="pickedEmployee"
    :key="editContactKey"
    v-model="editContactDialog"
    @reloadEmployee="reloadEmployee"
    @closeDialog="handleDialogClose"
    :employee="pickedEmployee"
  ></edit-contact-dialog>
  <edit-vacations-dialog
    v-if="vacation"
    v-model="editVacationDialog"
    @closeDialog="handleDialogClose"
    @reloadVacations="reloadVacations"
    :vacation="vacation"
  ></edit-vacations-dialog>
  <edit-absence-dialog
    v-if="absence"
    v-model="editAbsenceDialog"
    @closeDialog="handleDialogClose"
    @reloadAbsences="reloadAbsences"
    :absence="absence"
  ></edit-absence-dialog>
  <admin-add-vacation-dialog
    v-if="pickedEmployee"
    v-model="adminAddVacationDialog"
    @closeDialog="handleDialogClose"
    @reloadVacations="reloadVacations"
    :employee="pickedEmployee"
    :key="adminAddVacationKey"
  >
  </admin-add-vacation-dialog>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import ContactInfo from './ContactInfo.vue'
// eslint-disable-next-line no-unused-vars
import EditContactDialog from './EditContactDialog.vue'
import VacationTable from './VacationTable.vue'
import AddAbsenceDialog from './AddAbsenceDialog.vue'
import AbsenceTable from './AbsenceTable.vue'
// eslint-disable-next-line no-unused-vars
import EditAbsenceDialog from './EditAbsenceDialog.vue'
import { useEmployeeStore } from '@/stores/employee-store.js'
import { useVacationStore } from '@/stores/vacation-store.js'
import { useAbsenceStore } from '@/stores/absence-store.js'
import EditVacationsDialog from './EditVacationsDialog.vue'
import VacationStats from './VacationStats.vue'
// eslint-disable-next-line no-unused-vars
import AdminAddVacationDialog from './AdminAddVacationDialog.vue'

const employeeStore = useEmployeeStore()
const vacationStore = useVacationStore()
const absenceStore = useAbsenceStore()
const absenceActions = absenceStore.actions
const { currentEmployee } = storeToRefs(employeeStore)
const id = ref(employeeStore.state.pickedEmployeeId)
const vacations = ref(null)
const absences = ref(null)
const relationship = ref(null)
const absence = ref(null)
const vacation = ref(null)
const role = ref(currentEmployee.value.role)

const contactInfoKey = ref(0)
const vacationStatsKey = ref(0)
const vacationTableKey = ref(0)
const absenceTableKey = ref(0)
const editContactKey = ref(0)
const adminAddVacationKey = ref(0)

watch(
  () => employeeStore.state.pickedEmployeeId,
  async (newValue) => {
    id.value = newValue
    pickedEmployee.value = await employeeStore.actions.getEmployeeById(id.value)
    vacations.value = await vacationStore.actions.getVacationsByEmployeeIdDB(
      pickedEmployee.value.id
    )
    incrementKeys()
  }
)

const pickedEmployee = ref(null)
const editContactDialog = ref(false)
const addDialog = ref(false)
const editAbsenceDialog = ref(false)
const editVacationDialog = ref(false)
const adminAddVacationDialog = ref(false)

function openEditContactDialog() {
  editContactDialog.value = true
}

function openAddAbsenceDialog() {
  addDialog.value = true
}

function openEditAbsenceDialog(item) {
  absence.value = item
  editAbsenceDialog.value = true
}

function openEditVacationDialog(item) {
  vacation.value = item
  editVacationDialog.value = true
}

function openAdminAddVacationDialog() {
  adminAddVacationDialog.value = true
}

function handleDialogClose() {
  editContactDialog.value = false
  addDialog.value = false
  editAbsenceDialog.value = false
  absence.value = null
  vacation.value = null
  editVacationDialog.value = false
  adminAddVacationDialog.value = false
  editContactKey.value++
}

async function reloadVacations() {
  vacations.value = await vacationStore.actions.getVacationsByEmployeeIdDB(pickedEmployee.value.id)
  vacation.value = null
  editVacationDialog.value = false
  adminAddVacationDialog.value = false
  vacationTableKey.value++
  vacationStatsKey.value++
  adminAddVacationKey.value++
}

async function reloadAbsences() {
  absences.value = await absenceActions.getAbsencesByEmployeeIdDb(pickedEmployee.value.id)
  absence.value = null
  editAbsenceDialog.value = false
  addDialog.value = false
  absenceTableKey.value++
}

async function reloadEmployee() {
  pickedEmployee.value = await employeeStore.actions.getEmployeeById(id.value)
  editContactDialog.value = false
  contactInfoKey.value++
  editContactKey.value++
}

function incrementKeys() {
  contactInfoKey.value++
  vacationTableKey.value++
  absenceTableKey.value++
  vacationStatsKey.value++
}
onMounted(async () => {
  await reloadEmployee()
  await reloadVacations()
  await reloadAbsences()

  relationship.value = employeeStore.pickedRelation

  await absenceStore.actions.checkSickNoteDeadline(pickedEmployee.value.id)
})
</script>

<style lang="scss" src="./EmployeeOverview.scss" scoped />
