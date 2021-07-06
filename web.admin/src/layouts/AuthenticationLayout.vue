<template>
  <q-layout view="hHh lpR fFf">
    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script>
export default {
  name: 'AuthenticationLayout',
  preFetch({store, redirect}) {
    if (store.getters["signIn/isSignedIn"])
      redirect({path: '/'});
  }
}
</script>

<style scoped>
</style>
