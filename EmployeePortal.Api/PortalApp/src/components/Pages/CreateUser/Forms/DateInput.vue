<template>
  <v-date-input
    color="blue"
    placeholder="DD.MM.YYYY"
    readonly
    v-model="localValue"
    label="Geburtstag"
    variant="solo-filled"
    :min="minDate"
    :rules="[required]"
    @update:modelValue="updateValue"
  ></v-date-input>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps(['modelValue', 'minDate', 'required'])
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
