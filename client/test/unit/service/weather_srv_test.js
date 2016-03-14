'use strict';

var chai = require('chai')
  , expect = chai.expect;
  
var WeatherSrvModule = require('../../../src/js/service/weather_service.js');

describe('The WeatherSrvTest', function() {
  var todayWeatherUrlValue = 'today';
  var weekendWeatherUrlValue = 'weekend';
  var $http;
  $http = {
      get: function(dataUrl)
      {
          return dataUrl;
      }
  };

  var TodayWeatherInfoUrl;
  var WeekendWeatherInfoUrl;
  beforeEach(function() {
      TodayWeatherInfoUrl = todayWeatherUrlValue; 
      WeekendWeatherInfoUrl = weekendWeatherUrlValue;
  });
  
  it('should properly construct url and return data for the today weather ', function() {
        //arrane
        var cityName = 'Paris';
        var service = WeatherSrvModule($http, TodayWeatherInfoUrl, WeekendWeatherInfoUrl);
        //act
        var result = service.getCurrentWeatherData(cityName);
        //assert
        expect(result).to.equal('todayParis.json?callback=JSON_CALLBACK');
  });
  
  it('should properly construct url and return data for the weekend weather ', function() {
        //arrane
        var cityName = 'Amsterdam';
        var service = WeatherSrvModule($http, TodayWeatherInfoUrl, WeekendWeatherInfoUrl);
        //act
        var result = service.getWeekendWeatherData(cityName);
        //assert
        expect(result).to.equal('weekendAmsterdam.json?callback=JSON_CALLBACK');
  });
});
