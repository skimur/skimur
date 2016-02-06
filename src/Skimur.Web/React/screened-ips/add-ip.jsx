var React = require("react");
var TextInput = require("../components/inputs/textInput.jsx");
var Formsy = require("formsy-react");
var FormsyComponents = require('formsy-react-components');

var AddIp = React.createClass({
    render: function () {
        return (
            <Formsy.Form className="form-inline">
                <FormsyComponents.Input name="text1"
                       id="artisanCraftedBespokeId"
                       value=""
                       type="text"
                       layout="vertical"
                       placeholder="Here is a text input."
                       help="This is a required text input."
                       required />
            </Formsy.Form>
        );
    }
});

module.exports = AddIp;