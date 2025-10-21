<template>
  <div class="user-profile" :key="userprofileKey">
    <edit-contact-dialog
      v-if="userData"
      v-model="openEditDialog"
      :key="editContactKey"
      :employee="userData"
      @closeDialog="closeDialogs"
      @reloadEmployee="reloadEmployee"
    />
    <add-absence-dialog
      v-if="userData"
      v-model="openAbsenceDialog"
      :employee="userData"
      :key="AddAbsenceKey"
      @closeDialog="closeDialogs"
      @reloadAbsences="reloadAbsences"
    />
    <edit-credentials-dialog
      v-if="userData"
      v-model="openCredentialsDialog"
      :employee="userData"
      :key="editCredtialsKey"
      @closeDialog="closeDialogs"
    />
    <v-container fluid>
      <v-row>
        <v-col>
          <profile-avatar
            :userRole="userData.role"
            @onOpenEditProfile="openDialog('edit')"
            @onOpenAddAbsence="openDialog('absence')"
            @onOpenEditCredentials="openDialog('credentials')"
          />
        </v-col>
        <v-col>
          <profile-details :userData="userData" />
        </v-col>
      </v-row>
    </v-container>
    <absences-table
      :headers="absenceHeaders"
      :absences="absences"
      :username="`${userData.name} ${userData.lastName}`"
      v-if="absences"
    />
    <p v-else>Keine Abwesenheiten vorhanden</p>

    <open-requests-table> </open-requests-table>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useEmployeeStore } from '../../../stores/employee-store.js'
import { useAbsenceStore } from '../../../stores/absence-store.js'
import { storeToRefs } from 'pinia'
import { useDateConverter } from '@/Composables/useDateConverter.js'
import { resolveSalutation } from '@/Composables/resolveSalutation.js'
import { resolveAbsenceType } from '@/Composables/resolveAbsenceType.js'
import EditContactDialog from '@/components/Pages/EmployeeOverview/EditContactDialog.vue'
import AddAbsenceDialog from '@/components/Pages/EmployeeOverview/AddAbsenceDialog.vue'
import EditCredentialsDialog from './EditCredentialsDialog.vue'
import ProfileAvatar from './ProfileAvatar.vue'
// eslint-disable-next-line no-unused-vars
import ProfileDetails from './ProfileDetails.vue'
import AbsencesTable from './AbsencesTable.vue'
import OpenRequestsTable from './OpenRequestsTable.vue'

const userprofileKey = ref(0)
const editContactKey = ref(0)
const editCredtialsKey = ref(0)
const AddAbsenceKey = ref(0)

const employeeStore = useEmployeeStore()
const absenceStore = useAbsenceStore()
const { currentEmployee } = storeToRefs(employeeStore)
const { localizedDate } = useDateConverter()
const { enumToString: resolveSalutationString } = resolveSalutation()
const { enumToString: resolveAbsenceTypeString } = resolveAbsenceType()

const openEditDialog = ref(false)
const openAbsenceDialog = ref(false)
const openCredentialsDialog = ref(false)

const userData = computed(() => ({
  title: resolveSalutationString(currentEmployee.value.salutation),
  name: currentEmployee.value.firstName,
  lastName: currentEmployee.value.lastName,
  email: currentEmployee.value.email,
  department: currentEmployee.value.department,
  employeeInternalId: currentEmployee.value.employeeInternalId,
  role: currentEmployee.value.role,
  dateofBirth: localizedDate(currentEmployee.value.dateofBirth),
  phoneNumber: currentEmployee.value.phoneNumber,
  userName: currentEmployee.value.userName,
  ...currentEmployee.value
}))

const profileDetails = ref([
  { label: 'Titel', value: userData.value.title },
  { label: 'Name', value: `${userData.value.name}, ${userData.value.lastName}` },
  { label: 'Abteilung', value: userData.value.department },
  { label: 'Email', value: userData.value.email },
  { label: 'Rufnummer', value: userData.value.phoneNumber },
  { label: 'Geburtsdatum', value: userData.value.dateofBirth },
  { label: 'Rolle', value: userData.value.role },
  { label: 'Mitarbeiter ID', value: userData.value.employeeInternalId }
])

const absenceHeaders = ref([
  {
    title: 'Von',
    key: 'startDate',
    align: 'center',
    sortable: true,
    value: (item) => `${localizedDate(item.startDate)}`
  },
  {
    title: 'Bis',
    key: 'endDate',
    value: (item) => `${localizedDate(item.endDate)}`,
    align: 'center',
    sortable: true
  },
  {
    title: 'Dauer',
    key: 'duration',
    value: (item) => `${item.duration}`,
    align: 'center',
    sortable: true
  },
  {
    title: 'Grund',
    key: 'absenceType',
    value: (item) => `${resolveAbsenceTypeString(item.absenceType)}`,
    align: 'center',
    sortable: true
  },
  {
    title: 'AU vorhanden',
    key: 'hasSickLeave',
    value: (item) => `${item.hasSickLeave}`,
    align: 'center',
    sortable: true
  },
  {
    title: 'Anmerkungen',
    key: 'remarks',
    value: (item) => `${item.remarks}`,
    align: 'center',
    sortable: false
  }
])

let absences = ref(null)

function openDialog(type) {
  closeDialogs()
  if (type === 'edit') openEditDialog.value = true
  if (type === 'absence') openAbsenceDialog.value = true
  if (type === 'credentials') openCredentialsDialog.value = true
}

function closeDialogs() {
  openEditDialog.value = false
  openAbsenceDialog.value = false
  openCredentialsDialog.value = false
  editContactKey.value++
  editCredtialsKey.value++
  AddAbsenceKey.value++
}

async function reloadEmployee() {
  closeDialogs()
  await employeeStore.actions.getEmployeeById(currentEmployee.value.id)
  userprofileKey.value++
}

async function reloadAbsences() {
  closeDialogs()
  absences.value = await absenceStore.actions.getAbsencesByEmployeeIdDb(currentEmployee.value.id)
  userprofileKey.value++
}

onMounted(async () => {
  await absenceStore.actions.checkSickNoteDeadline(currentEmployee.value.id)
  absences.value = await absenceStore.actions.getAbsencesByEmployeeIdDb(currentEmployee.value.id)
})
</script>

<style lang="scss" src="./UserProfile.scss" scoped />
