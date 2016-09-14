import React from 'react';
import { Button } from 'react-bootstrap';

const bootstrapSocial = require('bootstrap-social');
const fontAwesome = require('font-awesome/scss/font-awesome.scss');

console.log(fontAwesome);
console.log(bootstrapSocial);

export default (props) => {
  return (
    <Button
      {...props}
      className={bootstrapSocial['btn-social'] + ' ' + bootstrapSocial['btn-' + props.scheme.toLowerCase()]}>
      <span className={fontAwesome.fa + ' ' + fontAwesome['fa-' + props.scheme.toLowerCase()]} />
      {props.text}
    </Button>
  );
}
