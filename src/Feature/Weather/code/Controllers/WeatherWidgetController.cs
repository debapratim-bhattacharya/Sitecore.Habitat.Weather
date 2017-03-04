using System.Web.Mvc;
using Sitecore.Mvc.Presentation;
using Sitecore.Mvc.Controllers;
using Sitecore.Data.Items;
using Sitecore.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sitecore.Data.Fields;
using System.Net;
using Sitecore.Feature.Weather.Models;
using Sitecore.Foundation.Weather.Models;
using Sitecore.Feature.Weather.Repositories;

namespace Sitecore.Feature.Weather.Controllers
{
    public class WeatherWidgetController : SitecoreController
    {
        static HttpClient httpClient = new HttpClient();
        
        private Item GetDatasourceItem()
        {
            var datasourceId = RenderingContext.Current.Rendering.DataSource;
            return ID.IsID(datasourceId) ? Context.Database.GetItem(datasourceId) : null;
        }

        private string GetGeoLocation(string locationApi)
        {
            string userIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userIP))
            {
                userIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            }
            string url = locationApi + "//" + userIP.ToString();
            WebClient client = new WebClient();
            string jsonstring = client.DownloadString(url);
            dynamic dynObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonstring);
            return dynObj.city + "," + dynObj.country_code;

        }

        public ActionResult CityWeatherList()
        {
            var currentItem = GetDatasourceItem();
            var weatherInfo = new WeatherInfo() ;
            if (currentItem == null) return View("WeatherWidget", weatherInfo);
            var locationApi = @"http://ip-api.com/json/";//currentItem.Fields["LocationApi"].Value;

            var location = GetGeoLocation(locationApi);

            var targetItem = ((LookupField)currentItem.Fields["ApiSource"]).TargetItem;

            var appId = targetItem.Fields["AppId"].Value;
            var baseUrl = targetItem.Fields["AppWeatherBaseUrl"].Value;
            var destinationUrl = targetItem.Fields["SearchByDestinationRelativeUrl"].Value;
            var temparatureUnit = targetItem.Fields["TemperatureUnit"].Value;

            var request = new WeatherRequestModel()
            {
                AppId = appId,
                BaseURL = baseUrl,
                location = location,
                RelativeURL = destinationUrl,
                Unit = temparatureUnit
            };

            var weatherServiceRepository = new WeatherRepository();

            var weatherInfoString = ((IWeatherRepository)weatherServiceRepository).GetWeatherByCity(request);
            weatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo>(weatherInfoString);
            return View("WeatherWidget", weatherInfo);
        }


    }
}
