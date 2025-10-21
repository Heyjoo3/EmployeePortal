<template>
  <BaseModal
    :dialog="detailsDialog"
    title="Urlaubsdetails"
    bannerIcon="mdi-calendar"
    bannerText="Details"
    text-color="var(--primary-blue)"
    :formMode="false"
    @close="closeDialog"
  >
    <template #static-content>
      <v-row>
        <v-col>
          <p><b>Startdatum: </b></p>
          <p>{{ localizedDate(item.startDate) }}</p>
          <p v-if="item.isHalfStartDay">Halbtägig</p>
          <p v-else>Ganztägig</p>
        </v-col>
        <v-col>
          <p><b>Enddatum: </b></p>
          <p>
            {{ localizedDate(item.endDate) }}
          </p>
          <p v-if="item.isHalfEndDay">Halbtägig</p>
          <p v-else>Ganztägig</p>
        </v-col>
        <v-col>
          <p><b>Urlaubstage:</b> {{ item.vacationdays }}</p>
          <p><b>Urlaubsart:</b>{{ item.type }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col
          ><p><b>Gesamtstatus:</b>{{ enumToString(item.status) }}</p></v-col
        >
      </v-row>
      <v-row>
        <v-col>
          <p><b>Vorgesetzter:</b> {{ supervisor }}</p>
          <p><b>Status:</b>{{ enumToString(item.supervisorStatus) }}</p>
        </v-col>
        <v-col>
          <p><b>Vertreter:</b>{{ substitute }}</p>
          <p><b>Status:</b> {{ enumToString(item.substituteStatus) }}</p>
        </v-col>
        <v-col>
          <p><b>Admin Status:</b> {{ enumToString(item.adminStatus) }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p><b>Beschreibung:</b> {{ description }}</p>
        </v-col>
      </v-row>
    </template>
  </BaseModal>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import BaseModal from '../../BaseComponents/BaseModal.vue'
import { useDateConverter } from '@/Composables/useDateConverter'
import { resolveVacationStatus } from '@/Composables/resolveVacationStatus'
import { useEmployeeStore } from '@/stores/employee-store'

const props = defineProps({
  item: null
})

const emit = defineEmits(['closeDialog'])

const localizedDate = useDateConverter().localizedDate
const enumToString = resolveVacationStatus().enumToString
const employeeStore = useEmployeeStore()

const supervisor = ref(null)
const substitute = ref(null)
const description = ref(null)

const dialog = ref(false)

const closeDialog = () => {
  emit('closeDialog')
}

onMounted(async () => {
  dialog.value = true
  supervisor.value = props.item.supervisor
    ? await employeeStore.actions.getNameById(props.item.supervisor)
    : 'Kein Vorgesetzter'
  substitute.value = props.item.substitute
    ? await employeeStore.actions.getNameById(props.item.substitute)
    : 'Kein Vertreter'
  description.value = props.item.description || 'Keine Beschreibung'
})
</script>
