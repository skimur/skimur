import React, { Component } from 'react';
import Helmet from 'react-helmet';

export default class Home extends Component {
  render() {
    return (
      <div>
        <Helmet title="Home" />
        <p>HOME!</p>
      </div>
    );
  }
}
