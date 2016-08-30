import React, { Component } from 'react';
import { Input, ErrorList } from 'components';
import { action, observable, reaction, computed } from 'mobx';
import { observer } from 'mobx-react';
import api from 'helpers/api';
import Form, { obvervableFormField } from 'state/Form';

class LoginFormState extends Form {

  @obvervableFormField userName = '';
  @obvervableFormField password = '';

  @action
  login() {
    api.login(this.userName.value, this.password.value).then(result => {
      this.updateModelState(result);
    });
  }
} 

@observer
export default class LoginForm extends Component {

  store = new LoginFormState();

  onClick(event) {
    event.preventDefault();
    this.store.login();
  }

  render() {
    return (
      <form className="form-horizontal">
        <ErrorList errors={this.store.errors} />
        <Input field={this.store.userName} name="userName" label="User name" />
        <Input field={this.store.password} name="password" label="Password" />
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default" onClick={this.onClick.bind(this)}>Login</button>
          </div>
        </div>
      </form>
    );
  }
}