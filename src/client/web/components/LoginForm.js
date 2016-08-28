import React, { Component } from 'react';
import { observable, action, computed } from 'mobx';
import { observer } from 'mobx-react';
import classNames from 'classnames';

class Field {
  constructor() {
    this.onChanged = this.onChanged.bind(this);
  }

  @observable touched = false;

  @observable value = null;

  @observable errors = [];

  @computed 
  get isValid() {
    return this.errors.length == 0;
  }

  @action
  onChanged(event) {
    this.value = event.target.value;
  }
}

const InputText = observer(({ field }) => {
  return <input
    onChange={field.onChanged}
    type="text"
    value={field.value}
    className="form-control"
  />
});

const InputPassword = observer(({ field }) => {
  return <input
    onChange={field.onChanged}
    type="password"
    value={field.value}
    className="form-control"
  />
});

const Input = observer(({ field, name, label, type }) => {
  if(!type) {
    type = 'text'
  }
  const rowClass = classNames({
    'form-group': true,
    'has-error': !field.isValid,
  });

  let input;
  if(type == 'text') {
    input = <InputText field={field} />
  } else if (type == 'password') {
    input = <InputPassword field={field} />
  } else {
    throw 'Invalid input type';
  }

  return (
    <div className={rowClass}>
      <label className="col-md-2 control-label" htmlFor={name}>{label}</label>
      <div className="col-md-10">
        {input}
      </div>
    </div>
  );
});

@observer
export default class LoginForm extends Component {

  userName = new Field()
  password = new Field();

  render() {
    return (
      <form className="form-horizontal">
        <Input field={this.userName} name="userName" label="User name" />
        <Input field={this.password} name="password" label="Password" />
        <div className="form-group">
          <div className="col-md-offset-2 col-md-10">
            <button type="submit" className="btn btn-default">Login</button>
          </div>
        </div>
      </form>
    );
  }
}