"use strict";

var React = require("react");
var Formsy = require("formsy-react");

var Input = React.createClass({
    mixins: [Formsy.Mixin],
    getDefaultProps: function() {
        return {
            showLabel: true
        };
    },
    changeValue: function(event) {
        this.setValue(event.currentTarget["value"]);
    },
    render: function () {

        var className = 'form-group ' + (this.props.className || '') + (this.showError() ? ' has-feedback has-error' : '');
        
        var errorMessage = this.getErrorMessage();
        var error;
        if (errorMessage)
            error = (<p className="help-block">{errorMessage}</p>);

        var label = this.props.showLabel === "true" ? (<label forName={this.props.name} className="control-label">{this.props.title}</label>) : null;

        return (
            <div className={className}>
                {label}
                <input id={this.props.name} name={this.props.name} placeholder={this.props.placeholder} onChange={this.changeValue} type="text" className="form-control"  />
                {error}
            </div>
        );
    }
});

module.exports = Input;