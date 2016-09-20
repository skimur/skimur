import { useStrict, action, runInAction, asMap, autorun, observable } from 'mobx';
import { nonenumerable } from 'helpers/decorators';
import deepAssign from 'deep-assign';
import NavStore from './stores/nav';
import AppStore from './stores/app';
import AuthStore from './stores/auth';
import ExternalLogins from './stores/externalLogins';

useStrict(true);

class Store {

  @observable viewBag = {};
  @nonenumerable pathListenerDispose = null;

  constructor(history) {
    this.nav = new NavStore(history);
    this.app = new AppStore();
    this.auth = new AuthStore();
    this.externalLogins = new ExternalLogins();
    // this helps
    this.pathListenerDispose = autorun(() => {
      var path = this.nav.fullPath;
      if(!this.pathListenerDispose) {
        // ignore first run
        return;
      }
      // don't fire this again, called once after navigation
      this.pathListenerDispose();
      this.pathListenerDispose = null;
      runInAction(() => {
        this.viewBag = {};
      });
    });
  }

  @action
  initialize(state = {}) {
    Object.keys(state).forEach(key => {
      if(key == 'viewBag') {
        this.viewBag = state[key];
      } else {
        if(this[key]) {
          deepAssign(this[key], state[key]);
        }
      }
    });
  }
}

export default Store;
