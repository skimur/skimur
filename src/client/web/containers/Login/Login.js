import React, { Component } from 'react';
import { LoginForm } from 'components';
import { Link } from 'react-router';
import { inject, observer } from 'mobx-react';
import { navigateTo } from 'actions';
import { injectActions } from 'helpers/decorators';

@inject('store')
@injectActions({ navigateTo }, 'store')
@observer
export default class Login extends Component {
  onLoggedIn = () => {
    if(this.props.store.nav.query.returnUrl) {
      this.props.actions.navigateTo(this.props.store.nav.query.returnUrl);
    } else {
      this.props.actions.navigateTo('/');
    }
  }
  componentWillMount() {
    this.setState({
      isShowing: true
    });
  }
  onClick = (event) => {
    event.preventDefault();
    this.setState({
      isShowing: false
    });
  }
  render() {
    console.log(this.state);
    const { isShowing } = this.state
    if(!isShowing) {
      setTimeout(() => {
        this.setState({isShowing: true});
      }, 1000)
    }
    return (
      <div>
        <h2>Login</h2>
        <hr />
        {isShowing && 
          <LoginForm onLoggedIn={this.onLoggedIn} />
        }
        <p>
          <Link to="/register">Register as a new user?</Link>
        </p>
        <p>
          <Link to="/forgotpassword">Forgot your password?</Link>
        </p>
        <button onClick={this.onClick}>
          Flip
        </button>
      </div>
    );
  }
}