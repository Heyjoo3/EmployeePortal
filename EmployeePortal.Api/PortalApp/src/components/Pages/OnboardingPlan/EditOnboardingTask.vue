<template>
  <BaseModal
    :title="title"
    :show-close="true"
    :banner-background-color="bannerBackgroundColor"
    :banner-icon="bannerIcon"
    :banner-text="bannerText"
    :save-button-text="saveButtonText"
    @close="$emit('close')"
    @submit="handleSaveTask()"
  >
    <template #form-content>
      <v-form ref="taskForm">
        <v-row>
          <v-col cols="8">
            <v-text-field
              label="Aufgabenname"
              v-model="taskName"
              required
              density="compact"
              variant="outlined"
              :rules="nameRules"
            ></v-text-field>
          </v-col>
          <v-col cols="4">
            <v-select
              label="Aufgabenart"
              :items="['Networking', 'Project', 'Todo', 'TimeScheduled']"
              v-model="taskType"
              density="compact"
              variant="outlined"
              required
              :rules="[(v) => !!v || 'Die Aufgabenart ist erforderlich.']"
            ></v-select>
          </v-col>
        </v-row>

        <div v-show="taskType === 'Todo'">
          <v-row>
            <v-col cols="12">
              <v-textarea
                label="Beschreibung"
                v-model="taskDescription"
                rows="3"
                density="compact"
                variant="outlined"
              ></v-textarea>
            </v-col>
          </v-row>
        </div>

        <div v-show="taskType === 'TimeScheduled'">
          <v-row>
            <v-col cols="6">
              <base-date-picker label="Startdatum" :density="'compact'" />
            </v-col>
            <v-col cols="6">
              <base-date-picker label="Enddatum" :density="'compact'" />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <v-textarea
                label="Beschreibung"
                v-model="taskDescription"
                rows="3"
                density="compact"
                variant="outlined"
              ></v-textarea>
            </v-col>
          </v-row>
        </div>

        <div v-show="taskType === 'Networking'">
          <!-- startdate, duedate, partner, place, example questions -->
          <v-row>
            <v-col cols="6">
              <base-date-picker label="Startdatum" :density="'compact'" />
            </v-col>
            <v-col cols="6">
              <v-select
                label="Partner"
                :items="['Partner A', 'Partner B', 'Partner C']"
                variant="outlined"
                density="compact"
              ></v-select>
            </v-col>
            <v-col cols="12">
              <v-text-field label="Ort" density="compact" variant="outlined"></v-text-field>
            </v-col>
            <v-col cols="12">
              <v-textarea
                label="Beispielfragen"
                rows="3"
                density="compact"
                variant="outlined"
              ></v-textarea>
            </v-col>
          </v-row>
        </div>

        <div v-show="taskType === 'Project'">
          <!-- priority, tools, description -->
          <v-select
            label="Priorität"
            :items="['Hoch', 'Mittel', 'Niedrig']"
            v-model="priority"
            variant="outlined"
            density="compact"
          ></v-select>

          <v-text-field label="Material" density="compact" variant="outlined"></v-text-field>

          <v-textarea
            label="Beschreibung"
            v-model="taskDescription"
            rows="3"
            density="compact"
            variant="outlined"
          ></v-textarea>
        </div>
      </v-form>
    </template>
  </BaseModal>
</template>

<script setup>
import BaseModal from '@/components/BaseComponents/BaseModal.vue'
import BaseDatePicker from '@/components/BaseComponents/BaseDatePicker.vue'
import { onMounted, ref } from 'vue'
import { useOnboardingStore } from '@/stores/onboarding-store'

const onboardingStore = useOnboardingStore()

// Props
const props = defineProps({
  selectedTask: String,
  taskGroupId: String
})

const emit = defineEmits(['close', 'submit'])

const nameRules = [
  (v) => !!v || 'Der Name der Aufgabe ist erforderlich.',
  (v) =>
    (v && v.length <= 100) || 'Der Name der Aufgabe darf maximal 100 Zeichen lang sein.',
]

const handleSaveTask = async () => {
  const valid = taskForm.value ? await taskForm.value.validate() : true

  console.log('Form valid:', valid)
  if (!valid.valid) {
    // validation messages are shown by the field rules
    console.log('Form is invalid, not submitting')
    return
  }

  let taskData = {
    title: taskName.value,
    description: taskDescription.value,
    taskType: taskType.value,
  }

  if (taskType.value === 'TimeScheduled') {
    taskData.startDate = startDate.value
    taskData.endDate = endDate.value
  } else if (taskType.value === 'Networking') {
    taskData.startDate = networkingStartDate.value
    taskData.partner = partner.value
    taskData.place = place.value
    taskData.exampleQuestions = exampleQuestions.value
  } else if (taskType.value === 'Project') {
    taskData.priority = priority.value
    taskData.material = material.value
  }

  if (props.selectedTask) {
    taskData.id = props.selectedTask 
  }

  onboardingStore.actions.saveTask(props.taskGroupId, taskData)
  emit('submit')

}

const startDate = ref(null)
const endDate = ref(null)
const networkingStartDate = ref(null)
const partner = ref('')
const place = ref('')
const exampleQuestions = ref('')
const priority = ref('')
const material = ref('')

const bannerIcon = props.selectedTask ? 'mdi-pencil' : 'mdi-plus'
const bannerText = props.selectedTask ? 'Aufgabe bearbeiten' : 'Neue Aufgabe erstellen'
const bannerBackgroundColor = props.selectedTask
  ? 'var(--status-inactive-update)'
  : 'var(--status-active-add)'
const saveButtonText = props.selectedTask ? 'Änderungen speichern' : 'Aufgabe speichern'
const title = props.selectedTask ? 'Aufgabe bearbeiten' : 'Neue Aufgabe'

const taskName = ref(props.selectedTask || '')
const taskDescription = ref('')
const taskType = ref('')
const taskForm = ref(null)

onMounted(() => {
  console.log('EditOnboardingTask mounted with selectedTask:', props.selectedTask)
  console.log('Task Group ID:', props.taskGroupId)
})
</script>
