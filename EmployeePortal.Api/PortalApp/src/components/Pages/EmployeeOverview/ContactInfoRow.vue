<template>
  <v-col>
    <div class="property-label mt-4">{{ props.label }}</div>
    <div class="property-value">{{ props.value }}</div>
  </v-col>
</template>

<script setup>
const props = defineProps({
  label: {
    type: String,
    required: true
  },
  value: {
    required: true
  }
})
</script>
