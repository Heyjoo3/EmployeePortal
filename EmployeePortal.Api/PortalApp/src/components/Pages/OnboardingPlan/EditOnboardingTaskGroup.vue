<template>
  <BaseModal
    :title="title"
    :show-close="true"
    :banner-background-color="bannerBackgroundColor"
    :banner-icon="bannerIcon"
    :banner-text="bannerText"
    :save-button-text="Speichern"
    @close="$emit('close')"
    @submit="onSubmit"
  >
    <template #form-content>
      <v-form ref="taskGroupForm">
        <v-text-field
          label="Aufgabengruppenname"
          v-model="taskGroupName"
          required
          density="compact"
          variant="outlined"
          :rules="nameRules"
        ></v-text-field>

        <v-select
          label="Ansprechpartner"
          v-model="contactPerson"
          :items="contactPersonOptions"
          item-title="text"
          item-value="value"
          density="compact"
          variant="outlined"
          clearable
        ></v-select>
      </v-form>
    </template>
  </BaseModal>
</template>
<script setup>
import BaseModal from '@/components/BaseComponents/BaseModal.vue'
import { useEmployeeStore } from '@/stores/employee-store'
import { useOnboardingStore } from '@/stores/onboarding-store'
import { onMounted, ref, computed } from 'vue'

const employeeStore = useEmployeeStore()
const onboardingStore = useOnboardingStore()

const contactPersonOptions = ref([])
const taskGroupForm = ref(null)

const props = defineProps({
  selectedGroup: {
    type: Object,
    default: null,
  },
})

const emit = defineEmits(['close', 'submit'])

const nameRules = [
  (v) => !!v || 'Der Name der Aufgabengruppe ist erforderlich.',
  (v) =>
    (v && v.length <= 100) ||
    'Der Name der Aufgabengruppe darf maximal 100 Zeichen lang sein.',
]



const taskGroupName = ref(props.selectedGroup ? props.selectedGroup.name : '')
const contactPerson = ref(props.selectedGroup ? props.selectedGroup.contactPerson : '')

const bannerIcon = props.selectedGroup ? 'mdi-pencil' : 'mdi-plus'
const bannerText = props.selectedGroup ? 'Aufgabengruppe bearbeiten' : 'Neue Aufgabengruppe erstellen'
const bannerBackgroundColor = props.selectedGroup
  ? 'var(--status-inactive-update)'
  : 'var(--status-active-add)'
const title = props.selectedGroup ? 'Aufgabengruppe bearbeiten' : 'Neue Aufgabengruppe erstellen'


const contactPersonName = computed(() => {
  const opt = contactPersonOptions.value.find((o) => o.value === contactPerson.value)
  return opt ? opt.text : (props.selectedGroup?.contactPersonName ?? '')
})

const onSubmit = async () => {

  console.log('Submitting Task Group Form')
  // validate form
  const valid = taskGroupForm.value ? await taskGroupForm.value.validate() : true
  if (!valid.valid) {
    // validation messages are shown by the field rules
    return
  }

  const group = {
    id: props.selectedGroup ? props.selectedGroup.id : null,
    title: taskGroupName.value,
    contactPerson: contactPerson.value,
    contactPersonName: contactPersonName.value,
  }

  console.log('Saving Task Group Component: ', group)

  onboardingStore.actions.saveTaskGroup(group)
  emit('close')

  // emit payload to parent
  // emit('submit', {
  //   id: props.selectedGroup ? props.selectedGroup.id : null,
  //   name: taskGroupName.value,
  //   contactPerson: contactPerson.value,
  //   contactPersonName: contactPersonName.value,
  // })
}

onMounted(() => {
  const employees = employeeStore.getEmployees
  contactPersonOptions.value = employees.map((emp) => ({
    text: emp.name,
    value: emp.id,
  })).sort((a, b) => a.text.localeCompare(b.text))
  console.log('Loaded employees:', contactPersonOptions.value)
})
</script>
