import { store } from 'src/utils/state';
import { useStrict } from 'mobx';

import AppStore from './stores/app';

/**
  Enables / disables strict mode globally.
  In strict mode, it is not allowed to
  change any state outside of an action
 */
useStrict(true);

/**
  Stores
*/
export default store
  .setup({
    app: AppStore
  });
