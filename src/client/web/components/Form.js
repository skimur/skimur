import React, { Component } from 'react';
import { observable, computed, action, runInAction } from 'mobx';

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
    this.value = event.target.value;
  }
}

const observableFormField = function(target, key, descripter) {
  let initialValue = descripter.initializer();
  descripter.initializer = function() {
    return runInAction(() => {
      let field = new Field();
      field.value = initialValue;
      if(!target._fields)
        target._fields = [];
      target._fields.push({ key, field });
      return field;
    });
  }
  return descripter;
}

class Form extends Component {

  @observable modelStateErrors = [];

  @computed
  get isModelStateValid() {
    return this._fields.filter(field => field.isValid).count == 0 && this.modelStateErrors.length == 0;
  }

  @action
  updateModelState(modelState) {
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
}

Form.observableFormField = observableFormField;

export default Form;