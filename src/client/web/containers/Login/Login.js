import React, { Component } from 'react';
import { LoginForm } from 'components';
import { Link } from 'react-router';
import { inject, observer } from 'mobx-react';
import { navigateTo } from 'actions';
import { injectActions } from 'helpers/decorators';

@inject('store')
@injectActions({ navigateTo }, 'store')
@observer
export default class Login extends Component {
  onLoggedIn = () => {
    if(this.props.store.nav.query.returnUrl) {
      this.props.navigateTo(this.props.store.nav.query.returnUrl);
    } else {
      this.props.navigateTo('/');
    }
  }
  render() {
    return (
      <div>
        <h2>Login</h2>
        <hr />
        <LoginForm onLoggedIn={this.onLoggedIn} />
        <p>
          <Link to="/register">Register as a new user?</Link>
        </p>
        <p>
          <Link to="/forgotpassword">Forgot your password?</Link>
        </p>
      </div>
    );
  }
}