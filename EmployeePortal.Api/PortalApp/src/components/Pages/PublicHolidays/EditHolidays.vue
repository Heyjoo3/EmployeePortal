<template>
  <div class="vacation-days-edit">
    <h2>Gesetzliche Feiertage</h2>
    <v-card>
      <v-tabs
        v-model="tab"
        align-tabs="center"
        bg-color="var(--primary-blue)"
        color="var(--white)"
        stacked
      >
        <v-tab
          v-for="year in allHolidayYears"
          :key="year"
          :text="year"
          :value="'tab-' + year"
        ></v-tab>
      </v-tabs>

      <v-tabs-window v-model="tab">
        <v-tabs-window-item v-for="year in allHolidayYears" :key="year" :value="'tab-' + year">
          <div class="public-holidays">
            <v-card>
              <v-data-table
                :headers="holidayList"
                :items="sortHolidays(year, allPublicHolidays)"
                :sort-by="[{ key: 'date', order: 'asc' }]"
                density="compact"
                hover="true"
                :items-per-page="25"
                no-data-text="Keine Feiertage verfügbar"
              >
                <template #[`item.date`]="{ item }">
                  <span>{{ formatDate(item.date) }}</span>
                </template>
                <template #[`item.delete`]="{ item }">
                  <v-btn
                    class="ma-2"
                    size="small"
                    color="var(--status-error-delete)"
                    icon="mdi-delete"
                    variant="text"
                    @click="deleteHoliday(item.id)"
                  ></v-btn>
                </template>
              </v-data-table>
            </v-card>
          </div>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-card>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useVacationStore } from '@/stores/vacation-store'
import { sortHolidays } from '@/Composables/sort-public-holidays'

const vacationStore = useVacationStore()

const { allPublicHolidays, allHolidayYears } = storeToRefs(vacationStore)

const emit = defineEmits(['delete-holiday'])

const tab = ref(null)

const holidayList = ref([
  {
    title: 'Datum',
    key: 'date',
    align: 'center',
    value: (item) => `${item.date}`
  },
  { title: 'Feiertag', key: 'type', align: 'center', sortable: false },
  { title: 'Tag', key: 'day', align: 'center', sortable: false },
  { align: 'center', key: 'delete', sortable: false, value: 'delete' }
])

// Methods
const deleteHoliday = (id) => {
  emit('delete-holiday', id)
}

const formatDate = (date) => {
  const options = { day: '2-digit', month: '2-digit', year: 'numeric' }
  return new Date(date).toLocaleDateString(undefined, options)
}

onMounted(() => {
  tab.value = `tab-${new Date().getFullYear()}`
})
</script>

<style lang="scss" src="./PublicHolidays.scss" scoped />
