"use strict";

var React = require("react");
var ReactDOM = require('react-dom');
var AddIp = require("./screened-ips/add-ip.jsx");

var ScreenedIps = React.createClass({
    addIp: function (event) {
        console.log(event);
    },
    render: function () {
        return (
            <div>
                <AddIp onAddIp={this.addIp} />
            </div>
        );
    }
});

ReactDOM.render((
    <ScreenedIps />
    ),
    document.getElementById("screened-ips"));