var gulp = require("gulp");
var fs = require('fs');
var less = require("gulp-less");
var concat = require("gulp-concat");
var merge = require("merge-stream");
var rimraf = require("gulp-rimraf");
var rename = require("gulp-rename");
var cssmin = require("gulp-cssmin");
var paths = require("../paths");

var styles = {
	site: {
		styleSheets: [
			"Styles/site.less",
		],
		fileName: "site.css",
		dest: paths.webroot + "css/"
	}
};

var compile = function(cb) {
	fs.writeFileSync("bower_components/bootstrap/less/variables.less", "@import \"../../../Styles/bootstrap-variables\";");

	var compiledCss = [];
	for (var componentKey in styles) {
		if (styles.hasOwnProperty(componentKey)) {
			compiledCss.push(styles[componentKey]);
		}
	}

	var streams = [];

	compiledCss.forEach(function(component) {
		streams.push(gulp.src(component.styleSheets)
			.pipe(less())
			.pipe(concat(component.fileName))
			.pipe(gulp.dest(component.dest)));
	});

	return merge(streams);
}

var min = function() {
	var compiledCss = [];

	for (var componentKey in styles) {
		if (styles.hasOwnProperty(componentKey)) {
			compiledCss.push(styles[componentKey]);
		}
	}

	var streams = [];

	compiledCss.forEach(function(component) {
		var min = true;
		if (component.skipMin != "undefined") {
			min = !component.skipMin;
		}
		if (min) {
			streams.push(gulp.src(component.dest + component.fileName)
				.pipe(cssmin({
					keepSpecialComments: "0"
				}))
				.pipe(rename({
					suffix: ".min"
				}))
				.pipe(gulp.dest(component.dest)));
		}
	});

	return merge(streams);
}

var clean = function()
{
	var compiledCss = [];
    for (var componentKey in styles) {
        if (styles.hasOwnProperty(componentKey)) {
            compiledCss.push(styles[componentKey].dest + styles[componentKey].fileName);
        }
    }
    return gulp.src(compiledCss, { read: false })
        .pipe(rimraf({ force: true }))
        .pipe(rename({ suffix: ".min" }))
        .pipe(rimraf({ force: true }))
}

gulp.task("compile:css", compile);
gulp.task("min:css", min);
gulp.task("clean:css", clean);

module.exports = {
    compile: compile,
    min: min,
    clean: clean
}