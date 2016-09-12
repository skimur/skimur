import React, { Component } from 'react';
import { observable, action } from 'mobx';
import { inject, observer } from 'mobx-react';
import { injectActions } from 'helpers/decorators';
import { getSecurity, setTwoFactor } from 'actions';
import { Alert, Button } from 'react-bootstrap';

@inject('store')
@injectActions({ getSecurity, setTwoFactor }, 'store')
@observer
export default class Security extends Component {

  @observable securityLoaded = false;

  @observable twoFactorEnabled = false;
  @observable validTwoFactorProviders = [];
  @observable emailConfirmed = false;

  @observable settingTwoFactor = false;

  componentDidMount() {
    if(!this.securityLoaded) {
      this.props.getSecurity()
        .then(result => {
          this.handleSecurity(result);
        });
    }
  }

  render() {
    if(!this.securityLoaded)
      return <div>Loading</div>;
    
    return (
      <div>
        <Alert bsStyle={this.twoFactorEnabled ? 'success' : 'danger'}>
          Two-factor authentication is <strong>{this.twoFactorEnabled ? 'enabled' : 'disabled'}</strong>.
          <br />
          <Button
            onClick={this.toggleTwoFactorClick}
            disabled={this.settingTwoFactor}>
            {this.twoFactorEnabled ? 'Disable' : 'Enable'}
          </Button>
        </Alert>
        {(this.twoFactorEnabled && this.validTwoFactorProviders.length === 0) &&
          <Alert bsStyle="warning">
            Although you have two-factor authentication enabled, you have no valid providers to authenticate with.
          </Alert>
        }
      </div>
    );
  }

  @action
  toggleTwoFactorClick = (event) => {
    event.preventDefault();
    this.settingTwoFactor = true;
    this.props.setTwoFactor(!this.twoFactorEnabled)
      .then(result => {
        this.handleSetTwoFactor(result);
      });
  }

  @action
  handleSetTwoFactor(result) {
    this.settingTwoFactor = false;
    this.twoFactorEnabled = result.twoFactorEnabled;
  }

  @action
  handleSecurity(result) {
    this.securityLoaded = true;
    this.twoFactorEnabled = result.twoFactorEnabled;
    this.validTwoFactorProviders = result.validTwoFactorProviders;
    this.emailConfirmed = result.emailConfirmed;
  }
}
