import { observable, computed, action, runInAction, autorun, asMap } from 'mobx';
import { nonenumerable } from 'helpers/decorators';
import { parsePath  } from 'history/modules/PathUtils';
import { parse as parseQueryString } from 'query-string'

class NavStore {

  // private properties
  @nonenumerable
  isHistoryChanged = false;
  @nonenumerable
  history = null;
  @nonenumerable
  ranPathChangedOnce = false;

  // public properties
  @observable path = ''
  @observable hash = ''
  @observable search = ''
  @computed get query() {
    return parseQueryString(this.search);
  }
  @computed get fullPath() {
    return this.path + this.hash + this.search;
  }

  constructor(history) {
    this.history = history;
    // callback gets called immediately one time
    this.history.listen(location => {
      this.isHistoryChanged = true;
      runInAction(() => {
        this.path = location.pathname;
        this.hash = location.hash;
        this.search = location.search;
      });
      this.isHistoryChanged = false;
    });
    autorun(this.onPathChanged);
  }

  @action navigateTo(path) {
    var newPath = parsePath(path);
    this.path = newPath.pathname;
    this.hash = newPath.hash;
    this.search = newPath.search;
  }

  @nonenumerable
  onPathChanged = () => {
    // this is so that we are registered to fire again if these values change
    var path = this.path;
    var hash = this.hash;
    var search = this.search;
    if(!this.ranPathChangedOnce) {
      this.ranPathChangedOnce = true;
      return;
    }
    // if this event is raised due to the history object changing, no need to sync the history object.
    if(this.isHistoryChanged) return;
    this.history.push(this.path + this.hash + this.search);
  }
}

export default NavStore;