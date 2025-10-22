<template>
  <base-modal @close="$emit('close')" title="Aufgabendetails bearbeiten">
    <template #form-content>
      <v-row style="width: 100%">
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
      </v-row>
    </template>
  </base-modal>
</template>

<script setup>
import BaseDatePicker from '@/components/BaseComponents/BaseDatePicker.vue'
import BaseModal from '@/components/BaseComponents/BaseModal.vue'

const props = defineProps({
  plan: {
    type: Object,
    default: null,
  },
})
</script>
