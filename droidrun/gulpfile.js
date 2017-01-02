var gulp = require('gulp');
var concat = require('gulp-concat');
var sequence = require('run-sequence');
var del = require('del');
var streamqueue = require('streamqueue');
var path = require('path');
var templateCache = require('gulp-angular-templatecache');
var minifyHTML = require('gulp-minify-html');
var minifyCss = require('gulp-minify-css');
var sass = require('gulp-sass');
var browserSync = require('browser-sync').create();

var wwwroot = './www/';
var src = './frontend/';

gulp.task('default', function (done) {
  sequence('clean', 'build', done);
});

gulp.task('clean', function (done) {
  del([path.join(wwwroot, '**/*')], { force: true }, done);
});

gulp.task('build', ['scripts', 'styles', 'index.html']);

gulp.task('scripts', function() {
  var bower = gulp.src([
    'bower_components/socket.io-client/socket.io.js',
    'bower_components/**/*.min.js']);

  // var templates = gulp.src(path.join(src, '**/*.html'))
  //       .pipe(minifyHTML({ empty: true, quotes: true }))
  //       .pipe(templateCache({
  //         standalone: true
  //       }));

  var srcScripts = gulp.src([
    path.join(src, '**/*.js')
  ]);

  return streamqueue.obj(bower, srcScripts).pipe(concat('auto-bundle.js'))
    .pipe(gulp.dest(wwwroot));
});

gulp.task('styles', function () {
  var sassStyles = gulp.src(path.join(src, '**/*.scss'))
                 .pipe(sass());
                 
  var cssStyles = gulp.src([
    'bower_components/**/*.min.css',
    'bower_components/skeleton/css/*.css',
    'bower_components/ngprogress/ngProgress.css',
    path.join(src, '**/*.css')]);
                 
  return streamqueue.obj(sassStyles, cssStyles).pipe(concat('auto-bundle.css'))
    .pipe(minifyCss())
    .pipe(gulp.dest(wwwroot));
});

gulp.task('index.html', function() {
  return gulp.src(path.join(src, 'index.html'))
    .pipe(minifyHTML({ empty: true, quotes: true }))
    .pipe(gulp.dest(wwwroot));
});

gulp.task('watch', function() {
  gulp.watch(path.join(src, "**/*"), ['styles', 'scripts', 'index.html']);
});

// gulp.task('serve', ['sass'], function() {
// 
//     browserSync.init({
//         server: wwwroot
//     });
// 
//     gulp.watch(path.join(src, "**/*.scss"), ['sass']);
//     gulp.watch(path.join(src, "**/*.html")).on('change', browserSync.reload);
// });
// 
// gulp.task('sass', function() {
//     var sassStyles = gulp.src(path.join(src, '**/*.scss'))
//                  .pipe(sass());
//                  
//     var cssStyles = gulp.src([
//       'bower_components/**/*.min.css',
//       'bower_components/skeleton/css/*.css',
//       path.join(src, '**/*.css')]);
//                   
//     return streamqueue.obj(sassStyles, cssStyles).pipe(concat('auto-bundle.css'))
//           .pipe(gulp.dest(wwwroot))
//           .pipe(browserSync.stream());
// });