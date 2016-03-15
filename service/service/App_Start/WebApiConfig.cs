using System.Web.Http;
using System.Web.Routing;

namespace service
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Web API routes
            config.Routes.MapHttpRoute(name: "TodayWeatherInfo API call",
                                routeTemplate: "TodayWeatherInfo/{cityInfo}"
                                , defaults: new { controller = "ServiceApi", action = "TodayWeatherInfo" }
                                , constraints: new { httpMethod = new HttpMethodConstraint("GET") }
                        );
            //TODO change it to separated call for weekend weather when the call is implemented
            config.Routes.MapHttpRoute(name: "NextWeekendWeatherInfo API call",
                                routeTemplate: "NextWeekendWeatherInfo/{cityInfo}"
                                , defaults: new { controller = "ServiceApi", action = "TodayWeatherInfo" }
                                , constraints: new { httpMethod = new HttpMethodConstraint("GET") }
                        );
        }
    }
}
