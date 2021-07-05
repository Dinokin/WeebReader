import jwtDecode from "jwt-decode";

export function isSignedIn(state) {
  let token = state.token;

  if (!token)
    return false;

  let remainingTime = getRemainingTime(token);

  return remainingTime > 0;
}

export function isRefreshWindow(state) {
  let token = state.token;

  if (!token)
    return false;

  let remainingTime = getRemainingTime(token);

  return remainingTime > 0 && remainingTime < 5;
}

function getRemainingTime(token) {
  let tokenDate = new Date(getPayload(token).exp * 1000);
  let currentDate = new Date(Date.now());

  return new Date(tokenDate - currentDate).getMinutes();
}

function getPayload(token) {
  try {
    return jwtDecode(token)
  }
  catch (e) {
    return null;
  }
}
