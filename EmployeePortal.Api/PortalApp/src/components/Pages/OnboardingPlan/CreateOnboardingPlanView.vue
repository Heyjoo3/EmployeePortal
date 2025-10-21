<template>
  <div class="header">
    <h1>Onboarding Plan</h1>
    <v-select
      label="Neuer Mitarbeiter"
      variant="underlined"
      item-color="red"
      single-line
      density="compact"
      class="employee-selection"
      :items="employees"
      item-title="name"
      item-value="id"
      v-model="selectedEmployeeId"
    />
    <v-btn color="success" @click="saveOnboardingPlan"> Speichern </v-btn>
  </div>
  <div class="onboarding-page">
    <div class="plan-details">
      <v-row  style="width: 100%; ">
        <v-col cols="3">
          <base-date-picker label="Startdatum" :value="new Date()" :density="'compact'" />
        </v-col>
        <v-col cols="3">
          <base-date-picker
            label="Enddatum"
            :value="new Date(Date.now() + 28 * 24 * 60 * 60 * 1000)"
            :density="'compact'"
          />
        </v-col>
        <v-col cols="6">
          <v-select label="Ansprechpartner" variant="outlined" density="compact" />
        </v-col>
      </v-row>

      <v-row class="templates" style="width: 100%;">
        <v-col>
          <v-select
            v-model="selectedTemplates"
            label="Vorlage auswählen"
            variant="outlined"
            density="compact"
            :items="[
              { title: 'Onboarding Plan Vorlage 1', id: 1 },
              { title: 'Onboarding Plan Vorlage 2', id: 2 }
            ]"
            multiple
            chips
            clearable
          >
            <template #chip="{ item }">
              <v-chip>
                {{ item.title }}
                <v-btn
                  icon
                  density="compact"
                  size="x-small"
                  @mousedown.stop
                  @click.stop="showTemplateInfo(item)"
                  style="margin-left: 4px; margin-right: 2px"
                >
                  <v-icon size="16">mdi-information</v-icon>
                </v-btn>
              </v-chip>
            </template>
          </v-select>
        </v-col>
        
        <!-- <div v-if="selectedTemplates.length" class="selected-templates-list" style="margin-top: 10px;">
        <div
          v-for="(template, idx) in selectedTemplates"
          :key="template"
          style="display: flex; align-items: center; gap: 8px; margin-bottom: 4px;"
        >
          <span>{{ template }}</span>
          <v-btn
        icon
        density="compact"
        size="small"
        @click="showTemplateInfo(template)"
          >
        <v-icon size="18">mdi-information</v-icon>
          </v-btn>
        </div>
      </div> -->
      </v-row>
    </div>

    <div class="task-groups">
      <v-btn color="primary" width="100%" class="mb-3" @click="openEditTaskGroupDialog(true, null)"
        >+ Aufgabengruppe hinzufügen</v-btn
      >

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
            <v-btn color="primary" width="100%" @click="openEditTaskDialog(true, null)"
              >+ Aufgabe hinzufügen</v-btn
            >
            <v-list v-for="i in 30" :key="i" bg-color="var(--very-light-blue)">
              <div class="list-item">
                <div class="item-title">Aufgabe</div>
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
import BaseDatePicker from '@/components/BaseComponents/BaseDatePicker.vue'
import EditOnboardingTask from '@/components/Pages/OnboardingPlan/EditOnboardingTask.vue'
import EditOnboardingTaskGroup from '@/components/Pages/OnboardingPlan/EditOnboardingTaskGroup.vue'
import ViewOnboardingTask from './ViewOnboardingTask.vue'
import { useOnboardingStore } from '@/stores/onboarding-store'
import { useEmployeeStore } from '@/stores/employee-store'
import { ref } from 'vue'

const onboardingStore = useOnboardingStore()
const employeeStore = useEmployeeStore()

const employees = ref(employeeStore.state.employees)

const showEditTaskDialog = ref(false)
const showViewTaskDialog = ref(false)
const showEditTaskGroupDialog = ref(false)
const selectedTask = ref(null)
const selectedGroup = ref(null)
const selectedEmployeeId = ref(null)

const showTemplateInfo = (template) => {
  console.log('Show info for template:', template)
}

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

const saveOnboardingPlan = async () => {

  

    const onboardingPlan = {
      employeeId: selectedEmployeeId.value, // Replace with actual selected employee
      startDate: new Date(), // Replace with actual start date
      title: 'Onboarding Plan Title', // Replace with actual title
      // Add other necessary fields
    }

    await onboardingStore.actions.createOnboardingPlan(onboardingPlan)
    console.log('Onboarding plan saved:', onboardingPlan)
}

const selectedTemplates = ref([])
</script>

<style scoped>
.onboarding-page {
  width: 90%;
  margin: auto;
  margin-top: 0px;
}

.header {
  margin-top: 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
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
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  background-color: var(--very-light-blue);
  padding: 16px;
  border-radius: 4px;
  box-shadow:
    0px 3px 1px -2px var(--v-shadow-key-umbra-opacity, rgba(0, 0, 0, 0.2)),
    0px 2px 2px 0px var(--v-shadow-key-penumbra-opacity, rgba(0, 0, 0, 0.14)),
    0px 1px 5px 0px var(--v-shadow-key-ambient-opacity, rgba(0, 0, 0, 0.12));
}
</style>
