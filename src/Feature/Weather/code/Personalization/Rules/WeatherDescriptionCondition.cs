using System;
using System.Linq;
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
    public class WeatherDescriptionCondition<T>
        : StringOperatorCondition<T> where T : RuleContext
    {
        public Item SpecifiedValue { get; set; }

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
            var field = (LookupField) item.Fields[targetItemKey];
            if (field == null)
            {
                return string.Empty;
            }
            var targetItem = field.TargetItem;
            return targetItem.Fields[targetFieldey].Value;
        }

        protected override bool Execute(T ruleContext)
        {
            string userIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userIp))
            {
                userIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            var location = ((ILocationRepository) new LocationRepository())
                .GetLocationFromIp(userIp, GetLocationApi());

            var request = new WeatherRequestModel()
            {
                AppId = DataSourceItem.Fields["AppId"].Value,
                BaseURL = DataSourceItem.Fields["AppWeatherBaseUrl"].Value,
                location = location,
                RelativeURL = DataSourceItem.Fields["SearchByDestinationRelativeUrl"].Value,
                Unit = GetValueFromTargetItem(DataSourceItem, "TemperatureUnit","Name")
            };

            IWeatherRepository weatherServiceRepository = new WeatherRepository();

            var weatherInfoString = weatherServiceRepository.GetWeatherByCity(request);
            var weatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo>(weatherInfoString);

            switch (this.GetOperator())
            {
                case StringConditionOperator.Unknown:
                    return false;
                case StringConditionOperator.Equals:
                    return CompareWithSpecifiedValue(weatherInfo);
                case StringConditionOperator.CaseInsensitivelyEquals:
                    return CompareWithSpecifiedValue(weatherInfo);
                case StringConditionOperator.NotEqual:
                    return false;
                case StringConditionOperator.NotCaseInsensitivelyEquals:
                    return CompareWithSpecifiedValue(weatherInfo);
                case StringConditionOperator.Contains:
                    return false;
                case StringConditionOperator.MatchesRegularExpression:
                    return false;
                case StringConditionOperator.StartsWith:
                    return false;
                case StringConditionOperator.EndsWith:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool CompareWithSpecifiedValue(WeatherInfo weatherInfo)
        {
            return SpecifiedValue.Fields["Name"].Value.ToLowerInvariant()
                        .Equals(weatherInfo.list.FirstOrDefault()?.weather.FirstOrDefault()?.description.ToLowerInvariant());
        }
    }
}