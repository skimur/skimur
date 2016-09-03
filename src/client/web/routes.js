import React from 'react';
import { IndexRoute, Route } from 'react-router';
import {
  App,
  Home,
  NotFound,
  Login,
  Register
} from './containers';

export default () => {
  return (
    <Route path="/" component={App}>
      { /* Home (main) route */ }
      <IndexRoute component={Home} />

      { /* Routes */ }
      <Route path="login" components={Login} />
      <Route path="register" components={Register} />

      { /* Catch all route */ }
      <Route path="*" component={NotFound} status={404} />
    </Route>
  );
};
