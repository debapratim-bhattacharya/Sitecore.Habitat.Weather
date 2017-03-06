using System;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Feature.Weather.Models;
using Sitecore.Feature.Weather.Repositories;
using Sitecore.Foundation.Weather.Models;
using Sitecore.Mvc.Presentation;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Sitecore.Feature.Weather.Personalization.Rules
{
    public class WeatherTemperatureCondition<T>
        : OperatorCondition<T> where T : RuleContext
    {
        public int SpecifiedValue { get; set; }

        public int Value { get; set; }

        private const string OpenWeatherMapConfigurationItemId = "A0DBDE20-5088-4CD2-A823-659075C6B8EE";

        private Item _dataSourceItem;

        private Item DataSourceItem
            => _dataSourceItem ??
            (_dataSourceItem = GetDatasourceItem() ??
                Context.Database.GetItem(OpenWeatherMapConfigurationItemId));

        private Item GetDatasourceItem()
        {
            var datasourceId = RenderingContext.Current.Rendering.DataSource;
            return ID.IsID(datasourceId) ? Context.Database.GetItem(datasourceId) : null;
        }

        private string GetLocationApi()
        {
            return DataSourceItem.Fields["LocationApi"].Value;
        }

        private string GetValueFromTargetItem(Item item, string targetItemKey, string targetFieldey)
        {
            var field = (LookupField)item.Fields[targetItemKey];
            if (field == null)
            {
                return string.Empty;
            }
            var targetItem = field.TargetItem;
            return targetItem.Fields[targetFieldey].Value;
        }

        protected override bool Execute(T ruleContext)
        {
            var userIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userIp))
            {
                userIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            var location = ((ILocationRepository)new LocationRepository())
                .GetLocationFromIp(userIp, GetLocationApi());

            var request = new WeatherRequestModel()
            {
                AppId = DataSourceItem.Fields["AppId"].Value,
                BaseURL = DataSourceItem.Fields["AppWeatherBaseUrl"].Value,
                location = location,
                RelativeURL = DataSourceItem.Fields["SearchByDestinationRelativeUrl"].Value,
                Unit = GetValueFromTargetItem(DataSourceItem, "TemperatureUnit", "Name")
            };

            IWeatherRepository weatherServiceRepository = new WeatherRepository();

            var weatherInfoString = weatherServiceRepository.GetWeatherByCity(request);
            var weatherCurrent = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherCurrent>(weatherInfoString);

            switch (this.GetOperator())
            {
                case ConditionOperator.Equal:
                    return SpecifiedValue 
                        == System.Convert.ToInt32(Math.Round(weatherCurrent.main.temp));
                case ConditionOperator.GreaterThanOrEqual:
                    return SpecifiedValue
                        >= System.Convert.ToInt32(Math.Round(weatherCurrent.main.temp));
                case ConditionOperator.GreaterThan:
                    return SpecifiedValue
                        > System.Convert.ToInt32(Math.Round(weatherCurrent.main.temp));
                case ConditionOperator.LessThanOrEqual:
                    return SpecifiedValue
                        <= System.Convert.ToInt32(Math.Round(weatherCurrent.main.temp));
                case ConditionOperator.LessThan:
                    return SpecifiedValue
                        < System.Convert.ToInt32(Math.Round(weatherCurrent.main.temp));
                case ConditionOperator.NotEqual:
                    return SpecifiedValue
                        != System.Convert.ToInt32(Math.Round(weatherCurrent.main.temp));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}