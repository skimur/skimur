import { runInAction, action } from 'mobx';
import api from 'helpers/api';

export function logIn(store)  {
  return (userName, password) => {
    return api.logIn(userName, password)
      .then(result => {
        if(result.success) {
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

