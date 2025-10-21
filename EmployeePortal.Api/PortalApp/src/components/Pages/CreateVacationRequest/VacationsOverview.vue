<template>
  <div class="overview" :key="componentKey">
    <div class="tableHead">
      <h2 class="mt-4">Urlaubsübersicht</h2>
      <export-button
        @click="
          exportVacations(
            allCurrentEmployeeVacations,
            `${employeeStore.currentEmployee.firstName} ${employeeStore.currentEmployee.lastName}`
          )
        "
      />
    </div>

    <v-card v-if="allCurrentEmployeeVacations">
      <v-data-table
        :headers="vacationList"
        :items="sortedVacation"
        @click:row="handleClick"
        hover="true"
        no-data-text="Keine Urlaube verfügbar"
      >
        <template #[`item.employeeStatus`]="{ item }">
          <v-checkbox
            v-if="item.status == 6 || item.status == 4 || item.status == 5"
            v-model="item.employeeStatus"
            :disabled="item.employeeStatus == 4"
            :true-value="4"
            :false-value="6"
            style="
              display: flex;
              justify-content: center;
              align-items: center;
              color: var(--bold-main-blue);
            "
            @change="cancelVacation(item)"
          ></v-checkbox>
        </template>

        <template #[`item.delete`]="{ item }">
          <v-tooltip v-if="item.type == 'Betriebsurlaub'" location="end">
            <v-btn class="ma-2" size="small" color="white" icon="mdi-delete"></v-btn>
          </v-tooltip>

          <!-- Löschen möglich wenn Urlaub abgelehnt ist oder der Urlaub noch nicht von jemanden bearbeitet wurde  -->
          <v-tooltip
            text="Urlaub löschen"
            v-else-if="
              item.status == 2 ||
              (item.status == 3 &&
                item.adminStatus == 0 &&
                item.substituteStatus == 3 &&
                item.supervisorStatus == 3) ||
              (item.substitute == null &&
                item.status == 3 &&
                item.adminStatus == 0 &&
                item.substituteStatus == 1 &&
                item.supervisorStatus == 3)
            "
            location="end"
          >
            <template v-slot:activator="{ props }">
              <v-btn v-bind="props" variant="text" @click="showOverlayDelete(item)">
                <v-icon class="ma-2" size="small" color="red-lighten-2">mdi-delete</v-icon>
                <v-overlay
                  v-model="deleteItem"
                  class="align-center justify-center"
                  location-strategy="static"
                  scroll-strategy="none"
                >
                  <v-card
                    class="info-card"
                    title="Bist du dir sicher, dass du diesen Urlaub löschen möchtest? Du kannst dies nicht rückgängig machen."
                  >
                    <v-row class="justify-center">
                      <div class="confirm-button">
                        <v-btn @click="deleteVacation(item.vacationId)">Ja</v-btn>
                        <v-btn @click="deleteItem = null">Nein</v-btn>
                      </div>
                    </v-row>
                    <v-row></v-row>
                  </v-card>
                </v-overlay>
              </v-btn>
            </template>
          </v-tooltip>

          <!-- Stornierung möglich nachdem urlaub genehmigt wurde -->
          <v-tooltip
            text="Urlaub stornieren"
            v-else-if="item.status == 1 && new Date(item.startDate) >= new Date()"
            location="end"
          >
            <template v-slot:activator="{ props }">
              <v-btn v-bind="props" variant="text" @click="showOverlayCancel(item)">
                <v-icon class="ma-2" size="small" color="var(--status-error-delete)"
                  >mdi-cancel</v-icon
                >
                <v-overlay
                  v-model="cancelItem"
                  class="align-center justify-center"
                  location-strategy="static"
                  scroll-strategy="none"
                >
                  <v-card
                    class="info-card"
                    title="Bist du dir sicher, dass du diesen Urlaub stornieren möchtest? Du kannst dies nicht rückgängig machen."
                  >
                    <v-row class="justify-center">
                      <div class="confirm-button">
                        <v-btn @click="cancelVacation(item)">Ja</v-btn>
                        <v-btn @click="cancelItem = null">Nein</v-btn>
                      </div>
                    </v-row>
                    <v-row></v-row>
                  </v-card>
                </v-overlay>
              </v-btn>
            </template>
          </v-tooltip>
        </template>
        <template
          v-if="allCurrentEmployeeVacations != null && allCurrentEmployeeVacations.length < 10"
          #bottom
        >
        </template>
      </v-data-table>
      <vacation-detail-modal v-if="openDialog" :item="vacation" @closeDialog="closeDialog" />
    </v-card>
  </div>
