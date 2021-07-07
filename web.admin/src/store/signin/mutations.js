export function addToken (state, token) {
  state.token = token;
}

export function removeToken(state) {
  state.token = null;
}
