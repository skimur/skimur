import { observable, computed, action } from 'mobx';
import api from 'helpers/api';

class AuthStore {
    @observable user = null;

    @computed
    get isLoggedIn() {
      return this.user != null;
    }

    @action logOff() {
      this.user = null;
    }

    @action logIn(newUser) {
      this.user = newUser;
    }
}

export default AuthStore;