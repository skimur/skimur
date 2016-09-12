import React, { Component } from 'react';
import { observable, action } from 'mobx';
import { inject, observer } from 'mobx-react';
import { injectActions } from 'helpers/decorators';
import { getEmail, changeEmail, verifyEmail } from 'actions';
import { Alert, Button } from 'react-bootstrap';
import { ChangeEmailForm } from 'components';

@inject("store")
@injectActions({ getEmail, changeEmail, verifyEmail }, 'store')
@observer
export default class Email extends Component {

  @observable emailLoaded = false;

  @observable email = '';
  @observable emailConfirmed = false;

  @observable sendingEmailVerification = false;

  componentDidMount() {
    if(!this.emailLoaded) {
      this.props.getEmail()
        .then(result => {
          this.handleEmail(result);
        });
    }
  }

  render() {
    if(!this.emailLoaded)
      return <div>Loading</div>;
    
    return (
      <div>
        <h2>Email</h2>
        <div className="form-horizontal">
          <div className="form-group">
            <label className="col-md-2 control-label" htmlFor="currentEmail">Current email</label>
            <div className="col-md-10">
              <p id="currentEmail" className="form-control-static">{this.email}</p>
            </div>
          </div>
        </div>
        {!this.emailConfirmed &&
          <Alert bsStyle="danger">
            Your email is not verified.
            <br />
            <Button
              onClick={this.verifyClick}
              disabled={this.sendingEmailVerification}>
              Verify
            </Button>
          </Alert>
        }
        <h3>Change your email</h3>
        <ChangeEmailForm />
      </div>
    );
  }

  @action
  verifyClick = (event) => {
    event.preventDefault();
    this.sendingEmailVerification = true;
    this.props.verifyEmail()
      .then(result => {
        this.handleVerifyEmail(result);
      });
  }

  @action
  handleVerifyEmail(result) {
    this.sendingEmailVerification = false;
  }

  @action
  handleEmail(result) {
    this.emailLoaded = true;
    this.email = result.email;
    this.emailConfirmed = result.emailConfirmed;
  }
}
