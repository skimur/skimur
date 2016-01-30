var gulp = require("gulp");
var rimraf = require("gulp-rimraf");
var paths = require("../paths");

var compile = function() {
	return gulp.src("bower_components/font-awesome/fonts/*.*")
		.pipe(gulp.dest(paths.webroot + "fonts"));
}

var clean = function() {
	return gulp.src(paths.webroot + "fonts/*.*", {
			read: false
		})
		.pipe(rimraf({
			force: true
		}));
}

gulp.task("compile:font", compile);
gulp.task("clean:font", clean)

module.exports = {
	compile: compile,
	clean: clean
}