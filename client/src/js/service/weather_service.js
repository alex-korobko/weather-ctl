'use strict';

module.exports = function($http, TodayWeatherInfoUrl, WeekendWeatherInfoUrl) {

//TODO change $HTTP get to jsonp call when the real service is ready.
    var getCurrentWeatherData = function(city){
        return $http.get(getServiceUrl(TodayWeatherInfoUrl, city));
    };

    var getWeekendWeatherData = function(city){
        return $http.get(getServiceUrl(WeekendWeatherInfoUrl, city));
    };
    
    return {
        getCurrentWeatherData : getCurrentWeatherData,
        getWeekendWeatherData : getWeekendWeatherData,
    };
};

    function getServiceUrl(serviceUrl, city)
    {
        return serviceUrl + city + '.json?callback=JSON_CALLBACK';
    }
