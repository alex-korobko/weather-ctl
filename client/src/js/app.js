'use strict';

require('es5-shim');
require('es5-sham');

require('jquery');
var angular = require('angular');
require('angular-route');

var app = angular.module('weatherControl', [ 'ngRoute' ]);

app.constant('TodayWeatherInfoUrl', '/TodayWeatherInfo/');
app.constant('WeekendWeatherInfoUrl', '/NextWeekendWeatherInfo/');
app.constant('ValidInputPattern', '\\d+');

require('./service');
require('./controller');

app.config(function($routeProvider) {

  $routeProvider.when('/weather', {
    templateUrl: '/templates/weather_ctl.html',
    controller: 'WeatherCtl',
  })
  //TODO: add other routes here
  .otherwise({
    redirectTo: '/weather',
  });
});