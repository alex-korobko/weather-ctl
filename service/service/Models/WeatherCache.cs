using System;
using System.Collections.Generic;

namespace service.Models
{
    public class WeatherCache
    {
        public const int ExpirationTimeMinutes = 15; 
        private struct Response
        {
            public DateTime Recieved;
            public string ResponseBody;
        };

        private Dictionary<long, Response> _cachedResponses;

        public WeatherCache()
        {
            _cachedResponses = new Dictionary<long, Response>();
        }

        private void SwipeExpiredResponses()
        {
            List <long> itemsToRemove = new List<long>();
            DateTime currTime = DateTime.Now;
            foreach (var response in _cachedResponses)
            {
                if (response.Value.Recieved.AddMinutes(ExpirationTimeMinutes) < currTime)
                {
                    itemsToRemove.Add(response.Key);
                };
            }
        }

        public bool TryGetResponse(long cityId, out string response)
        {
            response = null;
            Response existingResponse;
            if (_cachedResponses.TryGetValue(cityId, out existingResponse))
            {
                if (existingResponse.Recieved.AddMinutes(ExpirationTimeMinutes) < DateTime.Now)
                {
                    //expired
                    return false;
                };
                response = existingResponse.ResponseBody;
                return true;
            }
            return false;
        }

        public void SetResponse(long cityId, string response)
        {
            SwipeExpiredResponses();
            Response newResp;
            newResp.Recieved = DateTime.Now;
            newResp.ResponseBody = response;
            _cachedResponses[cityId] = newResp; 
        }

    }
}