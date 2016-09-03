import React, { Component } from 'react';
import { RegisterForm } from 'components';

export default class Register extends Component {
  render() {
    return (
      <div>
        <h2>Register</h2>
        <h4>Create a new account.</h4>
        <hr />
        <RegisterForm />
      </div>
    );
  }
}