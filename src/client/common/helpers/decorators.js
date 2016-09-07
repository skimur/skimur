import React from 'react';
import _ from 'lodash';

export function nonenumerable(target, name, descriptor) {
  descriptor.enumerable = false;
  return descriptor;
}

function createActionInjector(actions, component, store) {
  const Injector = React.createClass({
    displayName: "ActionInjector",
    render: function() {
      if(_.isUndefined(actions)) {
        // no actions to bind. shouldn't happen'
        return React.createElement(component, this.props);
      }

      let bindingStore = null;
      if (_.isFunction(store)) {
        bindingStore = store(this.props);
      } else if(_.isString(store)) {
        bindingStore = this.props[store];
        if(_.isUndefined(bindingStore)) {
          throw 'Could not find the store to bind the action with the key of ' + store;
        }
      }

      let newProps = {};

      // copy over all the old properties
      for (let key in this.props) if (this.props.hasOwnProperty(key)) {
        newProps[key] = this.props[key];
      }

      // bind all the actions
      for (let key in actions) {
        newProps[key] = actions[key](bindingStore);
      }

      newProps.ref = instance => {
        this.wrappedInstance = instance;
      }

      return React.createElement(component, newProps);
    }
  });
  Injector.wrappedComponent = component;
  return Injector;
}

export function injectActions(actions, store) {
  return component => {
    return createActionInjector(actions, component, store);
  };
}