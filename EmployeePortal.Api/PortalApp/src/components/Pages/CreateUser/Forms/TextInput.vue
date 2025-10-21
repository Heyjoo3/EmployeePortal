<template>
  <v-text-field
    v-model="localValue"
    :rules="[required]"
    :label="label"
    :type="type"
    :prepend-icon="prependIcon"
    :placeholder="placeholder"
    clearable
    variant="solo-filled"
    :error-messages="errorMessages"
    @update:modelValue="updateValue"
  ></v-text-field>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  items: {
    type: Array,
    default: () => []
  },
  required: {
    type: Boolean,
    default: false
  },
  errorMessages: {
    type: Array,
    default: () => []
  },
  label: {
    type: String,
    default: ''
  },
  type: {
    type: String,
    default: 'text'
  },
  prependIcon: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: ''
  }
})
const emit = defineEmits(['update:modelValue'])

const localValue = ref(props.modelValue)

watch(
  () => props.modelValue,
  (newVal) => {
    localValue.value = newVal
  }
)

const updateValue = (newValue) => {
  emit('update:modelValue', newValue)
}
</script>

<style lang="scss" src="../CreateUser.scss" scoped />
