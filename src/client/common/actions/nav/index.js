export function navigateTo(store)  {
  return (path) => {
    store.nav.navigateTo(path);
  }
};
