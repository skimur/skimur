var markdownHelper = (function (markdown) {
    "use strict";

    var exports = {};
    
    exports.compile = function (content, options) {
        return markdown.toHTML(content);
    };

    return exports;

}(exports));