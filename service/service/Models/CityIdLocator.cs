using System.Collections.Generic;

namespace service.Models
{
    public static class CityIdLocator
    {
        //It real world the class should probably have dictionary of cities for the city.list.json.gz file:
        //http://bulk.openweathermap.org/sample/
        //and of course it is necessary to add a timestamp and remove not used items. 
        static private Dictionary<string, long> _cityIds = new Dictionary<string, long>();
        public static bool TryLocateCityId(string cityInfo, out long cityId)
        {
            return _cityIds.TryGetValue(cityInfo, out cityId);
        }
        
        public static void AddCityIdLocation(string location, long cityId)
        {
            _cityIds[location] = cityId;
        }
    }
}