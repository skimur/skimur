import React, { Component } from 'react';
import { Input, ErrorList } from 'components';
import Form from 'components/Form';
import { action, observable, reaction, computed } from 'mobx';
import { inject, observer } from 'mobx-react';
import { register, navigateTo } from 'actions';

@inject("store") @observer
export default class RegisterForm extends Form {

  constructor(props) {
    super(props);
    this.register = register(props.store);
    this.navigateTo = navigateTo(props.store);
  }

  @Form.observableFormField userName = '';
  @Form.observableFormField email = '';
  @Form.observableFormField password = '';
  @Form.observableFormField passwordConfirm = '';

  onClick = (event) => {
    event.preventDefault();
    this.register(this.userName.value, this.email.value, this.password.value, this.passwordConfirm.value).then(result => {
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
        <Input field={this.email} name="userName" label="Email" />
        <Input field={this.password} name="userName" label="Password" />
        <Input field={this.passwordConfirm} name="userName" label="Confirm" />
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Register</button>
          </div>
        </div>
      </form>
    );
  }
}