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
    })
    .catch(error => {
      if (error.response.code !== 401)
        return;

      removeToken(context);
    });
}

function saveToken(context, token) {
  context.commit('addToken', token);
  api.defaults.headers.common['Authorization'] = 'Bearer ' + token;
  localStorage.setItem('token', token);
}

function removeToken(context) {
  context.commit('removeToken');
  delete api.defaults.headers.common['Authorization'];
  localStorage.removeItem('token');
}
