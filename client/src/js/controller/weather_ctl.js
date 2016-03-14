'use strict';

module.exports = function($scope, ValidInputPattern,  WeatherService) {
    $scope.todayWeatherReady = false;
    $scope.todayWeatherErrorMes = '';
    $scope.weekendWeatherReady = false;
    $scope.weekendWeatherErrorMes = '';
    $scope.validInfoPattern = ValidInputPattern;
    
    $scope.city = 'London';

    $scope.refreshData = function()
    {
        WeatherService.getCurrentWeatherData($scope.city).then(function(response){
            $scope.todayWeather = response.data;
            $scope.todayWeatherReady = true;
        }, function (response) {
            $scope.todayWeatherReady = false;
            $scope.todayWeatherErrorMes = response.data || 'Request for today weather failed';
        });

        WeatherService.getWeekendWeatherData($scope.city).then(function(response){
            $scope.weekendWeather = response.data;
            $scope.weekendWeatherReady = true;
        }, function (response) {
            $scope.weekendWeatherReady = false;
            $scope.weekendWeatherErrorMes = response.data || 'Request for weekend weather failed';
        });        
    };
    
     $scope.refreshData();
};