import React, { Component } from 'react';
import { observable, action } from 'mobx';
import { inject, observer } from 'mobx-react';
import { getExternalLogins } from 'actions';
import { injectActions } from 'helpers/decorators';
import { ExternalLoginButton } from 'components';

@inject('store')
@injectActions({ getExternalLogins }, 'store')
@observer
export default class Logins extends Component {

  @observable loginsLoaded = false;

  @observable currentLogins = [];
  @observable otherLogins = [];

  componentDidMount() {
    if(!this.loginsLoaded) {
      this.props.getExternalLogins()
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
                  <ExternalLoginButton
                    key={i}
                    scheme={currentLogin.loginProvider}
                    text={currentLogin.loginProviderDisplayName} />
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
                  <ExternalLoginButton
                    key={i}
                    scheme={otherLogin.scheme}
                    text={otherLogin.displayName} />
                ))}
              </tbody>
            </table>
          </div>
        }
      </div>
    );
  }
}
