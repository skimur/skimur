import api from 'helpers/api';

export function logIn(store)  {
  return (userName, password) => {
    return api.logIn(userName, password)
      .then(result => {
        if(result.user) {
          // the user was logged in
          store.auth.logIn(result.user);
        }
        return result;
      });
  }
};

export function logOff(store)  {
  return () => {
    return api.logOff()
      .then(result => {
        if(result.success) {
          store.auth.logOff();
        }
        return result;
      });
  }
};

export function register(store)  {
  return (userName, email, password, passwordConfirm) => {
    return api.register(userName, email, password, passwordConfirm)
      .then(result => {
        if(result.user) {
          // we registered the user and auto logged them in
          store.auth.logIn(result.user);
        }
        return result;
      });
  }
};

export function sendCode(provider, remember) {
  return (provider, remember) => {
    return api.sendCode(provider, remember);
  };
}

export function verifyCode(store) {
  return (provider, code, rememberBrowser, rememberMe) => {
    return api.verifyCode(provider, code, rememberBrowser, rememberMe)
      .then(result => {
        if(result.user) {
          // the user verified the code and is now logged in
          store.auth.logIn(result.user);
        }
        return result;
      });
  };
}