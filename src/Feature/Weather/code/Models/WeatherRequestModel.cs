using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.Weather.Models
{
    public class WeatherRequestModel
    {
        public string location { get; set; }
        public string AppId { get; set; }
        public string BaseURL { get; set; }
        public string RelativeURL { get; set; }
        public string Unit { get; set; }
    }
}