import React, { Component, PropTypes } from 'react';
import Helmet from 'react-helmet';

export class App extends Component {
  static propTypes = {
    children: PropTypes.object.isRequired
  };
  render() {
    return (
      <div>
        {this.props.children}
      </div>
    );
  }
}
