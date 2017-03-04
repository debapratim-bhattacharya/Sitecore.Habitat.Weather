using System.Collections.Generic;
using Sitecore.Foundation.Weather.Models;
using System.Web.Script.Serialization;
using System.Net;
using Sitecore.Feature.Weather.Models;

namespace Sitecore.Feature.Weather.Repositories
{
    public class WeatherRepository : IWeatherRepository
    { 
        public string URL = string.Empty;

        public static string json = string.Empty;


        public string GetCurrentWeatherJson(string url)
        {
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(url);
            }

            var weatherCurrent = (new JavaScriptSerializer()).Deserialize<WeatherCurrent>(json);
            var js = new JavaScriptSerializer();
            json = js.Serialize(weatherCurrent);
            return json;
        }
        public string GetForecastWeatherJson(string url)
        {
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(url);
            }
            var weatherForecast = (new JavaScriptSerializer()).Deserialize<WeatherInfo>(json);
            var js = new JavaScriptSerializer();
            json = js.Serialize(weatherForecast);
            return json;
        }

        string IWeatherRepository.GetWeatherByCity(WeatherRequestModel request)
        {
            URL = request.BaseURL + request.RelativeURL;
            Dictionary<string, string> replacements = new Dictionary<string, string>
            {
                { "{{city}}", request.location },
                {"{{appid}}",request.AppId },
                {"{{metric}}",request.Unit }
            };
            foreach (string toReplace in replacements.Keys)
            {
                URL = URL.Replace(toReplace, replacements[toReplace]);
            }
            return GetCurrentWeatherJson(URL);
        }


        string IWeatherRepository.GetWeatherForecastByCity(WeatherRequestModel request)
        {
            URL = request.BaseURL + request.RelativeURL;
            Dictionary<string, string> replacements = new Dictionary<string, string>
            {
                { "{{city}}", request.location },
                {"{{appid}}",request.AppId },
                {"{{metric}}",request.Unit }
            };
            foreach (string toReplace in replacements.Keys)
            {
                URL = URL.Replace(toReplace, replacements[toReplace]);
            }
            return GetCurrentWeatherJson(URL);
        }
    }

}
