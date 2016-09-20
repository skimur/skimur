import React, { Component } from 'react';
import { inject, observer } from 'mobx-react';
import { ChangePasswordForm } from 'components';

@observer
export default class ChangePassword extends Component {
  render() {
    return (
      <div>
        <ChangePasswordForm />
      </div>
    );
  }
}
