using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Foundation.Weather.Models;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Sitecore.Feature.Weather.Scheduler.Agent
{
    public class WeatherForecast
    {
        static HttpClient httpClient = new HttpClient();
        private Item GetDatasourceItem()
        {
            var datasourceId = RenderingContext.Current.Rendering.DataSource;
            return ID.IsID(datasourceId) ? Context.Database.GetItem(datasourceId) : null;
        }
        public void Run()
        {
            var currentItem = GetDatasourceItem();
            var weatherInfo = new WeatherInfo();

            var apiSource = (LookupField)currentItem.Fields["ApiSource"];
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));



            using (httpClient)
            {
                HttpResponseMessage response = httpClient.GetAsync(apiSource.TargetItem.Uri.Path).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    weatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo>(data);
                };
            }
        }
    }
}