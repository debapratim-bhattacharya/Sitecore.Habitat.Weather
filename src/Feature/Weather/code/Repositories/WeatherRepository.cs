using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Foundation.Weather.Models;
using Sitecore.Mvc.Presentation;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net;
using Sitecore.Feature.Weather.Models;

namespace Sitecore.Feature.Weather.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        static HttpClient httpClient = new HttpClient();       
        
        public string URL = string.Empty;

        public static string json = string.Empty;


        public string GetCurrentWeatherJson(string url)
        {
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(url);
            }
            WeatherCurrent WeatherCurrent = (new JavaScriptSerializer()).Deserialize<WeatherCurrent>(json);
            JavaScriptSerializer js = new JavaScriptSerializer();
            json = js.Serialize(WeatherCurrent);
            return json;
        }
        public string GetForecastWeatherJson(string url)
        {
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(url);
            }
            WeatherInfo WeatherForecast = (new JavaScriptSerializer()).Deserialize<WeatherInfo>(json);
            JavaScriptSerializer js = new JavaScriptSerializer();
            json = js.Serialize(WeatherForecast);
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
            foreach (string to_replace in replacements.Keys)
            {
                URL = URL.Replace(to_replace, replacements[to_replace]);
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
            foreach (string to_replace in replacements.Keys)
            {
                URL = URL.Replace(to_replace, replacements[to_replace]);
            }
            return GetCurrentWeatherJson(URL);
        }
    }
}

