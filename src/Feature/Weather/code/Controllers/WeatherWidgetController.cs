using System.Web.Mvc;
using Sitecore.Mvc.Presentation;
using Sitecore.Mvc.Controllers;
using Sitecore.Foundation.Weather.Models;
using Sitecore.Data.Items;
using Sitecore.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sitecore.Data.Fields;

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

        public async Task<ActionResult> CityWeatherList()
        {
            var currentItem = GetDatasourceItem();
            var weatherInfo = new WeatherInfo() ;
            if (currentItem == null) return View("WeatherWidget", weatherInfo);
            var apiSource = (LookupField)currentItem.Fields["ApiSource"] ;
            //var targetItem = apiSource.TargetItem;
            //var uri = targetItem.Fields["AppID"].Value;

            HttpResponseMessage response = await httpClient.GetAsync(apiSource.TargetItem.Uri.Path);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                weatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo>(data);                 
            };

            return View("WeatherWidget", weatherInfo);
        }


    }
}
