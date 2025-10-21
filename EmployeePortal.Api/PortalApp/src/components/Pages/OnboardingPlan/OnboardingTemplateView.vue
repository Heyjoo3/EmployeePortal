<template>
  <div class="header">
    <h1 v-if="!isEditing">Vorlagentitel</h1>
    <!-- pencil icon cliackable to edit name and activate textfield -->
    <v-text-field
      v-if="isEditing"
      label="Vorlagenname"
      v-model="templateName"
      variant="solo"
      density="compact"
      @click:append-inner="isEditing = false"
    >
      <template #append>
        <v-icon color="var(--status-inactive-update)" @click="isEditing = false">mdi-content-save</v-icon>
        <v-icon color="var(--status-error-delete)" @click="isEditing = false">mdi-close</v-icon>
      </template>
    </v-text-field>
      
    <div class="buttons">
    <v-icon v-if="!isEditing" color="var(--status-inactive-update)" @click="isEditing = true">mdi-pencil</v-icon>
   
    </div>
  </div>
  <div class="template-page">
    <OnboardingTaskGroups :editMode="true" />
  </div>
</template>

<script setup>
import OnboardingTaskGroups from './OnboardingTaskGroups.vue'
import { ref } from 'vue'

const templateName = ref('')
const isEditing = ref(false)
</script>

<style scoped>
.header {
  margin-top: 30px;
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  margin-bottom: 20px;
  gap: 20px;
  position: sticky;
  top: 60px;
  z-index: 10;
  background-color: white;
  padding-left: 5%;
  padding-right: 5%;
}

.template-page {
  width: 90%;
  margin: auto;
  margin-top: 0px;
}
</style>
