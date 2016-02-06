var gulp = require("gulp")
var browserify = require("browserify");
var merge = require("merge-stream");
var rename = require("gulp-rename");
var source = require('vinyl-source-stream');
var reactify = require('reactify');
var uglify = require("gulp-uglify");
var rimraf = require("gulp-rimraf");
var paths = require("../paths");

var app = {
    vendor: {
        fileName: "app.vendor.js",
        dest: paths.webroot + "js/",
        libs: [
            "react",
            "react-dom",
            "formsy-react",
            "formsy-react-components"
        ]
    },
    scripts: [{
        src: "./React/app.react.jsx",
        fileName: "app.react.js",
        dest: paths.webroot + "js/"
    }, {
        src: "./React/app.screenedIps.jsx",
        fileName: "app.screenedIps.js",
        dest: paths.webroot + "js/"
    }]
};

var compileVendor = function () {

    var bundle = browserify({
        debug: false // Don't provide source maps for vendor libs
    })

    app.vendor.libs.forEach(function(lib) {
        bundle.require(lib);
    });

    return bundle.bundle()
        .pipe(source(app.vendor.fileName))
        .pipe(gulp.dest(app.vendor.dest));
};

var compileScripts = function (script) {

    var streams = [];

    app.scripts.forEach(function (script) {

        var bundle = browserify({
            entries: script.src,
            transform: [reactify]
        })

        app.vendor.libs.forEach(function (lib) {
            bundle.external(lib);
        });

        streams.push(bundle.bundle()
            .pipe(source(script.fileName))
            .pipe(gulp.dest(script.dest)));
    });

    return merge(streams);
};

var min = function() {
    var compiledJs = [];

    compiledJs.push(app.vendor);

    app.scripts.forEach(function(script) {
        compiledJs.push(script);
    });

    var streams = [];

    compiledJs.forEach(function (script) {
        var min = true;
        if (script.skipMin != "undefined") {
            min = !script.skipMin;
        }
        if (min) {
            streams.push(gulp.src(script.dest + script.fileName)
                .pipe(uglify())
                .pipe(rename({
                    suffix: ".min"
                }))
                .pipe(gulp.dest(script.dest)));
        }
    });

    return merge(streams);
};

var clean = function() {
    var compiledJs = [];

    compiledJs.push(app.vendor.dest + app.vendor.fileName);

    app.scripts.forEach(function (script) {
        compiledJs.push(script.dest + script.fileName);
    });

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
    return gulp.watch("./React/**/*.*", ["compile:app:scripts"]);
};

gulp.task("compile:app:vendor", compileVendor);
gulp.task("compile:app:scripts", compileScripts);
gulp.task("compile:app", ["compile:app:vendor", "compile:app:scripts"]);
gulp.task("min:app", min);
gulp.task("clean:app", clean);
gulp.task("watch:app", watch);

module.exports = {
    compileScripts: compileScripts,
    compileVendor: compileVendor,
    min: min,
    clean: clean,
    watch: watch
}