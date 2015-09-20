var markedHelper = (function (marked) {
    "use strict";

    var exports = {},
        defaultOptions = {
            gfm: true,
            tables: true,
            breaks: false,
            pedantic: false,
            sanitize: true,
            smartLists: true,
            silent: false,
            highlight: null,
            langPrefix: 'lang-',
            smartypants: false,
            headerPrefix: '',
            renderer: new marked.Renderer(),
            xhtml: false
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
        var compilationOptions;

        options = options || {};
        compilationOptions = extend(extend({}, defaultOptions), options);
        compilationOptions.ment = [];

        var rendered = marked(markdown, compilationOptions);

        return {
            result: rendered,
            mentions: compilationOptions.ment
        };
    };

    return exports;
}(marked));