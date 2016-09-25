import api from 'helpers/api';
import { authentication as authenticationPopup } from 'helpers/oauth';

export function logIn(store)  {
  return (userName, password, rememberMe) => {
    return api.logIn(userName, password, rememberMe)
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
  return (userName, email, password, passwordConfirm, externalLogin) => {
    return api.register(userName, email, password, passwordConfirm, externalLogin)
      .then(result => {
        if(result.user) {
          // we registered the user and auto logged them in
          store.auth.logIn(result.user);
        }
        return result;
      });
  }
};

export function sendCode(store) {
  return (provider) => {
    return api.sendCode(provider);
  };
}

export function verifyCode(store) {
  return (provider, code, rememberMe, rememberBrowser) => {
    return api.verifyCode(provider, code, rememberMe, rememberBrowser)
      .then(result => {
        if(result.user) {
          // the user verified the code and is now logged in
          store.auth.logIn(result.user);
        }
        return result;
      });
  };
}

export function externalAuthentication(store) {
  return (provider, autoLogin = true) => {
    return authenticationPopup(provider, autoLogin)
      .then(result => {
        if(result.user) {
          // the user was signed in with this request
          store.auth.logIn(result.user);
        }
        return result;
      });
  };
}