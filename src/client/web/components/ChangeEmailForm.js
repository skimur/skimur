import React, { Component } from 'react';
import { observable, action } from 'mobx';
import { inject, observer } from 'mobx-react';
import { injectActions } from 'helpers/decorators';
import { changeEmail } from 'actions';
import { Input, ErrorList } from 'components';
import Form from 'components/Form';
import { Alert, Button } from 'react-bootstrap';

@inject("store")
@injectActions({ changeEmail }, "store")
@observer
export default class ChangeEmailForm extends Form {

  @Form.observableFormField email = '';
  @Form.observableFormField emailConfirm = '';
  @Form.observableFormField currentPassword = '';

  @observable processing = false;

  @observable changeEmailSent = false;

  @action
  onClick = (event) => {
    event.preventDefault();
    if(this.processing) return;
    this.processing = true;
    this.props.actions.changeEmail(this.email.value, this.emailConfirm.value, this.currentPassword.value)
      .then(result => {
        this.handleChangeEmail(result);
      });
  }

  @action
  onConfirmClick = (event) => {
    event.preventDefault();
    this.changeEmailSent = false;
    this.email.value = '';
    this.emailConfirm.value = '';
    this.currentPassword.value = '';
  }

  @action
  handleChangeEmail(result) {
    this.processing = false;
    this.updateModelState(result);
    if(result.success)
      this.changeEmailSent = true;
  }

  render() {
    if(this.changeEmailSent) {
      return (<Alert bsStyle={'success'}>
          A link has been sent to your e-mail to finish up changing your e-mail
          <br />
          <Button
            onClick={this.onConfirmClick}>
            Ok
          </Button>
        </Alert>)
    }

    return (
      <form onSubmit={this.onClick} className="form-horizontal">
        <ErrorList errors={this.modelStateErrors} />
        <Input field={this.email} name="email" label="Email" />
        <Input field={this.emailConfirm} name="email" label="Confirm" />
        <Input field={this.currentPassword} name="password" type="password" label="Current password" />
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
    );
  }
}