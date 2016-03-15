using service.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace service.Controllers
{
    public class ServiceController : ApiController
    {
        #region constants
        static private string regexCityName = "^[a-zA-Z]+(?:[\\s-][a-zA-Z]+)*(,[a-zA-Z]{2,3})*$";
        static private string regexCityZip = "^\\d+,[a-zA-Z]{2,3}$";
        static private string regexCityId = "^\\d+{2,10}$";
        static private string apiKey = ""; //TODO add API key back after checkout
        #endregion
        static private WeatherCache _currentWeatherCache;
        static private WeatherCache _weekendWeatherCache;


        static ServiceController()
        {
            //TODO Move to a service locator of a DI container (add Unity?)
            _currentWeatherCache = new WeatherCache();
            _weekendWeatherCache = new WeatherCache();
        }

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

        private static async Task GetCurrentWeatherFromDataSource(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.openweathermap.org/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpResponseMessage response = await client.GetAsync("api/products/1");
                if (response.IsSuccessStatusCode)
                {
                }
            }
        }

        public IHttpActionResult TodayWeatherInfo(string cityInfo)
        {
            string weatherResponse;
            long cityId;
            if (CityIdLocator.TryLocateCityId(cityInfo, out cityId))
            {
                if (_currentWeatherCache.TryGetResponse(cityId, out weatherResponse))
                {
                    return Ok(weatherResponse);
                }
                else
                {
                    GetCurrentWeatherFromDataSource(BuildCurrentWeatherUrlAsCityId(cityId)).Wait();
                }
            }
            else {
                string requestUrl;
                if (TryBuildCurrentWeatherUrl(cityInfo, out requestUrl))
                {
                    GetCurrentWeatherFromDataSource(requestUrl).Wait();
                }
                else
                { 
                    //TODO return JSON packet with error description
                    return NotFound();
                }
            }
            //TODO return JSON packet with error description
            return NotFound();
        }

        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            var jsonString = await request.Content.ReadAsStringAsync();

            // TODO processing of request

            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