</template>

<script setup>
import { useVacationStore } from '@/stores/vacation-store'
import { useEmployeeStore } from '@/stores/employee-store'
import { onMounted, ref } from 'vue'
import { useDateConverter } from '@/Composables/useDateConverter'
import VacationDetailModal from './VacationDetailModal.vue'
import { resolveVacationStatus } from '@/Composables/resolveVacationStatus'
import { sortVacationsByDate } from '@/Composables/sortVacationsByDate'
import { exportVacations } from '@/Composables/pdf/exportVacations'
import ExportButton from '@/components/BaseComponents/ExportButton.vue'

const emit = defineEmits(['refreshData'])

const vacationStore = useVacationStore()
const { actions } = vacationStore
const employeeStore = useEmployeeStore()
const { sortVacationsByStartDate } = sortVacationsByDate()
const { localizedDate } = useDateConverter()
const { enumToString } = resolveVacationStatus()

const componentKey = ref(0)

const forceRender = () => {
  componentKey.value += 1
}

const refreshData = async () => {
  const vacations = await actions.getVacationsByEmployeeIdDB(employeeStore.currentEmployee.id)
  const companyVacation = await actions.getCompanyVacations()
  allCurrentEmployeeVacations.value = vacations.concat(companyVacation)
  sortedVacation.value = sortVacationsByStartDate(allCurrentEmployeeVacations.value)
}

const allCurrentEmployeeVacations = ref(null)
let sortedVacation = ref(null)

const vacation = ref(null)

const deleteItem = ref(null)
const cancelItem = ref(null)

const openDialog = ref(false)

const closeDialog = () => {
  openDialog.value = false
}

const vacationList = ref([
  {
    title: 'Startdatum',
    key: 'startDate',
    align: 'center',
    sortable: true,
    value: (leave) => `${localizedDate(leave.startDate)}`
  },
  {
    title: 'Enddatum',
    key: 'endDate',
    align: 'center',
    sortable: true,
    value: (leave) => `${localizedDate(leave.endDate)}`
  },
  { title: 'Urlaubsart', key: 'type', align: 'center', sortable: false },
  { title: 'Anzahl der Tage', key: 'vacationdays', align: 'center', sortable: false },
  {
    title: 'Status',
    key: 'status',
    align: 'center',
    sortable: false,
    value: (leave) => `${enumToString(leave.status)}`
  },
  {
    title: 'Stornierung gesehen',
    key: 'employeeStatus',
    align: 'center',
    sortable: false,
    value: (leave) => `${enumToString(leave.employeeStatus)}`
  },
  { align: 'center', key: 'delete', sortable: false, value: 'delete' }
])

const handleClick = async (e, { item }) => {
  if (e.target.type == 'checkbox') {
    await cancelVacation(item)
  } else if (!deleteItem.value && !cancelItem.value) {
    vacation.value = item
    openDialog.value = true
  }
}

function showOverlayDelete(item) {
  deleteItem.value = item.vacationId
}

function showOverlayCancel(item) {
  cancelItem.value = item
}

const deleteVacation = async () => {
  await actions.deleteVacation(deleteItem.value)
  deleteItem.value = null
  await refreshData()
  forceRender()
  emit('refreshData')
}

const cancelVacation = async (item) => {
  if (cancelItem.value != null) {
    cancelItem.value.employeeStatus = 4
    await actions.cancelVacation(cancelItem.value)
  } else {
    await actions.cancelVacation(item)
  }

  showOverlayCancel.value = false
  await refreshData()
  forceRender()
  emit('refreshData')
}

onMounted(async () => {
  await refreshData()
})
</script>

<style lang="scss" src="./VacationsOverview.scss" scoped />
