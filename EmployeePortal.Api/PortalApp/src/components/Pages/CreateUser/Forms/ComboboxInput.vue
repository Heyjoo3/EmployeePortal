<template>
  <v-combobox
    v-model="localValue"
    :label="label"
    :items="items"
    variant="solo-filled"
    :rules="[required]"
    :prependIcon="prependIcon"
    @update:modelValue="updateValue"
  ></v-combobox>
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
  label: {
    type: String,
    default: ''
  },
  prependIcon: {
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
