import { observable, action, runInAction, autorun } from 'mobx';
import { nonenumerable } from 'helpers/decorators';

class NavStore {

  @nonenumerable
  isHistoryChanged = false;

  @nonenumerable
  history = null;

  constructor(history) {
    this.history = history;
    // callback gets called immediately one time
    this.history.listen(location => {
      this.isHistoryChanged = true;
      runInAction(() => {
        this.path = location.pathname;
      });
      this.isHistoryChanged = false;
    });
    autorun(() => {
      var newPath = this.path;

      // if this event is raised due to the history object changing, no need to sync the history object.
      if(this.isHistoryChanged) return;
      this.history.push(newPath);
    });
  }

  @observable path = ''

  @action navigateTo(path) {
    this.path = path;
  }
}

export default NavStore;