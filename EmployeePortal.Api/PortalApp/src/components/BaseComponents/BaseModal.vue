<template>
  <v-dialog v-model="dialog" class="dialogClass" @click:outside="closeDialog">
    <v-card class="card-dialog" :style="{ color: props.textColor }">
      <v-card-title class="dialog-title">
        <span>{{ props.title }}</span>
        <div class="close-button-div">
          <v-btn class="close-button" color="primary" @click="closeDialog">
            <v-icon small>mdi-window-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-row
        no-gutters
        justify="start"
        class="banner"
        :style="{ backgroundColor: `${props.bannerBackgroundColor} !important` }"
      >
        <v-col>
          <v-icon class="banner-icon" left>{{ props.bannerIcon }}</v-icon>
          <span class="banner-text">{{ props.bannerText }}</span>
        </v-col>
      </v-row>

      <!-- Bedingte Anzeige: Formular oder statische Informationen -->
      <v-form v-if="props.formMode" ref="formRef" @submit.prevent="submitForm" class="inner-form">
        <slot name="form-content"></slot>
        <div class="buttons">
          <v-btn v-if="props.deleteButton" class="text-none delete-button" @click="emit('delete')"
            >Löschen</v-btn
          >
          <v-btn class="text-none save-button" @click="submitForm">{{
            props.saveButtonText
          }}</v-btn>
        </div>
      </v-form>

      <div v-else class="inner-content">
        <slot name="static-content"></slot>
      </div>
    </v-card>
  </v-dialog>
</template>

<script setup>
import { onMounted, ref } from 'vue'

const dialog = ref(false)

const emit = defineEmits(['close', 'submit', 'delete'])

const props = defineProps({
  title: {
    type: String,
    default: 'Action'
  },
  bannerIcon: {
    type: String,
    default: 'mdi-plus'
  },
  bannerText: {
    type: String,
    default: 'Hinzufügen'
  },
  saveButtonText: {
    type: String,
    default: 'Speichern'
  },
  formMode: {
    type: Boolean,
    default: true
  },
  deleteButton: {
    type: Boolean,
    default: false
  },
  bannerBackgroundColor: {
    type: String,
    default: 'var(--status-active-add)'
  },
  textColor: {
    type: String,
    default: 'var(--dark-text)'
  }
})

const formRef = ref(null)

function closeDialog() {
  emit('close')
}

function submitForm() {
  emit('submit')
}

onMounted(() => {
  dialog.value = true
})
</script>

<style lang="scss" src="./BaseModal.scss" scoped />
