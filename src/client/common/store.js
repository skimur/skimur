import { useStrict, action } from 'mobx';
import deepAssign from 'deep-assign';
import NavStore from './stores/nav';
import AppStore from './stores/app';
import AuthStore from './stores/auth';

useStrict(true);

class Store {
  constructor(history) {
    this.nav = new NavStore(history);
    this.app = new AppStore();
    this.auth = new AuthStore();
  }

  @action
  initialize(state = {}) {
    deepAssign(this, state);
  }
}

export default Store;
