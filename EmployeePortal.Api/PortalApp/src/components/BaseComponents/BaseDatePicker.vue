<template>
  <v-menu
    open-on-click="true"
    :close-on-content-click="false"
    transition="scale-transition"
    min-width="auto"
    offset-y
  >
    <template v-slot:activator="{ props }">
      <v-text-field
        v-model="formattedDate"
        :label="label"
        :variant="variant"
        readonly
        v-bind="props"
        :density="density"
    ></v-text-field>
    </template>
    <v-date-picker
      v-model="date"
      :max="maxDate"
      :min="minDate"
      hide-header
      show-adjacent-months
      @update:modelValue="updateDate"
    ></v-date-picker>
  </v-menu>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useDateConverter } from '@/Composables/useDateConverter.js'

const { localizedDate } = useDateConverter()

const props = defineProps({
  value: Date,
  label: String,
  variant: { type: String, default: 'outlined' },
  minDate: { type: Date, default: null },
  maxDate: { type: Date, default: null },
  density: { type: String, default: 'comfortable' }
})

const emit = defineEmits(['input'])

const date = ref(null)

const formattedDate = computed(() => localizedDate(date.value))

const updateDate = (newDate) => {
  emit('input', newDate)
}

onMounted(() => {

    if (typeof props.value === 'string') {
      date.value = new Date(props.value)
    } else {
      date.value = props.value
    }
})
</script>
