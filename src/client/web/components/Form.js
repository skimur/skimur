import React, { Component } from 'react';
import { observable, computed, action, runInAction, extendObservable } from 'mobx';
import { observableForm, observableFormField } from 'utils/state/form';

@observableForm
class Form extends Component {
}

Form.observableFormField = observableFormField;

export default Form;