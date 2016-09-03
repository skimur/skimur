import React, { Component } from 'react';
import { Input, ErrorList } from 'components';
import { action, observable, reaction, computed } from 'mobx';
import { observer } from 'mobx-react';
import api from 'helpers/api';
import Form, { obvervableFormField } from 'state/Form';

class RegisterFormState extends Form {

  @obvervableFormField userName = '';
  @obvervableFormField email = '';
  @obvervableFormField password = '';
  @obvervableFormField passwordConfirm = '';

  @action
  register() {
    api.register(this.userName.value, this.email.value, this.password.value, this.passwordConfirm.value).then(result => {
      this.updateModelState(result);
    });
  }
} 

@observer
export default class RegisterForm extends Component {

  store = new RegisterFormState();

  onClick(event) {
    event.preventDefault();
    this.store.register();
  }

  render() {
    return (
      <form onSubmit={this.onClick.bind(this)} className="form-horizontal">
        <ErrorList errors={this.store.errors} />
        <Input field={this.store.userName} name="userName" label="User name" />
        <Input field={this.store.email} name="userName" label="Email" />
        <Input field={this.store.password} name="userName" label="Password" />
        <Input field={this.store.passwordConfirm} name="userName" label="Confirm" />
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Register</button>
          </div>
        </div>
      </form>
    );
  }
}