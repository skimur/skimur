import { useStrict, action } from 'mobx';
import deepAssign from 'deep-assign';
import AppStore from './stores/app';
import AuthStore from './stores/auth';

useStrict(true);

class Store {
  constructor() {
    this.app = new AppStore();
    this.auth = new AuthStore();
  }

  @action
  initialize(state = {}) {
    deepAssign(this, state);
  }
}

export default Store;
