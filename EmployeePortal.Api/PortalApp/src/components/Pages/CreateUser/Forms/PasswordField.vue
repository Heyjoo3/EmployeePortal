<template>
  <v-row class="passwordRow">
    <v-text-field
      v-model="localValue"
      :error-messages="errorMessages"
      prepend-icon="mdi-key"
      :rules="rules.passwordRule"
      label="Kennwort"
      clearable
      :type="inputType"
      variant="solo-filled"
      @update:modelValue="updateValue"
    >
      <template #append-inner>
        <v-icon @click="toggleVisibility">{{ iconToggle }}</v-icon>
      </template>
      <template #append>
        <v-icon @click="copyPasswordToClipboard">mdi-content-copy</v-icon>
      </template>
    </v-text-field>
    <v-btn @click="generatePassword" class="password-button">Generieren</v-btn>
  </v-row>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps(['modelValue', 'rules', 'errorMessages'])
const emit = defineEmits(['update:modelValue', 'toggleAlert'])

const localValue = ref(props.modelValue)
const hidePassword = ref(false)

watch(
  () => props.modelValue,
  (newVal) => {
    localValue.value = newVal
  }
)

const inputType = computed(() => {
  return hidePassword.value == false ? 'password' : 'text'
})

const iconToggle = computed(() => {
  return hidePassword.value ? 'mdi-eye' : 'mdi-eye-off'
})

const toggleVisibility = () => {
  hidePassword.value = !hidePassword.value
}

const copyPasswordToClipboard = () => {
  navigator.clipboard
    .writeText(localValue.value)
    .then(() => {
      emit('toggleAlert', true)
    })
    .catch((error) => {
      console.error('Beim Kopieren in die Zwischenablage ist etwas schief gelaufen:', error)
    })
}

const generatePassword = () => {
  const length = 10
  const uppercaseChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
  const lowercaseChars = 'abcdefghijklmnopqrstuvwxyz'
  const numbers = '0123456789'
  const specialChars =
    '! " # $ % & \' ( ) * + , - . / : ; < = > ? @ [ \\ ] ^ _ ` { | } ~ ¡ ¢ £ ¥ § ¤ © ® ™ ¶ × ÷ ¿ ° ± µ ¶ ª º ¼ ½ ¾ † ‡ • … ‰ ‹ › ‾ ‟ ‘ ’ “ ” „ ‹ › « » ⸮ ‽ ⁂ ⁇ ⁈ ⁉ ⅛ ⅜ ⅝ ⅞ ↔ ↕ ↖ ↗ ↘ ↙ ↩ ↪ ⇄ ⇅ ⇆ ⇇ ⇈ ⇉ ⇊ ⇋ ⇌ ⇏ ⇐ ⇑ ⇒ ⇓ ⇔ ∀ ∂ ∃ ∅ ∆ ∇ ∈ ∉ ∋ ∏ ∑ − ∗ √ ∝ ∞ ∟ ∠ ∩ ∪ ∫ ∴ ∼ ≈ ≠ ≡ ≤ ≥ ⊂ ⊃ ⊄ ⊆ ⊇ ⊕ ⊗'
  const allChars = uppercaseChars + lowercaseChars + numbers + specialChars
  let password = ''

  password += uppercaseChars[Math.floor(Math.random() * uppercaseChars.length)]
  password += lowercaseChars[Math.floor(Math.random() * lowercaseChars.length)]
  password += numbers[Math.floor(Math.random() * numbers.length)]
  password += specialChars[Math.floor(Math.random() * specialChars.length)]

  for (let i = 3; i < length; i++) {
    const randomIndex = Math.floor(Math.random() * allChars.length)
    password += allChars[randomIndex]
  }

  password = password
    .split('')
    .sort(() => Math.random() - 0.5)
    .join('')

  localValue.value = password
  emit('update:modelValue', password)
}

const updateValue = (newValue) => {
  emit('update:modelValue', newValue)
}
</script>

<style scoped>
.passwordRow {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: auto;
  gap: 10px;
}

.password-button {
  margin-top: -20px;
}
</style>
