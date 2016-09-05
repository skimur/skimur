import React, { Component } from 'react';
import { action } from 'mobx';
import { inject, observer } from 'mobx-react';
import Helmet from 'react-helmet';

@inject("store") @observer
export default class Home extends Component {
  render() {
    const {
      user
    } = this.props.store.auth;
    return (
      <div>
        <Helmet title="Home" />
        <p>{user && user.userName}</p>
      </div>
    );
  }
}
