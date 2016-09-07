import React, { Component } from 'react';
import { action, observable, reaction, computed } from 'mobx';
import { inject, observer } from 'mobx-react';
import { logIn, navigateTo } from 'actions';
import Form from 'components/Form';
import Input from 'components/Input';
import ErrorList from 'components/ErrorList';

@inject("store") @observer
export default class LoginForm extends Form {

  constructor(props) {
    super(props);
    this.logIn = logIn(props.store);
    this.navigateTo = navigateTo(props.store);
  }

  @Form.observableFormField userName = '';
  @Form.observableFormField password = '';

  onClick = (event) => {
    event.preventDefault();
    this.logIn(this.userName.value, this.password.value).then(result => {
      this.updateModelState(result);
      if(result.success) {
        this.navigateTo('/');
      }
    });
  }

  render() {
    return (
      <form onSubmit={this.onClick} className="form-horizontal">
        <ErrorList errors={this.modelStateErrors} /> 
        <Input field={this.userName} name="userName" label="User name" /> 
        <Input field={this.password} name="password" label="Password" /> 
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Login</button>
          </div>
        </div>
      </form>
    );
  }
}