using service.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace service.Controllers
{
    public class ServiceApiController : ApiController
    {
        #region constants
        static private string dataProviderUrl = "http://api.openweathermap.org/";
        static private string regexCityName = "^[a-zA-Z]+(?:[\\s-][a-zA-Z]+)*(,[a-zA-Z]{2,3})*$";
        static private string regexCityZip = "^\\d+,[a-zA-Z]{2,3}$";
        static private string regexCityId = "^(\\d+){2,10}$";
        static private string apiKey = ""; //TODO add API key back after checkout
        #endregion
        static private WeatherCache _currentWeatherCache = new WeatherCache();
        static private WeatherCache _weekendWeatherCache = new WeatherCache();

#region Buld request URL to openweathermap.org 
        private static bool TryBuildCurrentWeatherUrlAsCityName(string cityInfo, out string url)
        {
            url = null;
            Regex regex = new Regex(regexCityName, RegexOptions.IgnoreCase);
            Match match = regex.Match(cityInfo);
            if (match.Success)
            {
                url = string.Format("data/2.5/weather?q={0}&units=metric&APPID={1}", cityInfo, apiKey);
                return true;
            }
            return false;
        }

        private static bool TryBuildCurrentWeatherUrlAsCityZip(string cityInfo, out string url)
        {
            url = null;
            Regex regex = new Regex(regexCityZip, RegexOptions.IgnoreCase);
            Match match = regex.Match(cityInfo);
            if (match.Success)
            {
                url = string.Format("data/2.5/weather?zip={0}&units=metric&APPID={1}", cityInfo, apiKey);
                return true;
            }
            return false;
        }

        private static bool TryBuildCurrentWeatherUrlAsCityId(string cityInfo, out string url)
        {
            url = null;
            Regex regex = new Regex(regexCityId, RegexOptions.IgnoreCase);
            Match match = regex.Match(cityInfo);
            long cityIdValue;
            if (match.Success &&
                Int64.TryParse(cityInfo, out cityIdValue))
            {
                url = BuildCurrentWeatherUrlAsCityId(cityIdValue);
                return true;
            }
            return false;
        }

        private bool TryBuildCurrentWeatherUrl(string cityInfo, out string url)
        {
            url = null;
            if (TryBuildCurrentWeatherUrlAsCityId(cityInfo, out url))
            {
                return true;
            };

            if (TryBuildCurrentWeatherUrlAsCityName(cityInfo, out url))
            {
                return true;
            };

            if (TryBuildCurrentWeatherUrlAsCityZip(cityInfo, out url))
            {
                return true;
            }
            //TODO add here attempt to build url using  geo coordinates etc
            return false;
        }

        private static string BuildCurrentWeatherUrlAsCityId(long cityId)
        {
            return string.Format("data/2.5/weather?id={0}&units=metric&APPID={1}", cityId, apiKey);
        }
#endregion

        private static async Task<string> GetWeatherFromDataSource(string url)
        {
            string result=null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(dataProviderUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            return result;
        }

        private static void GetDataFromJsonResponse(string cityInfo, string response, WeatherCache cache)
        {
            var results = JsonConvert.DeserializeObject<dynamic>(response);
            var sysData = results.sys;
            var coord = results.coord;
            string cityName = sysData.Name;
            string cityId = sysData.id;
            string cityCountry = sysData.country;
            string lon = coord.lon;
            string lat = coord.lat;
            
            Int64 newId;
            if (Int64.TryParse(cityId, out newId))
            {
                //updating cache
                cache.SetResponse(newId, response);
                //updating City Locator
                CityIdLocator.AddCityIdLocation(cityInfo, newId); //user request, may be zip, name only etc
                CityIdLocator.AddCityIdLocation(cityName + "," + cityCountry, newId); //city by name with country
                CityIdLocator.AddCityIdLocation("lon="+lon+",lat="+lat, newId); //geo coord in format lon=-12.5,lat=45.8
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> TodayWeatherInfo(string cityInfo)
        {
            string weatherResponse;
            long cityId;
            if (CityIdLocator.TryLocateCityId(cityInfo, out cityId))
            {
                if (_currentWeatherCache.TryGetResponse(cityId, out weatherResponse))
                {
                    return Ok(weatherResponse);
                }
            }

            string requestUrl;
            if (TryBuildCurrentWeatherUrl(cityInfo, out requestUrl))
            {
                string responseBody = await GetWeatherFromDataSource(requestUrl);
                if (responseBody == null)
                {
                    //TODO return JSON packet with error description
                    return NotFound();
                }
                GetDataFromJsonResponse(cityInfo, responseBody, _currentWeatherCache);
                return Ok(responseBody);
            }
            else
            { 
                //TODO return JSON packet with error description
                return NotFound();
            }
        }

    }
}
