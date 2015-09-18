var markedHelper = (function (marked) {
    "use strict";

    var exports = {},
        defaultOptions = {
            silent: false,
            highlight: null,
            langPrefix: 'lang-',
            smartypants: false,
            headerPrefix: '',
            renderer: new marked.Renderer(),
            xhtml: false
        };

    // add the class *table* so they'll be styled correctly
    defaultOptions.renderer.table = function (header, body) {
        return '<table class="table">\n'
            + '<thead>\n'
            + header
            + '</thead>\n'
            + '<tbody>\n'
            + body
            + '</tbody>\n'
            + '</table>\n';
    };

    function extend(destination, source) {
        var propertyName;

        destination = destination || {};

        for (propertyName in source) {
            if (source.hasOwnProperty(propertyName)) {
                destination[propertyName] = source[propertyName];
            }
        }

        return destination;
    }

    exports.compile = function (markdown, options) {
        options = options || {};
        var compilationOptions = extend(extend({}, defaultOptions), options);

        return marked(markdown, compilationOptions);
    };

    return exports;
}(marked));