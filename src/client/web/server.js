import React from 'react';
import ReactDOM from 'react-dom/server';
import Html from './helpers/Html';
import { match } from 'react-router';
import getRoutes from './routes';
import createHistory from 'react-router/lib/createMemoryHistory';
import RouterContext from 'react-router/lib/RouterContext';
import { toJS } from 'mobx';
import { Provider } from 'mobx-react';
import Store from 'store';

export function renderView(callback, path, model, viewBag) {
  const history = createHistory(path);
  const result = {
    html: null,
    status: 404,
    redirect: null
  };
  match(
    { history, routes: getRoutes(), location: path },
    (error, redirectLocation, renderProps) => {
      if (redirectLocation) {
        result.redirect = redirectLocation.pathname + redirectLocation.search;
      } else if (error) {
        result.status = 500;
      } else if (renderProps) {
        result.status = 200;
        renderProps.routes.forEach(function(route) {
          if(route.status)
            result.status = route.status;
        });
        const store = new Store();
        store.initialize(model);
        const component =
        (
          <Provider store={store}>
            <RouterContext {...renderProps} />
          </Provider>
        );
        result.html = ReactDOM.renderToString(<Html component={component} state={toJS(store)} />);
      } else {
        result.status = 404;
      }
    });
  callback(null, result);
}

export function renderPartialView(callback) {
  callback('TODO', null);
}
