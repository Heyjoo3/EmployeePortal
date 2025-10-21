<template>
  <v-row class="form-actions">
    <v-col>
      <v-btn to="/userprofile" variant="tonal"> Abbrechen </v-btn>
    </v-col>
    <v-col>
      <v-btn
        variant="tonal"
        type="submit"
        class="create-button"
        :disabled="!props.form"
        :loading="props.loading"
      >
        Erstellen
      </v-btn>
    </v-col>
  </v-row>
</template>

<script setup>
const props = defineProps(['form', 'loading'])
</script>

<style lang="scss" src="./CreateUser.scss" scoped />
