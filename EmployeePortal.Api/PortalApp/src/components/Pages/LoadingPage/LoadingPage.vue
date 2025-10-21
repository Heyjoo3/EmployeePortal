<template>
  <v-layout>
    <v-app-bar color="var(--primary-blue)" prominent>
      <base-header :logIn="true" />
    </v-app-bar>
    <v-main class="body">
      <div>
        <v-progress-circular
          color="var(--primary-blue)"
          indeterminate
          :size="128"
          :width="10"
        ></v-progress-circular>
      </div>
      <br />
      <h2>Einen Moment bitte...</h2>
    </v-main>
  </v-layout>
</template>

<script setup>
import BaseHeader from '@/components/Layout/Header/BaseHeader.vue'
</script>

<style lang="scss" src="./LoadingPage.scss" scoped></style>
