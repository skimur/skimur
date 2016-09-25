import React, { Component } from 'react';
import { observable, action } from 'mobx';
import { inject, observer } from 'mobx-react';
import { injectActions } from 'helpers/decorators';
import { changePassword } from 'actions';
import { Input, ErrorList } from 'components';
import Form from 'components/Form';
import { Alert, Button } from 'react-bootstrap';

@inject("store")
@injectActions({ changePassword }, "store")
@observer
export default class ChangePasswordForm extends Form {

  @Form.observableFormField oldPassword = '';
  @Form.observableFormField newPassword = '';
  @Form.observableFormField newPasswordConfirm  = '';

  @observable passwordChanged = false;
  @observable processing = false;

  @action
  onClick = (event) => {
    event.preventDefault();
    if(this.processing) return;
    this.processing = true;
    this.props.actions.changePassword(this.oldPassword.value, this.newPassword.value, this.newPasswordConfirm.value)
      .then(result => {
        this.handleChangePassword(result);
      });
  }

  @action
  handleChangePassword(result) {
    this.processing = false;
    this.updateModelState(result);
    if(result.success)
      this.passwordChanged = true;
  }

  render() {
    return (
      <div>
        {this.passwordChanged &&
          <p>
          Your password has been changed.
          </p>
        }
        {!this.passwordChanged &&
          <form onSubmit={this.onClick} className="form-horizontal">
            <ErrorList errors={this.modelStateErrors} />
            <Input field={this.oldPassword} type="password" label="Current password" />
            <Input field={this.newPassword} type="password" label="New password" />
            <Input field={this.newPasswordConfirm} type="password" label="Confirm new password" />
            <div className="form-group">
              <div className="col-md-offset-2 col-md-10">
                <Button
                  bsStyle='default'
                  disabled={this.processing}
                  onClick={this.onClick}>
                  Change
                </Button>
              </div>
            </div>
          </form>
        }
      </div>
    );
  }
}