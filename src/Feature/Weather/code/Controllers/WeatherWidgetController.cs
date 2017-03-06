using System.Web.Mvc;
using Sitecore.Mvc.Presentation;
using Sitecore.Mvc.Controllers;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Fields;
using System.Net;
using Sitecore.Feature.Weather.Models;
using Sitecore.Foundation.Weather.Models;
using Sitecore.Feature.Weather.Repositories;

namespace Sitecore.Feature.Weather.Controllers
{
    public class WeatherWidgetController : SitecoreController
    {
        private Item GetDatasourceItem()
        {
            var datasourceId = RenderingContext.Current.Rendering.DataSource;
            return ID.IsID(datasourceId) ? Context.Database.GetItem(datasourceId) : null;
        }

        private string GetGeoLocation(string locationApi)
        {
            string userIp = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userIp))
            {
                userIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            }

            ILocationRepository locationRepository = new LocationRepository();
            return locationRepository.GetLocationFromIp(userIp, locationApi);

        }

        public ActionResult CityWeatherList()
        {
            var currentItem = GetDatasourceItem();
            var weatherInfo = new WeatherCurrent() ;
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

            IWeatherRepository weatherServiceRepository = new WeatherRepository();

            var weatherInfoString = weatherServiceRepository.GetWeatherByCity(request);
            weatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherCurrent>(weatherInfoString);
            return View("WeatherWidget", weatherInfo);
        }


    }
}
