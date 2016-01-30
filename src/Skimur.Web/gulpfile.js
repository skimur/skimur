"use strict";

var gulp = require("gulp"),
    rimraf = require("gulp-rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    less = require("gulp-less"),
    rename = require("gulp-rename"),
    fs = require('fs'),
    merge = require('merge-stream'),
    reactify = require('reactify'),
    browserify = require('browserify'),
    source = require('vinyl-source-stream'),
    buffer = require('vinyl-buffer'),
    through = require('through2'),
    async = require("async"),
    watch = require('gulp-watch');

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
    },
    app: {
        shared: {
            fileName: "shared.app.js",
            dest: paths.webroot + "js/",
        },
        scripts: [
            {
                src: "./React/app.react.jsx",
                fileName: "app.react.js",
                dest: paths.webroot + "js/"
            },
            {
                src: "./React/screenedIps.react.jsx",
                fileName: "screenedIps.react.js",
                dest: paths.webroot + "js/"
            }
        ]
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
        .pipe(rimraf({ force: true }));
});

function getDependencies(file, cb) {
    var dependencies = [];
    var dependencyCollector = through.obj(function (row, enc, next) {
        dependencies.push(row.file);
        console.log(row);
        next();
    });
    var b = browserify({
        entries: file,
        debug: true,
        transform: [reactify]
    });
    b.pipeline.get('deps').push(dependencyCollector);
    b.bundle(function () {
        console.log(dependencies);
        cb(dependencies);
    });
}

gulp.task("clean:js:app", function () {
    var scripts = [];

    scripts.push(components.app.shared.dest + components.app.shared.fileName);

    components.app.scripts.forEach(function (script) {
        scripts.push(script.dest + script.fileName);
    });

    return gulp.src(scripts, { read: false })
        .pipe(rimraf({ force: true }))
        .pipe(rename({ suffix: ".min" }))
        .pipe(rimraf({ force: true }));
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

gulp.task("clean", ["clean:js", "clean:js:app", "clean:css"]);

gulp.task("compile:font", function (cb) {
    return gulp.src("bower_components/font-awesome/fonts/*.*")
        .pipe(gulp.dest(paths.webroot + "fonts"));
});

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

gulp.task("compile:js:app", function (cb) {
    var dependencies = [];
    var dependenciesDuplicate = [];

    async.each(components.app.scripts,
        function (item, forEachCallback) {
            getDependencies(item.src, function (scriptDeps) {
                scriptDeps.forEach(function (item) {
                    if (dependencies.indexOf(item) > -1) {
                        // this module is duplicated, used in multiple scripts at a time.
                        dependenciesDuplicate.push(item);
                    } else {
                        dependencies.push(item);
                    }
                });
                forEachCallback(null);
            });
        },
        function (err) {

            // Now that we have all the modules that are shared throughout the scripts, let's build a bundle with all of them.

            var sharedBundle = browserify({
                debug: true,
                transform: [reactify]
            });

            dependenciesDuplicate.forEach(function (item) {
                sharedBundle.require(item);
            });

            sharedBundle.bundle()
                .pipe(source(components.app.shared.fileName))
                .pipe(buffer())
                .pipe(gulp.dest(components.app.shared.dest))
                .on("end", function () {
                    async.each(components.app.scripts,
                        function (item, forEachCallback) {
                            var componentBundle = browserify({
                                entries: item.src,
                                debug: true,
                                transform: [reactify]
                            });

                            dependenciesDuplicate.forEach(function (item) {
                                componentBundle.external(item);
                            });

                            componentBundle.bundle()
                                .pipe(source(item.fileName))
                                .pipe(buffer())
                                .pipe(gulp.dest(item.dest))
                                .on("end", function () {
                                    forEachCallback();
                                });
                        },
                        function (err) {
                            cb();
                        });
                });
        });
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

gulp.task("compile", ["compile:js", "compile:js", "compile:js:app", "compile:css", "compile:font"]);

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

gulp.task("min:js:app", function () {
    var scripts = [];
    var streams = [];

    scripts.push({
        fileName: components.app.shared.fileName,
        dest: components.app.shared.dest
    });

    components.app.scripts.forEach(function (script) {
        scripts.push(script);
    });

    scripts.forEach(function (script) {
        streams.push(gulp.src(script.dest + script.fileName)
            .pipe(uglify())
            .pipe(rename({ suffix: ".min" }))
            .pipe(gulp.dest(script.dest)));
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

gulp.task("min", ["min:js", "min:js:app", "min:css"]);

gulp.task("watch", function () {
    var appScripts = [];
    async.each(components.app.scripts, function (script, cb) {
        getDependencies(script.src, function (dependencies) {
            dependencies.forEach(function (dependency) {
                appScripts.push(dependency);
            });
            cb();
        });
    }, function () {
        gulp.watch(appScripts, ["compile:js:app"]);
    });
 
    var clientScripts = [];
    for (var componentKey in components.scripts) {
        if (components.scripts.hasOwnProperty(componentKey)) {
            components.scripts[componentKey].scripts.forEach(function (script) {
                clientScripts.push(script);
            });
        }
    }
    gulp.watch(clientScripts, ["compile:js"]);
})