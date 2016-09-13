import React from 'react';
import { IndexRoute, Route } from 'react-router';
import {
  App,
  Home,
  NotFound,
  Login,
  Register,
  ConfirmEmail,
  Manage,
  ManageIndex,
  ManageSecurity,
  ManageChangePassword,
  ManageLogins,
  ManageEmail
} from './containers';

export default (store) => {
  const requireLogin = (nextState, replace, cb) => {
    const { auth: { user } } = store;
    if (!user) {
      // oops, not logged in, so can't be here!
      replace('/login?returnUrl=' +
        encodeURIComponent(nextState.location.pathname + nextState.location.search));
    }
    cb();
  };
  return (
    <Route path="/" component={App}>
      { /* Home (main) route */ }
      <IndexRoute component={Home} />

      { /* Routes */ }
      <Route path="login" components={Login} />
      <Route path="register" components={Register} />
      <Route path="confirmemail" components={ConfirmEmail} />

      { /* Manage */ }
      <Route path="manage" component={Manage} onEnter={requireLogin}>
        <IndexRoute component={ManageIndex} onEnter={requireLogin} />
        <Route path="security" component={ManageSecurity} onEnter={requireLogin} />
        <Route path="email" component={ManageEmail} onEnter={requireLogin} />
        <Route path="changepassword" component={ManageChangePassword} onEnter={requireLogin} />
        <Route path="logins" component={ManageLogins} onEnter={requireLogin} />
      </Route>

      { /* Catch all route */ }
      <Route path="*" component={NotFound} status={404} />
    </Route>
  );
};
