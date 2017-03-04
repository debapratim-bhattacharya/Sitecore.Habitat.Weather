using System.Collections.Generic;

namespace Sitecore.Foundation.Weather.Models
{
    public class WeatherInfo
    {
        public City city { get; set; }
        public List<Climate> list { get; set; }
    }

    public class City
    {
        public string name { get; set; }
        public string country { get; set; }
    }

    public class Temp
    {
        public double day { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double night { get; set; }
    }

    public class WeatherDesc
    {
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Climate
    {
        public Temp temp { get; set; }
        public int humidity { get; set; }
        public List<WeatherDesc> weather { get; set; }
    }
}