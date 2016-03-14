'use strict';

var browserify = require('browserify')
  , del = require('del')
  , source = require('vinyl-source-stream')
  , vinylPaths = require('vinyl-paths')
  , glob = require('glob')
  , Server = require('karma').Server
  , gulp = require('gulp');

// Load all gulp plugins listed in package.json
var gulpPlugins = require('gulp-load-plugins')({
  pattern: ['gulp-*', 'gulp.*'],
  replaceString: /\bgulp[\-.]/
});

// Define file path variables
var paths = {
  root: 'build/',      // App root path
  srcJs:  'src/js/',   // Source path for js files
  srcLess: 'src/less/', // Source path for less files
  srcAssets: 'src/assets/', //Source path for asset files
  distJs: 'build/js/', // Distribution path for js
  distCss:'build/css/', // Distribution path for css
  distAssets:'build/', // Distribution path for assets
  test: 'test/',     // Test path
};

/*
 * Useful tasks:
 * - gulp fast:
 *   - linting
 *   - unit tests
 *   - browserification
 *   - no minification, does not start server.
 * - gulp watch:
 *   - starts server with live reload enabled
 *   - lints, unit tests, browserifies and live-reloads changes in browser
 *   - no minification
 * - gulp:
 *   - linting
 *   - unit tests
 *   - browserification
 *   - minification and browserification of minified sources
 *   - start server for e2e tests
 *   - run Protractor End-to-end tests
 *   - stop server immediately when e2e tests have finished
 *
 * At development time, you should usually just have 'gulp watch' running in the
 * background all the time. Use 'gulp' before releases.
 */

var liveReload = true;

gulp.task('clean', function () {
  return gulp
  .src([paths.root + 'ngAnnotate', paths.root], {read: false})
  .pipe(vinylPaths(del));
});

gulp.task('lint', function () {
  return gulp
  .src(['gulpfile.js',
      paths.srcJs + '**/*.js',
      paths.test + '**/*.js',
      '!' + paths.srcJs + 'third-party/**',
      '!' + paths.test + 'browserified/**',
  ])
  .pipe(gulpPlugins.eslint())
  .pipe(gulpPlugins.eslint.format());
});

gulp.task('unit', function () {
  return gulp.src([
    paths.test + 'unit/**/*.js'
  ])
  .pipe(gulpPlugins.mocha({reporter: 'dot'}));
});

gulp.task('compile:js', ['lint', 'unit'], function () {
  return browserify(paths.srcJs + 'app.js', {debug: true})
  .bundle()
  .pipe(source('app.js'))
  .pipe(gulp.dest(paths.distJs))
  .pipe(gulpPlugins.connect.reload());
});

gulp.task('compile:css', function(){
  return gulp.src(paths.srcLess + '**/*.less')
  .pipe(gulpPlugins.less())
  .pipe(gulp.dest(paths.distCss));
});

gulp.task('compile:assets', function(){
   return gulp.src(paths.srcAssets + '**/*.{png,jpeg,gif,tiff,html,htm,json}')
  .pipe(gulp.dest(paths.distAssets));
});

gulp.task('compile', ['clean'], function(){
    gulp.start('compile:js', 'compile:css', 'compile:assets');
});

/*
somehow the clean task throws error on Windows
Error: EPERM, unlink 'c:...\weather-ctl\client\build\js\app.js'
the same happens to the project I used as example
https://github.com/basti1302/angular-browserify
all other files are removed successfully. It seems like the static http server blocks access to the file.
I attempted to use gulpPlugins.connect.serverClose();  before removing the files but got another error that happens
inside the gulp-connect addin: can't connect to undefined. So it seems like there is no internally stored connection.
        ['clean'],  
*/
gulp.task('recompile', function(){
    gulp.start('compile:js', 'compile:css', 'compile:assets');
});

gulp.task('browserify-tests', function () {
  var bundler = browserify({debug:true});
  glob
  .sync(paths.test + 'unit/**/*.js')
  .forEach(function (file) {
    bundler.add(file);
  });
  return bundler
  .bundle()
  .pipe(source('browserified_tests.js'))
  .pipe(gulp.dest(paths.test + 'browserified'));
});

gulp.task('karma', ['browserify-tests'], function (done) {
    new Server({
        configFile: __dirname + '/karma.conf.js',
        singleRun: true
    }, done).start();
});

gulp.task('server', ['compile'], function () {
  gulpPlugins.connect.server({
    root: paths.root,
    livereload: liveReload,
  });
});

gulp.task('e2e', ['server'], function () {
  return gulp.src([paths.test + 'e2e/**/*.js'])
  .pipe(gulpPlugins.protractor.protractor({
    configFile: 'protractor.conf.js',
    args: ['--baseUrl', 'http://127.0.0.1:8080'],
  }))
  .on('error', function (e) {
    throw e;
  })
  .on('end', function () {
    gulpPlugins.connect.serverClose();
  });
});

gulp.task('watch', function () {
  gulp.start('server');
  gulp.watch([
    paths.srcJs + '**/*.js',
    paths.srcLess + '**/*.less',
    paths.srcAssets + '**/*.html',
    '!' + paths.srcJs + 'third-party/**',
    paths.test + '**/*.js',
  ], ['recompile']);
});

gulp.task('default', ['compile'], function () {
   gulpPlugins.connect.server({
     root: paths.root,
     livereload: true,
   });
  gulp.src('')
  .pipe(gulpPlugins.open({app: 'Chrome', uri: 'http://localhost:8080/'}));
});