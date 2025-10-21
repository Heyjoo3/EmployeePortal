<template>
  <div class="vacation-days">
    <h2>Gesetzliche Feiertage</h2>
    <v-card v-if="isLoaded">
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
                disable-pagination
                :headers="holidayList"
                :items="sortHolidays(year, allPublicHolidays)"
                density="compact"
                hover="true"
                no-data-text="Keine Feiertage verfügbar"
              >
                <template #bottom> </template>
              </v-data-table>
            </v-card>
          </div>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useVacationStore } from '@/stores/vacation-store'
import { useDateConverter } from '@/Composables/useDateConverter'
import { sortHolidays } from '@/Composables/sort-public-holidays'
import { storeToRefs } from 'pinia'

const { actions } = useVacationStore()
const { allPublicHolidays, allHolidayYears } = storeToRefs(useVacationStore())
const isLoaded = ref(false)

const { localizedDate } = useDateConverter()
const tab = ref(null)

const holidayList = ref([
  {
    title: 'Datum',
    key: 'date',
    align: 'center',
    value: (item) => `${localizedDate(Date.parse(item.date))}`
  },
  { title: 'Feiertage', key: 'type', align: 'center', sortable: false },
  { title: 'Tag', key: 'day', align: 'center', sortable: false }
])

onMounted(async () => {
  isLoaded.value = false
  await actions.getAllPublicHolidayYears()
  await actions.getPublicHolidays(allHolidayYears)
  isLoaded.value = true
  tab.value = `tab-${new Date().getFullYear()}`
})
</script>

<style lang="scss" src="./VacationDays.scss" scoped />
