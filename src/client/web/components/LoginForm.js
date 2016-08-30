import React, { Component } from 'react';
import Field from 'state/Field';
import { Input } from 'components';
import { action, observable, reaction } from 'mobx';
import { observer } from 'mobx-react';
import api from 'helpers/api';
import Form, { obvervableFormField } from 'state/Form'

class LoginFormState extends Form {
  @obvervableFormField userName = ''
  @obvervableFormField password = ''

  @action
  login() {
    api.login(this.userName.value, this.password.value).then(result => {
      this.updateModelState(result);
    });
  }
}

@observer
export default class LoginForm extends Component {

  @observable store = new LoginFormState();

  constructor(props) {
    super(props);
    this.store = new LoginFormState();
  }

  onClick(event) {
    event.preventDefault();
    this.store.login();
  }

  render() {
    return (
      <form className="form-horizontal">
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