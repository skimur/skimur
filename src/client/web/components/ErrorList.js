import React, { Component } from 'react';
import { Glyphicon } from 'react-bootstrap';
import { observer } from 'mobx-react';

export default observer(({ errors }) => {
  if(errors.length == 0) return null;
  return (<div className="alert alert-danger">
    {errors.map((err, i) =>
    (
      <p key={i}>
        <Glyphicon glyph="exclamation-sign" />
        {' '}
        {err}
      </p>
    ))}
  </div>);
});