import {api} from 'boot/axios';

export function signIn (context, credentials) {
  if (context.getters.isSignedIn)
    return {status: 200, data: {token: context.state.token}}

  return api.post('/authentication/authenticate', credentials)
    .then(response => {
      let token = response.data.token;

      saveToken(context, token);

      return response;
    })
    .catch(error => {
      return error.response;
    });
}

export function refresh(context) {
  if (!context.getters.isRefreshWindow)
    return;

  api.post('/authentication/refresh')
    .then(response => {
      let token = response.data.token;

      saveToken(context, token);
    });
}

function saveToken(context, token) {
  context.commit('addToken', token);
  api.defaults.headers.common['Authorization'] = 'Bearer ' + token;
  localStorage.setItem('token', token);
}
