import React, { Component } from 'react';
import { observable, computed, action, runInAction, extendObservable } from 'mobx';

class Field {
  constructor() {
    this.onChanged = this.onChanged.bind(this);
  }

  @observable value = '';

  @observable errors = [];

  @computed
  get isValid() {
    return this.errors.length == 0;
  }

  @action
  onChanged(event) {
    if(event.target.type === 'checkbox') {
      this.value = event.target.checked;
    } else {
      this.value = event.target.value;
    }
  }
}

function updateModelState(modelState) {
  // clear all the errors on the form
  this.modelStateErrors.replace([]);
  this._fields.forEach(field => {
    field.field.errors.replace([]);
  });

  if(!modelState) return;

  for(var key in modelState.errors) {

    // update the global errors
    if(key == '_global') {
      modelState.errors[key].forEach(error => {
        this.modelStateErrors.push(error);
      });
      continue;
    }

    // update the field errors
    let field = null;
    this._fields.forEach(possibleField => {
      if(possibleField.key == key) {
        field = possibleField.field;
      }
    });

    if(field == null) {
      console.error('The field ' + key + ' is not defined.');
    } else {
      modelState.errors[key].forEach(error => {
        field.errors.push(error);
      });
    }
  }
}

export function observableFormField(target, key, descripter) {
  let initialValue = descripter.initializer();
  descripter.initializer = function() {
    var instance = this;
    return runInAction(() => {
      let field = new Field();
      field.value = initialValue;
      if(!instance._fields) {
        instance._fields = [];
      }
      instance._fields.push({ key, field });
      return field;
    });
  }
  return descripter;
}

export function makeObservableForm(target) {
  extendObservable(target, {
      modelStateErrors: [],
      isModelStateValid: function() {
        var filtersValid = this._fields.filter(field => !field.field.isValid).length == 0;
        var modelStateValid = this.modelStateErrors.length == 0;
        return filtersValid && modelStateValid;
      }
  });
  target.updateModelState = updateModelState.bind(target);
  return target;
}

export function observableForm(target) {
  return class extends target {
    constructor() {
      super();
      makeObservableForm(this);
    }
  };
}
