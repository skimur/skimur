import React, { Component } from 'react';
import { inject, observer } from 'mobx-react';
import { Link } from 'react-router';

@inject('store')
@observer
export default class ConfirmEmail extends Component {
  render() {
    const {
      success,
      change
    } = this.props.store.viewBag;
    return (
      <div>
        {!success &&
          <div>
            <h1 className="text-danger">Error.</h1>
            <h2 className="text-danger">An error occurred while processing your request.</h2>
          </div>
        }
        {success &&
          <div>
            {change &&
              <div>
                <h2>Change email</h2>
                <p>
                  Your email was succesfully changed.
                </p>
              </div>
            }
            {!change &&
              <div>
                <h2>Confirm email</h2>
                <p>
                  Thank you for confirming your email.
                  Please <Link to="/login">Click here to Log in</Link>.
                </p>
              </div>
            }
          </div>
        }
      </div>
    );
  }
}