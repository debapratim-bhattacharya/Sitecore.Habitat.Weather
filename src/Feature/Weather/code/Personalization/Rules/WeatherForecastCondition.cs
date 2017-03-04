using System;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Sitecore.Feature.Weather.Personalization.Rules
{
    public class WeatherForecastCondition<T>
        : OperatorCondition<T> where T : RuleContext
    {
        protected override bool Execute(T ruleContext)
        {
            throw new NotImplementedException();
        }
    }
}