<template>
  <div class="personnel-avatar">
    <img src="/src/assets/contact.png" width="280" height="272" />
    <div class="buttons" v-if="props.userRole === 'Admin'">
      <v-btn @click="$emit('onOpenEditProfile')"> Profil bearbeiten </v-btn>
      <v-btn @click="$emit('onOpenAddAbsence')"> Abwesend melden </v-btn>
    </div>
    <div class="buttons" v-else>
      <v-btn @click="$emit('onOpenEditCredentials')"> Anmeldedaten bearbeiten </v-btn>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  userRole: String
})
</script>

<style scoped>
.personnel-avatar {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.buttons {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 10px;
}
.buttons button {
  width: 280px;
  color: var(--white);
  background-color: var(--primary-blue);
}
</style>
