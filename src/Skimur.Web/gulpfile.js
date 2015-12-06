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

var bowerScripts = [
    "bower_components/jquery/dist/jquery.js"
];

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("compile:js", function (cb) {
    return gulp.src(bowerScripts)
        .pipe(concat("site.js"))
        .pipe(gulp.dest(paths.webroot + "js"));
});

gulp.task("compile:css", function (cb) {
    fs.writeFileSync("bower_components/bootstrap/less/variables.less", "@import \"../../../Styles/bootstrap-variables\";");
    return gulp.src('Styles/site.less')
            .pipe(less())
            .pipe(gulp.dest(project.webroot + '/css'));
});

gulp.task("compile", ["compile:js", "compile:css"]);