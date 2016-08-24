import React, { Component } from 'react';
import { inject, observer } from 'mobx-react';
import Helmet from 'react-helmet';

@inject("store") @observer
export default class Home extends Component {
  constructor(props) {
    super(props);
    this.buttonClick = this.buttonClick.bind(this);
  }
  buttonClick() {
    this.props.store.test = 'dsfsdf';
  }
  render() {
    return (
      <div>
        <Helmet title="Home" />
        <p>{this.props.store.test}</p>
        <button onClick={this.buttonClick}>
        </button>
      </div>
    );
  }
}
