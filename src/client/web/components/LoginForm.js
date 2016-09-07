import React, { Component } from 'react';
import { action, observable, reaction, computed } from 'mobx';
import { inject, observer } from 'mobx-react';
import { logIn, navigateTo } from 'actions';
import Form from 'components/Form';
import Input from 'components/Input';
import ErrorList from 'components/ErrorList';
import { injectActions } from 'helpers/decorators';

console.log(injectActions);

@inject("store")
@injectActions({ logIn, navigateTo }, "store")
@observer
export default class LoginForm extends Form {


  @Form.observableFormField userName = '';
  @Form.observableFormField password = '';

  onClick = (event) => {
    event.preventDefault();
    this.props.logIn(this.userName.value, this.password.value).then(result => {
      this.updateModelState(result);
      if(result.success) {
        this.props.navigateTo('/');
      }
    });
  }

  render() {
    return (
      <form onSubmit={this.onClick} className="form-horizontal">
        <ErrorList errors={this.modelStateErrors} /> 
        <Input field={this.userName} name="userName" label="User name" /> 
        <Input field={this.password} name="password" type="password" label="Password" /> 
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Login</button>
          </div>
        </div>
      </form>
    );
  }
}