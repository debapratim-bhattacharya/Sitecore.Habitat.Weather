using System.Net;

namespace Sitecore.Feature.Weather.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        public string GetLocationFromIp(string ipAddress, string apiUrl)
        {
            var url = apiUrl + "//" + ipAddress;
            var client = new WebClient();
            var jsonstring = client.DownloadString(url);
            dynamic dynObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonstring);
            if (dynObj == null)
                return string.Empty;
            return dynObj.city + "," + dynObj.country_code;
        }
    }
}