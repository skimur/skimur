import React, { Component, PropTypes } from 'react';
import Helmet from 'react-helmet';
import DevTools from 'mobx-react-devtools';

import Navbar from 'react-bootstrap/lib/Navbar';
import Nav from 'react-bootstrap/lib/Nav';
import NavItem from 'react-bootstrap/lib/NavItem';

import { IndexLink } from 'react-router';
import { LinkContainer, IndexLinkContainer } from 'react-router-bootstrap';

require('./App.scss');

export default class App extends Component {
  static propTypes = {
    children: PropTypes.object.isRequired
  };
  renderLoggedInLinks(user) {
    return (
      <Nav navbar pullRight>
        <LinkContainer to="/manage">
          <NavItem>Hello {user.userName}!</NavItem>
        </LinkContainer>
        <NavItem onSelect={this.logoffClick}>
          Log off
        </NavItem>
      </Nav>
    );
  }
  renderAnonymousLinks() {
    return (
      <Nav navbar pullRight>
        <LinkContainer to="/register">
          <NavItem>Register</NavItem>
        </LinkContainer>
        <LinkContainer to="/login">
          <NavItem>Login</NavItem>
        </LinkContainer>
      </Nav>
    );
  }
  render() {
    const {
      user
    } = this.props;
    let loginLinks;
    if (user) {
      loginLinks = this.renderLoggedInLinks(user);
    } else {
      loginLinks = this.renderAnonymousLinks();
    }
    return (
      <div>
        <Navbar inverse fixedTop>
          <Navbar.Header>
            <Navbar.Brand>
              <IndexLink to="/">
                {'Skimur'}
              </IndexLink>
            </Navbar.Brand>
            <Navbar.Toggle />
          </Navbar.Header>
          <Navbar.Collapse>
            <Nav navbar>
              <IndexLinkContainer to="/">
                <NavItem>Home</NavItem>
              </IndexLinkContainer>
              <LinkContainer to="/about">
                <NavItem>About</NavItem>
              </LinkContainer>
              <LinkContainer to="/contact">
                <NavItem>Contact</NavItem>
              </LinkContainer>
            </Nav>
            {loginLinks}
          </Navbar.Collapse>
        </Navbar>
        <div className="container body-content">
          {this.props.children}
          <hr />
          <footer>
            <p>&copy; 2016 - Skimur</p>
          </footer>
        </div>
        <DevTools />
      </div>
    );
  }
}
