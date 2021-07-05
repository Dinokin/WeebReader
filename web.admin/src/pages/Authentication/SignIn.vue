<template>
  <div id="sign-in-form-container" class="absolute-center">
    <q-form class="q-pa-lg" @submit="onSubmit">
      <q-input v-model="credentials.username" label="Username" lazy-rules :rules="[
        value => value?.length > 0 || 'An username is required.',
        value => value.length >= 3 || 'An username must have at least 3 characters.'
      ]">
        <template v-slot:prepend>
          <q-icon name="person"></q-icon>
        </template>
      </q-input>
      <q-input v-model="credentials.password" label="Password" type="password" lazy-rules :rules="[
        value => value?.length > 0 || 'A password is required.',
        value => value.length >= 3 || 'An username must have at least 3 characters.'
      ]">
        <template v-slot:prepend>
          <q-icon name="password"></q-icon>
        </template>
      </q-input>
      <q-btn class="full-width q-mt-lg" label="Submit" type="submit" color="primary"></q-btn>
    </q-form>
  </div>
</template>

<script>
import {reactive, toRaw} from 'vue';
import {useStore} from 'vuex';
import {useRouter} from 'vue-router';
import {useQuasar} from 'quasar';

export default {
  name: "SignIn",
  setup() {
    const store = useStore();
    const router = useRouter();
    const quasar = useQuasar();

    const credentials = reactive({
      username: '',
      password: ''
    });

    return {
      credentials,

      onSubmit() {
        quasar.loading.show();

        store.dispatch('signIn/signIn', toRaw(credentials)).then(response => {
          if (response.status === 200) {
            router.push({path: '/'});
          }
          else {
            for(let index in response.data.messages)
              quasar.notify({
                type: 'negative',
                message: response.data.messages[index],
                timeout: 1000,
                position: 'top'
              });
          }

          quasar.loading.hide();
        });
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
