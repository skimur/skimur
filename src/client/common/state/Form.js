import { observable, computed, action } from 'mobx';

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

const obvervableFormField = function(target, key, descripter) {
  let initialValue = descripter.initializer();
  descripter.initializer = function() {
    let field = new Field();
    field.value = initialValue;
    if(!target._fields)
      target._fields = [];
    target._fields.push({ key, field });
    return field;
  }
  return descripter;
}

export { obvervableFormField }

export default class Form {

  @observable errors = [];

  @computed 
  get isValid() {
    return this._fields.filter(field => field.isValid).count == 0 && this.errors.length == 0;
  }

  @action
  updateModelState(modelState) {
    // clear all the errors on the form
    this.errors.replace([]);
    this._fields.forEach(field => {
      field.field.errors.replace([]);
    });

    if(!modelState) return;

    for(var key in modelState.errors) {

      // update the global errors
      if(key == '_global') {
        modelState.errors[key].forEach(error => {
          this.errors.push(error);
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