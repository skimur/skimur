import React, { Component } from 'react';
import { Input, ErrorList } from 'components';
import Form from 'components/Form';
import ExternalLoginButton from 'components/ExternalLoginButton';
import { action, observable, reaction, computed, runInAction } from 'mobx';
import { inject, observer } from 'mobx-react';
import { register, navigateTo, externalAuthentication } from 'actions';
import { injectActions } from 'helpers/decorators';
import { observableForm, observableFormField } from 'utils/state/form';
import { Button, Row, Col } from 'react-bootstrap';

@observableForm
class RegisterFormState {

  constructor(actions, store) {
    this.actions = actions;
    this.store = store;
  }

  @observableFormField userName = '';
  @observableFormField email = '';
  @observableFormField password = '';
  @observableFormField passwordConfirm = '';
  @observable externalLogin = null;

  @action
  register() {
    let externalLoginProvider;
    if(this.externalLogin) {
      externalLoginProvider = this.externalLogin.scheme;
    }
    return this.actions.register(this.userName.value, this.email.value, this.password.value, this.passwordConfirm.value, externalLoginProvider)
      .then(result => {
        this.handleRegister(result);
        return result;
      });
  }

  @action
  handleRegister(result) {
    this.updateModelState(result);
    if(result.user) {
      this.actions.navigateTo('/');
    }
  }

  @action
  removeExternalLogin() {
    this.externalLogin = null;
  }

  @action
  addExternalLogin(scheme) {
    return this.actions.externalAuthentication(scheme, false /*don't login*/)
      .then(result => {
        if(result.cancelled)
          return result;
        this.handleExternalLogin(result);
        return result;
      })
      .catch(result => {
        
      });
  }

  @action
  handleExternalLogin(result) {
    this.updateModelState(result);
    if(result.externalAuthenticated) {
      this.externalLogin = result.loginProvider;
      this.userName.value = result.proposedUserName;
      this.email.value = result.proposedEmail;
    }
  }
}

@inject("store")
@injectActions({ register, navigateTo, externalAuthentication }, "store")
@observer
export default class RegisterForm extends Component {

  constructor(props) {
    super(props);
    this.state = new RegisterFormState(props.actions, props.store);
  }

  componentWillMount() {
    // when this page is loaded, load any query string paramaters
    // that may have been passed from the login page
    let query = this.props.store.nav.query;
    if(query.provider) {
      // let's see if it is a valid provider
      let found;
      this.props.store.externalLogins.loginProviders.forEach(function(loginProvider) { 
        if(!found) {
          if(loginProvider.scheme.toUpperCase() == query.provider.toUpperCase()) {
            found = loginProvider;
          }
        }
      });
      if(found) {
        runInAction(() => {
          this.state.externalLogin = found;
          this.state.userName.value = query.proposedUserName;
          this.state.email.value = query.proposedEmail;
        });
      }
    }
  }

  onClick = (event) => {
    event.preventDefault();
    this.state.register();
  }

  onRemoveExternalAuthClick = (event) => {
    event.preventDefault();
    this.state.removeExternalLogin();
  }

  onExternalLoginClick = (scheme) => {
    return (event) => {
      event.preventDefault();
      this.state.addExternalLogin(scheme);
    }
  }

  render() {
    const {
      loginProviders
    } = this.props.store.externalLogins;
    return (
      <form onSubmit={this.onClick} className="form-horizontal">
        <ErrorList errors={this.state.modelStateErrors} />
        {this.state.externalLogin &&
          <Row className="form-group">
            <Col md={2} />
            <Col md={10}>

              <ExternalLoginButton
                scheme={this.state.externalLogin.scheme}
                text={'Registering with ' + this.state.externalLogin.displayName}
                />
              {' '}
              <Button onClick={this.onRemoveExternalAuthClick}>
                Cancel
              </Button>
            </Col>
          </Row>
        }
         {(!this.state.externalLogin && loginProviders.length > 0) &&
          <Row className="form-group">
            <Col md={2} />
            <Col md={10}>
              {loginProviders.map((loginProvider, i) =>
              (
                <span key={i}>
                  <ExternalLoginButton
                    scheme={loginProvider.scheme}
                    text={loginProvider.displayName}
                    onClick={this.onExternalLoginClick(loginProvider.scheme)} />
                  {' '}
                </span>
              ))}
            </Col>
          </Row>
        }
        <Input field={this.state.userName} name="userName" label="User name" />
        <Input field={this.state.email} name="email" label="Email" />
        <Input field={this.state.password} name="password" type="password" label="Password" />
        <Input field={this.state.passwordConfirm} name="passwordConfirm" type="password" label="Confirm" />
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Register</button>
          </div>
        </div>
      </form>
    );
  }
}