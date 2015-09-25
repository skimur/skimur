var gulp = require('gulp');
var del = require('del');
var msbuild = require("gulp-msbuild");
var fs = require('fs');
var request = require('request');
var shell = require('gulp-shell');

gulp.task('default', ['local'], function() {
  // place code for your default task here
});

gulp.task('local', ['compile'], function() {
  // place code for your default task here
});

gulp.task('clean', function() {
  return del([
    'build/',
    'dist/'
  ]);
});

gulp.task('compile', ['nuget-restore'], function() {
  // return gulp
  //   .src('**/*.sln')
  //   .pipe(msbuild({
  //     toolsVersion: 14.0,
  //     targets: ['Clean', 'Build'],
  //     errorOnFail: true,
  //     stdout: true
  //   }));
});

gulp.task('test', function() {

});

gulp.task('nuget-download', function(done) {
    if(fs.existsSync('nuget.exe')) {
        done();
        return;
    }

    request.get('http://nuget.org/nuget.exe')
        .pipe(fs.createWriteStream('nuget.exe'))
        .on('close', done);
});

gulp.task('nuget-restore', ['nuget-download'], function() {
	return gulp.src('**/*.sln', {read: false})
    .pipe(shell([
      'nuget restore <%= (file.path) %>'
    ]))
});