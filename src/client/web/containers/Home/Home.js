import React, { Component } from 'react';
import { action } from 'mobx';
import { inject, observer } from 'mobx-react';
import Helmet from 'react-helmet';

export default class Home extends Component {
  render() {
    return (
      <div>
        <Helmet title="Home" />
      </div>
    );
  }
}
