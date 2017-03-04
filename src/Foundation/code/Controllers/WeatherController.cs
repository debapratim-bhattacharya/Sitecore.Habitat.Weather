using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using Sitecore.Foundation.Weather.Models;
using System.Web.Script.Serialization;

namespace Sitecore.Foundation.Weather.Controllers
{
    public class WeatherController : ApiController
    {
        string appId = "95fea370fea973f538d68ad88ec5e9cd";
        string json = string.Empty;
        string url = string.Empty;
        // /api/Math/GetWeatherByCity/?city=cityname
        [System.Web.Http.HttpGet]
        public string GetWeatherByCity(string city)
        {

            url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}&units={2}", city, appId, "celsius");
            return GetCurrentWeatherJson(url);
        }

        // /api/Math/GetWeatherByGeolocation/?latitude=latitude&longitude=longitude
        [System.Web.Http.HttpGet]
        public string GetWeatherByGeolocation(string latitude, string longitude)
        {
            url = string.Format("http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&units={3}", latitude, longitude, appId, "celsius");
            return GetCurrentWeatherJson(url);
        }

        [System.Web.Http.HttpGet]
        public string GetWeatherForecastByGeolocation(string latitude, string longitude)
        {
            url = string.Format("http://api.openweathermap.org/data/2.5/forecast/daily?lat={0}&lon={1}&appid={2}&units={3}", latitude, longitude, appId, "celsius");
            return GetForecastWeatherJson(url);
        }

        [System.Web.Http.HttpGet]
        public string GetWeatherForecastByCity(string city)
        {
            url = string.Format("http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&cnt={1}&APPID={2}&units={3}", city, 5, appId, "celsius");
            return GetForecastWeatherJson(url);
        }






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
            WeatherInfo weatherInfo = (new JavaScriptSerializer()).Deserialize<WeatherInfo>(json);
            JavaScriptSerializer js = new JavaScriptSerializer();
            json = js.Serialize(weatherInfo);
            return json;
        }
    }
}