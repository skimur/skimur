import React, { Component } from 'react';
import { LoginForm } from 'components';

export default class Login extends Component {
  render() {
    return (
      <div>
        <h2>Login</h2>
        <LoginForm />
      </div>
    );
  }
}