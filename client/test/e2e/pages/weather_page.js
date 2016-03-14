'use strict';

/*
 * Page object.
 */
var WeatherPage = function() {

  var driver = browser.driver;


  this.open = function() {
    browser.get('http://127.0.0.1:8080/#weather');
  };

  this.enterCityName = function(cityName) {
      enter('CityName', cityName);
  };


  this.containsText = function(text) {
    checkText(text, function(result) {
      expect(result).toBe(true);
    });
  };

  this.doesNotContainText = function(text) {
    checkText(text, function(result) {
      expect(result).toBe(false);
    });
  };

  function checkText(text, fn) {
    driver
    .isElementPresent({
      xpath: '//*[contains(text(), \'' + text + '\')]'
    }).then(fn);
  }
};

function enter(field, text) {
  field.clear();
  field.sendKeys(text);
}

module.exports = WeatherPage;