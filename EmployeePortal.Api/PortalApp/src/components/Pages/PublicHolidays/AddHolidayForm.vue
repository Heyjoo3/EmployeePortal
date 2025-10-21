<template>
  <div class="add-holidays">
    <h2>Feiertage hinzufügen</h2>
    <v-form ref="holidaysForm">
      <v-row>
        <v-col>
          <v-date-input
            color="var(--dark-text)"
            placeholder="dd.mm.yyyy"
            v-model="holiday.Date"
            label="Datum"
            variant="solo-filled"
            :min="minDate"
            :rules="rules.date"
            readonly
          ></v-date-input>
        </v-col>
        <v-col>
          <v-text-field
            v-model="holiday.Type"
            label="Bezeichnung"
            variant="solo-filled"
            :rules="rules.type"
          >
          </v-text-field>
        </v-col>
        <v-col>
          <v-btn class="add-button" @click="addHoliday" icon="mdi-plus"></v-btn>
        </v-col>
      </v-row>
    </v-form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useDateConverter } from '@/Composables/useDateConverter'

const { convertToGermanISO } = useDateConverter()

// Props and data
const holiday = ref({
  Date: null,
  Type: null
})
const minDate = ref(new Date().toISOString().substring(0, 10))
const holidaysForm = ref(null)
const rules = {
  type: [(v) => !!v || 'Wählen Sie den Feiertagstyp aus'],
  date: [(v) => !!v || 'Wählen Sie das Datum aus']
}

// Methods
const emit = defineEmits(['add-holiday'])

const addHoliday = async () => {
  const isValid = await holidaysForm.value.validate()

  if (isValid.valid) {
    const options = { weekday: 'long' }

    const date = convertToGermanISO(holiday.value.Date)
    const day = new Intl.DateTimeFormat('de-DE', options).format(new Date(holiday.value.Date))

    // Use the parent component's method or emit the event
    emit('add-holiday', {
      date: date,
      day: day,
      type: holiday.value.Type
    })

    holiday.value = { Date: null, Type: null }
  }
}
</script>

<style lang="scss" src="./PublicHolidays.scss" scoped />
