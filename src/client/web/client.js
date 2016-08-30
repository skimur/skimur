import React from 'react';
import ReactDOM from 'react-dom';
import { Router } from 'react-router';
import { browserHistory } from 'react-router';
import AppStore from 'stores/app';
import { Provider } from 'mobx-react';
import getRoutes from './routes';

let store = new AppStore();

//store.set(window.__STATE);

ReactDOM.render(
  <Provider store={new AppStore()}>
    <Router history={browserHistory}>
      {getRoutes()}
    </Router>
  </Provider>,
  document.getElementById('content')
);