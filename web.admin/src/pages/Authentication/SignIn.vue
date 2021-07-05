<template>
  <div id="sign-in-form-container" class="absolute-center">
    <q-form class="q-pa-lg" @submit="onSubmit">
      <q-input v-model="credentials.username" label="Username" lazy-rules="true" :rules="[]">
        <template v-slot:prepend>
          <q-icon name="person"></q-icon>
        </template>
      </q-input>
      <q-input v-model="credentials.password" label="Password" type="password" lazy-rules="true" :rules="[]">
        <template v-slot:prepend>
          <q-icon name="password"></q-icon>
        </template>
      </q-input>
      <q-btn class="full-width q-mt-lg" label="Submit" type="submit" color="primary"></q-btn>
    </q-form>
  </div>
</template>

<script>
import {reactive, toRaw} from "vue";
import {useQuasar} from "quasar";
import {useStore} from "vuex";

export default {
  name: "SignIn",
  setup() {
    const $q = useQuasar();
    const $store = useStore();
    const credentials = reactive({
      username: '',
      password: ''
    });

    return {
      credentials,

      onSubmit() {
        $store.dispatch('signIn/signIn', toRaw(credentials)).then(response => console.log(response));
      }
    }
  }
}
</script>

<style scoped>
#sign-in-form-container {
  width: 100%;
  max-width: 450px;
}
</style>
