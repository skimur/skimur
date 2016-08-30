import React from 'react';
import { observable, action, computed } from 'mobx';
import { observer } from 'mobx-react';
import cx from 'classnames';
import { Glyphicon } from 'react-bootstrap';

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

export default observer(({ field, name, label, type }) => {
  if(!type) {
    type = 'text'
  }

  //console.log(field);

  let input;
  if(type == 'text') {
    input = <InputText field={field} />
  } else if (type == 'password') {
    input = <InputPassword field={field} />
  } else {
    throw 'Invalid input type';
  }

  return (
    <div className={cx({
      'form-group': true,
      'has-error': !field.isValid,
    })}>
      <label className="col-md-2 control-label" htmlFor={name}>{label}</label>
      <div className="col-md-10">
        {input}
        {field.errors.length > 0 &&
          <div>
            {field.errors.map((err, i) =>
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
        }
      </div>
    </div>
  );
});