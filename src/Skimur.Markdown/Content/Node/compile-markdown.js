
var path = require('path');
var marked = require(path.resolve(__dirname, 'marked.js'));
var markedHelper = require(path.resolve(__dirname, 'markedHelper.js'))(marked);

module.exports = {
    compileMarkdown: function (callback, options) {
        // options = markdown content
        var result = markedHelper.compile(options);
        callback(null, result);
    }
};