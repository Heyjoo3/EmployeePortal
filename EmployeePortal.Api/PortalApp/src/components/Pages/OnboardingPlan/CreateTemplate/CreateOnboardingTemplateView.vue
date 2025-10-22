<template>

    <div class="header">
        <v-text-field
            label="Vorlagenname"
            v-model="templateName"
            variant="outlined"
            density="compact"
        ></v-text-field>
        <v-btn color="success" @click="console.log('Save button clicked')"> Speichern </v-btn>
    </div>
    <div class="template-page">
    <OnboardingTaskGroups />
    </div>
</template>

<script setup>
import OnboardingTaskGroups from './OnboardingTaskGroups.vue';
</script>

<style scoped>
.header {
   margin-top: 30px;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 20px;
  gap: 20px;
  position: sticky;
  top: 50px;
  z-index: 10;
  padding-top: 20px;
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