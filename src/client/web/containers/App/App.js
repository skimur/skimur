import React, { Component, PropTypes } from 'react';
import Helmet from 'react-helmet';
import DevTools from 'mobx-react-devtools';

export default class App extends Component {
  static propTypes = {
    children: PropTypes.object.isRequired
  };
  render() {
    return (
      <div>
        {this.props.children}
        <DevTools />
      </div>
    );
  }
}
