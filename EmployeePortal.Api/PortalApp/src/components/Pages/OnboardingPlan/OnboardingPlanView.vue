<template>
  <div class="header">
    <h1>Onboarding Plan - <strong>Max Mustermann</strong></h1>

    <v-btn color="primary" @click="openEditDetailsDialog(true, null)"> Bearbeiten </v-btn>
  </div>
  <div class="onboardingPage">
    <div class="plan-details" style="display: flex; justify-content: space-between; align-items: center; padding: 10px 30px ;">
      <div>
        <p><strong>Startdatum:</strong> 01.10.2025</p>
        <p><strong>Enddatum:</strong> 31.10.2025</p>
        <p><strong>Ansprechpartner:</strong> Lisa Schulz</p>
      </div>

      <div class="status">
          <h4><strong>Status</strong> In Bearbeitung</h4>
          <h4><strong>Fortschritt:</strong> 33%</h4>
      </div>
    </div>

    <div class="task-groups">
      <!-- <v-btn color="primary" width="100%" class="mb-3" @click="openEditTaskGroupDialog(true, null)"
        >+ Aufgabengruppe hinzufügen</v-btn
      > -->

      <v-expansion-panels>
        <v-expansion-panel bg-color="var(--very-light-blue)">
          <v-expansion-panel-title>
            <div class="header-taskgroup">
              <div>
                <h2>Aufgabengruppe</h2>
                <br />
                <p><strong>Ansprechpartner:</strong> Max Mustermann</p>
              </div>
              <div class="buttons-taskgroup">
                <v-btn
                  :color="'var(--status-inactive-update)'"
                  density="compact"
                  icon
                  rounded="true"
                  @click.stop="openEditTaskGroupDialog(true, null)"
                >
                  <v-icon size="18" color="white">mdi-pencil</v-icon>
                </v-btn>
                <v-btn
                  color="error"
                  density="compact"
                  icon="mdi-delete"
                  rounded="true"
                  @click.stop="console.log('Delete clicked')"
                ></v-btn>
              </div>
            </div>
          </v-expansion-panel-title>
          <v-expansion-panel-text>
            <!-- <v-btn color="primary" width="100%" @click="openEditTaskDialog(true, null)"
              >+ Aufgabe hinzufügen</v-btn
            > -->
            <v-list v-for="i in 3" :key="i" bg-color="var(--very-light-blue)">
              <div class="list-item">
                <div class="item-title">
                  Aufgabe {{ i }}
                  <br />
                  <p>Status: <strong :style="{ color: i % 2 === 0 ? '#ec8c0e' : '#28a745' }">{{ i % 2 === 0 ? 'In Bearbeitung' : 'Abgeschlossen' }}
                  </strong>
                  </p>
                </div>
                <div class="buttons">
                  <v-btn
                    :color="'primary'"
                    density="compact"
                    icon
                    rounded="true"
                    @click="openViewTaskDialog(true, 'Aufgabe1')"
                  >
                    <v-icon size="18" color="white">mdi-eye</v-icon>
                  </v-btn>
                  <v-btn
                    :color="'var(--status-inactive-update)'"
                    density="compact"
                    icon
                    rounded="true"
                    @click="openEditTaskDialog(true, 'Aufgabe1')"
                  >
                    <v-icon size="18" color="white">mdi-pencil</v-icon>
                  </v-btn>
                  <v-btn
                    color="error"
                    density="compact"
                    icon="mdi-delete"
                    rounded="true"
                    @click="console.log('Delete clicked')"
                  ></v-btn>
                </div>
              </div>
            </v-list>
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>
    </div>
  </div>
  <EditOnboardingPlanDetails
    v-if="showEditDetailsDialog"
    @close="openEditDetailsDialog(false, null)"
    :plan="plan"
  />
  <EditOnboardingTask
    v-if="showEditTaskDialog"
    @close="openEditTaskDialog(false, null)"
    :selectedTask="selectedTask"
  />
  <ViewOnboardingTask
    v-if="showViewTaskDialog"
    @close="openViewTaskDialog(false, null)"
    :selectedTask="selectedTask"
  />
  <EditOnboardingTaskGroup
    v-if="showEditTaskGroupDialog"
    @close="openEditTaskGroupDialog(false, null)"
    :selectedGroup="selectedGroup"
  />
</template>

<script setup>
import EditOnboardingPlanDetails from './AddEditDialogs/EditOnboardingPlanDetails.vue'
import EditOnboardingTask from '@/components/Pages/OnboardingPlan/AddEditDialogs/EditOnboardingTask.vue'
import EditOnboardingTaskGroup from '@/components/Pages/OnboardingPlan/AddEditDialogs/EditOnboardingTaskGroup.vue'
import ViewOnboardingTask from './Common/ViewOnboardingTask.vue'
import { ref } from 'vue'

const showEditDetailsDialog = ref(false)
const showEditTaskDialog = ref(false)
const showViewTaskDialog = ref(false)
const showEditTaskGroupDialog = ref(false)
const selectedTask = ref(null)
const selectedGroup = ref(null)
const plan = ref({
  status: 'In Bearbeitung',
  progress: 100,
  startDate: '01.10.2023',
  endDate: '31.10.2023',
  contactPerson: 'Peter Müller'
})

// const showTemplateInfo = (template) => {
//   console.log('Show info for template:', template)
// }

const openEditTaskDialog = (show, task) => {
  showEditTaskDialog.value = show
  console.log('Open Edit Task Dialog:', show, task)
}

const openViewTaskDialog = (show, task) => {
  showViewTaskDialog.value = show
  console.log('Open View Task Dialog:', show, task)
}

const openEditTaskGroupDialog = (show, group) => {
  showEditTaskGroupDialog.value = show
  console.log('Open Edit Task Group Dialog:', show, group)
}

const openEditDetailsDialog = (show) => {
  showEditDetailsDialog.value = show
  console.log('Open Edit Details Dialog:', show)
}

// const selectedTemplates = ref([])
</script>

<style scoped>
.onboardingPage {
  width: 90%;
  margin: auto;
  margin-top: 20px;
}

.header {
  margin-top: 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
  position: sticky;
  top: 40px;
  z-index: 10;
  padding-top: 20px;
  background-color: white;
  padding-left: 5%;
  padding-right: 5%;
}

.employee-selection :deep(.v-select__selection-text) {
  color: red !important;
  font-size: 28px;
  font-weight: 500;
}

.task-groups {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.list-item {
  padding: 8px 16px;
  border: 1px solid rgb(205, 205, 218);
  border-radius: 8px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: white;
}

.item-title {
  font-size: 16px;
  width: 80%;
}

.buttons {
  display: flex;
  gap: 10px;
  width: 104px;
}

.header-taskgroup {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-right: 28px;
}

.buttons-taskgroup {
  display: flex;
  gap: 10px;
  width: 72px;
}

.plan-details {
  margin-bottom: 30px;
  background-color: var(--very-light-blue);
  padding: 16px;
  border-radius: 4px;
  box-shadow:
    0px 3px 1px -2px var(--v-shadow-key-umbra-opacity, rgba(0, 0, 0, 0.2)),
    0px 2px 2px 0px var(--v-shadow-key-penumbra-opacity, rgba(0, 0, 0, 0.14)),
    0px 1px 5px 0px var(--v-shadow-key-ambient-opacity, rgba(0, 0, 0, 0.12));
}

.status {

 border-radius: 4px;
 padding: 8px 12px;
  background-color: #ec8c0e;
  color: white;
}
</style>
