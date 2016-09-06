import { runInAction, action } from 'mobx';
import api from 'helpers/api';

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

