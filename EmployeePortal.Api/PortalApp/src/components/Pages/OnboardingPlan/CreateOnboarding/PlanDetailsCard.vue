<template>
    <div class="plan-details">
        <v-row style="width: 100%; ">
            <v-col cols="3">
                        <base-date-picker label="Startdatum" :density="'compact'" :value="startDate" @input="startDate = $event" />
            </v-col>
            <v-col cols="3">
        <base-date-picker label="Enddatum" :density="'compact'" :value="endDate" @input="endDate = $event" />

            </v-col>
            <v-col cols="6">
                <v-select label="Ansprechpartner" variant="outlined" density="compact" :items="employees"
                    item-title="name" item-value="id" v-model="selectedBuddyId" />
            </v-col>
        </v-row>

        <v-row class="templates" style="width: 100%;">
            <v-col>
                <v-select v-model="selectedTemplates" label="Vorlage auswählen" variant="outlined" density="compact"
                    :items="[
                        { title: 'Onboarding Plan Vorlage 1', id: 1 },
                        { title: 'Onboarding Plan Vorlage 2', id: 2 }
                    ]" multiple chips clearable>
                    <template #chip="{ item }">
                        <v-chip>
                            {{ item.title }}
                            <v-btn icon density="compact" size="x-small" @mousedown.stop
                                @click.stop="showTemplateInfo(item)" style="margin-left: 4px; margin-right: 2px">
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
</template>

<script setup>
import BaseDatePicker from '@/components/BaseComponents/BaseDatePicker.vue'
import { useEmployeeStore } from '@/stores/employee-store'
import { ref } from 'vue'

const employeeStore = useEmployeeStore()
const employees = ref(employeeStore.state.employees)

const startDate = ref(new Date())
const endDate = ref(new Date(Date.now() + 28 * 24 * 60 * 60 * 1000))
const selectedTemplates = ref([])
const selectedBuddyId = ref(null)

const showTemplateInfo = (template) => {
    console.log('Show info for template:', template)
}

defineExpose({
  startDate,
  endDate,
  selectedTemplates,
  selectedBuddyId,
  getValues: () => ({
    startDate: startDate.value,
    endDate: endDate.value,
    selectedTemplates: selectedTemplates.value,
    selectedBuddyId: selectedBuddyId.value
  })
})
</script>

<style scoped>
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