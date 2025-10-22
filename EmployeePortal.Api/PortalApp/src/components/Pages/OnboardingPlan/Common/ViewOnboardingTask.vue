<template>
  <BaseModal
    :title="title"
    :show-close="true"
    :banner-background-color="bannerBackgroundColor"
    :banner-icon="bannerIcon"
    :banner-text="bannerText"
    :save-button-text="saveButtonText"
    @close="$emit('close')"
    @submit="$emit('submit')"
  >
    <template #form-content>
      <v-form>
        <v-row>
          <v-col cols="8">
            <p><strong>Aufgabenname:</strong> {{ taskName }}</p>
          </v-col>
          <v-col cols="4">
            <p><strong>Aufgabenart:</strong> {{ taskType }}</p>
          </v-col>
        </v-row>

        <div v-show="taskType === 'Todo'">
          <v-row>
            <v-col cols="12">
              <p><strong>Beschreibung:</strong></p>
              <br />
              <p>{{ taskDescription }}</p>
            </v-col>
          </v-row>
        </div>

        <div v-show="taskType === 'TimeScheduled'">
          <v-row>
            <v-col cols="6">
              <p><strong>Startdatum:</strong></p>
              <p>{{ startDate }}</p>
            </v-col>
            <v-col cols="6">
              <p><strong>Enddatum:</strong></p>
              <p>{{ endDate }}</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <p><strong>Beschreibung:</strong></p>
              <br />
              <p>{{ taskDescription }}</p>
            </v-col>
          </v-row>
        </div>

        <div v-show="taskType === 'Networking'">
          <v-row>
            <v-col cols="6">
              <p><strong>Startdatum:</strong></p>
              <p>{{ startDate }}</p>
            </v-col>
            <v-col cols="6">
              <p><strong>Ort:</strong></p>
              <p>{{ location }}</p>
            </v-col>
            <v-col cols="12">
              <p><strong>Partner:</strong></p>
              <p>{{ partner }}</p>
            </v-col>
            <v-col cols="12">
              <p><strong>Beispielfragen:</strong></p>
              <br />
              <p>{{ exampleQuestions }}</p>
            </v-col>
          </v-row>
        </div>

        <div v-show="taskType === 'Project'">
          <p><strong>Priorität:</strong></p>
          <p>{{ priority }}</p>

          <p><strong>Material:</strong></p>
          <p>{{ material }}</p>

          <p><strong>Beschreibung:</strong></p>
          <br />
          <p>{{ taskDescription }}</p>
        </div>
      </v-form>
    </template>
  </BaseModal>
</template>

<script setup>
import BaseModal from '@/components/BaseComponents/BaseModal.vue'
import { ref } from 'vue'

// Props
const props = defineProps({
  selectedTask: String
})

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
</script>
