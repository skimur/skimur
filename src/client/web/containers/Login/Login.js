import React, { Component } from 'react';
import { LoginForm } from 'components';
import { Link } from 'react-router';

export default class Login extends Component {
  render() {
    return (
      <div>
        <h2>Login</h2>
        <hr />
        <LoginForm />
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