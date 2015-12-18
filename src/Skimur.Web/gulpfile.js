"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    less = require("gulp-less"),
    fs = require('fs');

var project = require('./project.json');

var paths = {
    webroot: "./" + project.webroot + "/"
};
paths.concatJsDest = paths.webroot + "js/site.js";
paths.concatCssDest = paths.webroot + "css/site.css";
paths.concatJsScripts = [
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
];
paths.aceJsScripts = [
    "bower_components/ace-builds/src/ace.js",
    "bower_components/ace-builds/src/theme-github.js",
    "bower_components/ace-builds/src/mode-css.js"
];

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("compile:font", function (cb) {
    return gulp.src("bower_components/font-awesome/fonts/*.*")
        .pipe(gulp.dest(paths.webroot + "fonts"));
})

gulp.task("compile:js", function (cb) {
    return gulp.src(paths.concatJsScripts)
        .pipe(concat("site.js"))
        .pipe(gulp.dest(paths.webroot + "js"));
});

gulp.task("compile:js-ace", function(cb) {
    return gulp.src(paths.aceJsScripts)
        .pipe(gulp.dest(paths.webroot + "js"));
});

gulp.task("compile:css", function (cb) {
    fs.writeFileSync("bower_components/bootstrap/less/variables.less", "@import \"../../../Styles/bootstrap-variables\";");
    return gulp.src('Styles/site.less')
            .pipe(less())
            .pipe(gulp.dest(project.webroot + '/css'));
});

gulp.task("compile", ["compile:js", "compile:js-ace", "compile:css", "compile:font"]);