import { runInAction, action } from 'mobx';

export function navigateTo(store)  {
  return (path) => {
    store.nav.navigateTo(path);
  }
};
