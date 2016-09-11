import React from 'react';
import { observable, action, computed } from 'mobx';
import { observer } from 'mobx-react';
import cx from 'classnames';
import { Glyphicon } from 'react-bootstrap';

const Errors = observer(({ errors }) => {
  return (
    <div>
      {errors.map((err, i) =>
      (
        <p
          className="help-block"
          key={i}>
          <Glyphicon glyph="exclamation-sign" />
          {' '}
          {err}
        </p>
      ))}
    </div>
  );
});

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

const InputOption = observer(({ field, options }) => {
  return(
    <select
      onChange={field.onChanged}
      value={field.value}
      className="form-control">
      {options.map((option, i) =>
        (
          <option key={i} value={option.value}>{option.display}</option>
        )
      )}
    </select>
  );
});

const InputCheckbox = observer(({ field, name }) => {
  return(
    <input id={name} type="checkbox" checked={field.value} onChange={field.onChanged} />
  );
});

export default observer(({ field, name, label, type, options }) => {
  if(!type) {
    type = 'text'
  }

  let input;
  if(type == 'text') {
    input = <InputText field={field} name={name} />
  } else if (type == 'password') {
    input = <InputPassword field={field} name={name} />
  } else if (type == 'option') {
    input = <InputOption field={field} name={name} options={options} />
  } else if (type == 'checkbox') {
    input = <InputCheckbox field={field} name={name} />
  } else {
    throw 'Invalid input type';
  }

  let template;

  if(type != 'checkbox') {
    template = (
      <div className={cx({
        'form-group': true,
        'has-error': !field.isValid,
      })}>
        <label className="col-md-2 control-label" htmlFor={name}>{label}</label>
        <div className="col-md-10">
          {input}
          <Errors errors={field.errors} />
        </div>
      </div>
    );
  } else {
    template = (
      <div className={cx({
        'form-group': true,
        'has-error': !field.isValid,
      })}>
        <div className='col-md-offset-2 col-md-10'>
          <div className="checkbox">
            <label htmlFor={name}>
              {input}
              {' ' + label}
            </label>
          </div>
          <Errors errors={field.errors} />
        </div>
      </div>
    );
  }

  return template;
});