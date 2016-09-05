import React from 'react';
import ReactDOM from 'react-dom';
import { Router } from 'react-router';
import { browserHistory } from 'react-router';
import Store from 'store';
import { Provider } from 'mobx-react';
import getRoutes from './routes';

let store = new Store();

store.initialize(window.__data);

ReactDOM.render(
  <Provider store={store}>
    <Router history={browserHistory}>
      {getRoutes()}
    </Router>
  </Provider>,
  document.getElementById('content')
);