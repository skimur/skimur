"use strict";

var gulp = require("gulp"),
    rimraf = require("gulp-rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    less = require("gulp-less"),
    rename = require("gulp-rename"),
    fs = require('fs'),
    merge = require('merge-stream');

var project = require('./project.json');

var paths = {
    webroot: "./" + project.webroot + "/"
};

var components = {
    scripts: {
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
            skipMin:true
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
    },
    styles: {
        site: {
            styleSheets: [
                "Styles/site.less",
            ],
            fileName: "site.css",
            dest: paths.webroot + "css/"
        }
    }
};

gulp.task("clean:js", function () {
    var compiledJs = [];
    for (var componentKey in components.scripts) {
        if (components.scripts.hasOwnProperty(componentKey)) {
            compiledJs.push(components.scripts[componentKey].dest + components.scripts[componentKey].fileName);
        }
    }
    return gulp.src(compiledJs, { read: false })
        .pipe(rimraf({ force: true }))
        .pipe(rename({ suffix: ".min" }))
        .pipe(rimraf({ force: true }))
});

gulp.task("clean:css", function (cb) {
    var compiledCss = [];
    for (var componentKey in components.styles) {
        if (components.styles.hasOwnProperty(componentKey)) {
            compiledCss.push(components.styles[componentKey].dest + components.styles[componentKey].fileName);
        }
    }
    return gulp.src(compiledCss, { read: false })
        .pipe(rimraf({ force: true }))
        .pipe(rename({ suffix: ".min" }))
        .pipe(rimraf({ force: true }))
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("compile:font", function (cb) {
    return gulp.src("bower_components/font-awesome/fonts/*.*")
        .pipe(gulp.dest(paths.webroot + "fonts"));
})

gulp.task("compile:js", function (cb) {

    var compiledJs = [];
    for (var componentKey in components.scripts) {
        if (components.scripts.hasOwnProperty(componentKey)) {
            compiledJs.push(components.scripts[componentKey]);
        }
    }

    var streams = [];

    compiledJs.forEach(function (component) {
        streams.push(gulp.src(component.scripts)
           .pipe(concat(component.fileName))
           .pipe(gulp.dest(component.dest)));
    });

    return merge(streams);
});

gulp.task("compile:css", function (cb) {
    fs.writeFileSync("bower_components/bootstrap/less/variables.less", "@import \"../../../Styles/bootstrap-variables\";");

    var compiledCss = [];
    for (var componentKey in components.styles) {
        if (components.styles.hasOwnProperty(componentKey)) {
            compiledCss.push(components.styles[componentKey]);
        }
    }

    var streams = [];

    compiledCss.forEach(function (component) {
        streams.push(gulp.src(component.styleSheets)
           .pipe(less())
           .pipe(concat(component.fileName))
           .pipe(gulp.dest(component.dest)));
    });

    return merge(streams);
});

gulp.task("compile", ["compile:js", "compile:js", "compile:css", "compile:font"]);

gulp.task("min:js", function (cb) {

    var compiledJs = [];

    for (var componentKey in components.scripts) {
        if (components.scripts.hasOwnProperty(componentKey)) {
            compiledJs.push(components.scripts[componentKey]);
        }
    }

    var streams = [];

    compiledJs.forEach(function (component) {
        var min = true;
        if (component.skipMin != "undefined") {
            min = !component.skipMin;
        }
        if (min) {
            streams.push(gulp.src(component.dest + component.fileName)
                .pipe(uglify())
                .pipe(rename({ suffix: ".min" }))
                .pipe(gulp.dest(component.dest)));
        }
    });

    return merge(streams);

});

gulp.task("min:css", function () {

    var compiledCss = [];

    for (var componentKey in components.styles) {
        if (components.styles.hasOwnProperty(componentKey)) {
            compiledCss.push(components.styles[componentKey]);
        }
    }

    var streams = [];

    compiledCss.forEach(function (component) {
        var min = true;
        if (component.skipMin != "undefined") {
            min = !component.skipMin;
        }
        if (min) {
            streams.push(gulp.src(component.dest + component.fileName)
                .pipe(cssmin({ keepSpecialComments: "0" }))
                .pipe(rename({ suffix: ".min" }))
                .pipe(gulp.dest(component.dest)));
        }
    });

    return merge(streams);
});

gulp.task("min", ["min:js", "min:css"]);