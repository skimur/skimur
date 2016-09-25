import React, { Component } from 'react';
import { observable, action } from 'mobx';
import { inject, observer } from 'mobx-react';
import { getExternalLogins, externalAuthentication, addExternalLogin, removeExternalLogin } from 'actions';
import { injectActions } from 'helpers/decorators';
import { ExternalLoginButton } from 'components';
import { Button } from 'react-bootstrap';

@inject('store')
@injectActions({ getExternalLogins, externalAuthentication, addExternalLogin, removeExternalLogin }, 'store')
@observer
export default class Logins extends Component {

  @observable loginsLoaded = false;

  @observable currentLogins = [];
  @observable otherLogins = [];

  componentDidMount() {
    if(!this.loginsLoaded) {
      this.props.actions.getExternalLogins()
        .then(result => {
          this.handleLogins(result);
        });
    }
  }

  @action
  handleLogins(result) {
    this.loginsLoaded = true;
    this.currentLogins = result.externalLogins.currentLogins;
    this.otherLogins = result.externalLogins.otherLogins;
  }

  addButtonClick(scheme) {
    return (event) => {
      event.preventDefault();
      this.props.actions.externalAuthentication(scheme, false /*don't log in, just externally authenticate*/)
        .then(result => {
          if(result.cancelled)
            return result;
          this.handleExternalAuthentication(result);
          return result;
        });
    };
  }
  removeButtonClick(loginProvider, providerKey) {
    return (event) => {
      event.preventDefault();
      this.props.actions.removeExternalLogin(loginProvider, providerKey)
        .then(result => this.handleRemoveExternalLogin(result));
    };
  }

  @action
  handleExternalAuthentication(result) {
    if(result.externalAuthenticated) {
      // the user succesfully authenticated with the service.
      this.props.actions.addExternalLogin()
        .then(r => this.handleAddExternalLogin(r));
    }
  }

  @action
  handleAddExternalLogin(result) {
    if(!result.success) {
      // TODO: let the user now
    }
    this.currentLogins = result.externalLogins.currentLogins;
    this.otherLogins = result.externalLogins.otherLogins;
  }

  @action
  handleRemoveExternalLogin(result) {
    if(!result.success) {
      // TODO: let the user now
    }
    this.currentLogins = result.externalLogins.currentLogins;
    this.otherLogins = result.externalLogins.otherLogins;
  }

  render() {
    if(!this.loginsLoaded)
      return <div>Loading</div>;
    return (
      <div>
        <h2>Manage your external logins</h2>
        {(this.currentLogins.length > 0) &&
          <div>
            <h4>Current logins</h4>
            <table className="table">
              <tbody>
                {this.currentLogins.map((currentLogin, i) =>
                (
                  <tr key={i}>
                    <td>
                      <Button onClick={this.removeButtonClick(currentLogin.loginProvider, currentLogin.providerKey)}>
                        Remove
                      </Button>
                      {' '}
                      <ExternalLoginButton
                        scheme={currentLogin.loginProvider}
                        text={currentLogin.loginProviderDisplayName} />
                    </td>
                  </tr>
                  
                ))}
              </tbody>
            </table>
          </div>
        }
        {(this.otherLogins.length > 0) &&
          <div>
            <h4>Add another service to log in.</h4>
            <table className="table">
              <tbody>
                {this.otherLogins.map((otherLogin, i) =>
                (
                  <tr key={i}>
                    <td>
                      <Button onClick={this.addButtonClick(otherLogin.scheme)}>
                        Add
                      </Button>
                      {' '}
                      <ExternalLoginButton
                        scheme={otherLogin.scheme}
                        text={otherLogin.displayName} />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        }
      </div>
    );
  }
}
