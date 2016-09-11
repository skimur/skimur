import React, { Component } from 'react';
import { action, observable, reaction, computed } from 'mobx';
import { inject, observer } from 'mobx-react';
import { logIn, sendCode, verifyCode } from 'actions';
import Form from 'components/Form';
import Input from 'components/Input';
import ErrorList from 'components/ErrorList';
import { injectActions } from 'helpers/decorators';
import _ from 'lodash';

@observer
class LoginForm extends Form {

  @Form.observableFormField userName = '';
  @Form.observableFormField password = '';
  @Form.observableFormField rememberMe = true;

  @action
  handleLogin(result) {
    this.updateModelState(result);
    this.props.handleLogin(result);
  }

  onClick = (event) => {
    event.preventDefault();
    this.props.logIn(this.userName.value, this.password.value, this.rememberMe.value).then(result => {
      this.handleLogin(result);
    });
  }

  render() {
    return (
      <form onSubmit={this.onClick} className="form-horizontal">
        <ErrorList errors={this.modelStateErrors} /> 
        <Input field={this.userName} name="userName" label="User name" /> 
        <Input field={this.password} name="password" type="password" label="Password" />
        <Input field={this.rememberMe} name="rememberMe" type="checkbox" label="Remember me" />
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Login</button>
          </div>
        </div>
      </form>
    );
  }
}

@observer
class SendCodeForm extends Form {

  @Form.observableFormField provider = ''

  @action
  handleSendCode(result) {
    this.updateModelState(result);
    this.props.handleSendCode(result);
  }

  onClick = (event) => {
    event.preventDefault();

    // the initial state is emtpy if the value was never changed.
    let provider = this.provider.value;
    if(provider === "" && !_.isEmpty(this.props.userFactors)) {
      provider = this.props.userFactors[0];
    }

    this.props.sendCode(provider).then(result => {
      this.handleSendCode(result);
    });
  }

  render() {
    return (
      <form onSubmit={this.onClick} className="form-horizontal">
        <ErrorList errors={this.modelStateErrors} /> 
        <Input field={this.provider} 
          type="option"
          options={this.props.userFactors.map((userFactor) => ({ value: userFactor, display: userFactor }))}
          name="provider"
          label="Provider" />
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Send</button>
          </div>
        </div>
      </form>
    );
  }
}

@observer
class VerifiyCodeForm extends Form {

  @Form.observableFormField code = ''
  @Form.observableFormField rememberMe = true
  @Form.observableFormField rememberBrowser = true

  @action
  handleVerifyCode(result) {
    this.updateModelState(result);
    this.props.handleVerifyCode(result);
  }

  onClick = (event) => {
    event.preventDefault();
    this.props.verifyCode(this.props.provider, this.code.value, this.rememberMe.value, this.rememberBrowser.value).then(result => {
      this.handleVerifyCode(result);
    });
  }

  render() {
    return (
      <form onSubmit={this.onClick} className="form-horizontal">
        <ErrorList errors={this.modelStateErrors} /> 
        <Input field={this.code}
          name="code"
          label="Code" /> 
        <Input field={this.rememberMe}
          type='checkbox'
          name="rememberMe"
          label="Remember me" />
        <Input field={this.rememberBrowser}
          type='checkbox'
          name="rememberBrowser"
          label="Remember browser" /> 
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Verify</button>
          </div>
        </div>
      </form>
    );
  }
}

@inject("store")
@injectActions({ logIn, sendCode, verifyCode }, "store")
@observer
export default class LoginFormContainer extends Component {

  @observable requiresTwoFactor = false;
  @observable userFactors = [];
  @observable sentCode = false;
  @observable codeSentWithProvider;
  @observable codeVerified = false;

  @action
  handleLogin = (result) => {
    if(result.success) {
      this.props.onLoggedIn();
    } else {
      // maybe we failed to login because of two-factor?
      if(result.requiresTwoFactor) {
        this.requiresTwoFactor = true;
        this.userFactors = result.userFactors;
      }
    }
  }

  @action
  handleSendCode = (result) => {
    if(result.success) {
      this.sentCode = true;
      this.codeSentWithProvider = result.provider;
    }
  }

  @action
  handleVerifyCode = (result) => {
    if(result.success) {
      this.codeVerified = true;
      this.props.onLoggedIn();
    }
  }

  render() {
    if(this.codeVerified)
      return <div>You have been logged in!</div>

    if(this.sentCode)
      return <VerifiyCodeForm verifyCode={this.props.verifyCode} handleVerifyCode={this.handleVerifyCode} provider={this.codeSentWithProvider} />

    if(this.requiresTwoFactor)
      return <SendCodeForm sendCode={this.props.sendCode} handleSendCode={this.handleSendCode} userFactors={this.userFactors}  />
    
    return <LoginForm logIn={this.props.logIn} handleLogin={this.handleLogin} />
  }
}