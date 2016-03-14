'use strict';

var chai = require('chai')
  , expect = chai.expect;
  
var WeatherCtrlModule = require('../../../src/js/controller/weather_ctl.js');

describe('The WeatherCtlTest', function() {
  var testValue = 'test';
  var rejectedValue = 'rejected';
  var resolvedValue = 'resolved';
  var resolvePromise;
  var testData = {
      data:testValue
  };
  var $scope;
  var ValidInputPattern = '\\d+'; 
  var promiseMock = function () {
      return {
          then:function (resolve, reject) {
              if (resolvePromise &&
                    typeof resolve === 'function') {
                return resolve(testData); 
              }
              if (! resolvePromise &&
                    typeof reject === 'function') {
                return reject(testData);
              }
             }
        };
    };

  beforeEach(function() {
    resolvePromise = true;
    $scope = {
      todayWeatherReady : testValue,
      todayWeatherErrorMes : testValue,
      todayWeather:testValue,
      weekendWeatherReady : testValue,
      weekendWeatherErrorMes : testValue,
      weekendWeather:testValue,
      validInfoPattern : testValue,
      city : testValue
    };

    var WeatherService = {
      getCurrentWeatherData: promiseMock,
      getWeekendWeatherData: promiseMock
    };
      
    WeatherCtrlModule($scope, ValidInputPattern, WeatherService);
    });


    it('should preset values', function() {
        expect($scope.validInfoPattern).to.equal(ValidInputPattern);
        expect($scope.city).to.equal('London');
    });
    
    it('should switch to correct state if promise is rejected', function() {
        //arrange
        resolvePromise = false;
        testData = {
            data:rejectedValue
        };
        //act 
        $scope.refreshData();
        //assert
        expect($scope.todayWeatherReady).to.equal(false);
        expect($scope.todayWeatherErrorMes).to.equal(rejectedValue);
        expect($scope.todayWeather).to.equal(testValue);
        expect($scope.weekendWeatherReady).to.equal(false);
        expect($scope.weekendWeatherErrorMes).to.equal(rejectedValue);
        expect($scope.weekendWeather).to.equal(testValue);
    });
 
    it('should switch to correct state if promise is resolved', function() {
        //arrange
        resolvePromise = true;
        testData = {
            data:resolvedValue
        };
        //act 
        $scope.refreshData();
        //assert
        expect($scope.weekendWeatherReady).to.equal(true);
        expect($scope.weekendWeatherErrorMes).to.equal('');
        expect($scope.weekendWeather).to.equal(resolvedValue);
        expect($scope.todayWeatherReady).to.equal(true);
        expect($scope.todayWeatherErrorMes).to.equal('');
        expect($scope.todayWeather).to.equal(resolvedValue);
    });
});