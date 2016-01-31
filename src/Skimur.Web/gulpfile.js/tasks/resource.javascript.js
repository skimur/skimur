var gulp = require("gulp")
var merge = require("merge-stream");
var concat = require("gulp-concat");
var uglify = require("gulp-uglify");
var rename = require("gulp-rename");
var rimraf = require("gulp-rimraf");
var paths = require("../paths");

var scripts = {
    site: {
        scripts: [
            "bower_components/jquery/dist/jquery.js",
            "bower_components/jquery-validation/dist/jquery.validate.js",
            "bower_components/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
            "Scripts/jquery.validate.bootstrap.js",
            "bower_components/modernizr/modernizr.js",
            "bower_components/bootstrap/dist/js/bootstrap.js",
            "bower_components/respondJs/src/respond.js",
            "bower_components/remarkable-bootstrap-notify/bootstrap-notify.js",
            "bower_components/sweetalert/dist/sweetalert-dev.js",
            "bower_components/marked/lib/marked.js",
            "Scripts/markedHelper.js",
            "bower_components/to-markdown/dist/to-markdown.js",
            "bower_components/bootstrap-markdown/js/bootstrap-markdown.js",
            "bower_components/magnific-popup/dist/jquery.magnific-popup.js",
            "Scripts/app/magnific-popup-module.js",
            "Scripts/app/api.js",
            "Scripts/app/ui.js",
            "Scripts/app/login.js",
            "Scripts/app/misc.js",
            "Scripts/app/comments.js",
            "Scripts/app/posts.js",
            "Scripts/app/subs.js",
            "Scripts/app/messages.js",
            "Scripts/app/moderators.js"
        ],
        fileName: "site.js",
        dest: paths.webroot + "js/"
    },
    ace: {
        scripts: [
            "bower_components/ace-builds/src/ace.js"
        ],
        fileName: "ace.js",
        dest: paths.webroot + "js/",
        skipMin: true
    },
    aceThemeGitHub: {
        scripts: [
            "bower_components/ace-builds/src/theme-github.js"
        ],
        fileName: "theme-github.js",
        dest: paths.webroot + "js/",
        skipMin: true
    },
    aceModeCss: {
        scripts: [
            "bower_components/ace-builds/src/mode-css.js"
        ],
        fileName: "mode-css.js",
        dest: paths.webroot + "js/",
        skipMin: true
    },
    aceWorkerCss: {
        scripts: [
            "bower_components/ace-builds/src/worker-css.js"
        ],
        fileName: "worker-css.js",
        dest: paths.webroot + "js/",
        skipMin: true
    }
};

var compile = function(cb) {
    var compiledJs = [];
    for (var componentKey in scripts) {
        if (scripts.hasOwnProperty(componentKey)) {
            compiledJs.push(scripts[componentKey]);
        }
    }

    var streams = [];

    compiledJs.forEach(function(component) {
        streams.push(gulp.src(component.scripts)
            .pipe(concat(component.fileName))
            .pipe(gulp.dest(component.dest)));
    });

    return merge(streams);
}

var min = function() {
    var compiledJs = [];

    for (var componentKey in scripts) {
        if (scripts.hasOwnProperty(componentKey)) {
            compiledJs.push(scripts[componentKey]);
        }
    }

    var streams = [];

    compiledJs.forEach(function(component) {
        var min = true;
        if (component.skipMin != "undefined") {
            min = !component.skipMin;
        }
        if (min) {
            streams.push(gulp.src(component.dest + component.fileName)
                .pipe(uglify())
                .pipe(rename({
                    suffix: ".min"
                }))
                .pipe(gulp.dest(component.dest)));
        }
    });

    return merge(streams);
}

var clean = function() {
    var compiledJs = [];
    for (var componentKey in scripts) {
        if (scripts.hasOwnProperty(componentKey)) {
            compiledJs.push(scripts[componentKey].dest + scripts[componentKey].fileName);
        }
    }
    return gulp.src(compiledJs, {
            read: false
        })
        .pipe(rimraf({
            force: true
        }))
        .pipe(rename({
            suffix: ".min"
        }))
        .pipe(rimraf({
            force: true
        }))
};

var watch = function() {
    return gulp.watch("./Scripts/**/*.*", ["compile:js"]);
};

gulp.task("compile:js", compile);
gulp.task("min:js", min);
gulp.task("clean:js", clean);
gulp.task("watch:js", watch);

module.exports = {
    compile: compile,
    min: min,
    clean: clean,
    watch: watch
}