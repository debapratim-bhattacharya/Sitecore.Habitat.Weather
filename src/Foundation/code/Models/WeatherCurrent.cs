using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.Weather.Models
{
    public class WeatherCurrent
    {
        public WeatherDesc weather { get; set; }
        public main main { get; set; }
        public string name { get; set; }

    }
    public class main
    {
        public double temp { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double humidity { get; set; }
        public double pressure { get; set; }
    }
}