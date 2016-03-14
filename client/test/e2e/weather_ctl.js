'use strict';

var WeatherPage = require('./pages/weather_page');

describe('The weather control integrated in a page', function() {

  var weatherPage;

  beforeEach(function() {
    weatherPage = new WeatherPage();
    weatherPage.open();
  });

  it('show current weather for a city', function() {
  });
});