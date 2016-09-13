import React, { Component } from 'react';
import { inject, observer } from 'mobx-react';

@inject('store')
@observer
export default class ConfirmEmail extends Component {
  render() {
    return (
      <div>
      </div>
    );
  }
}