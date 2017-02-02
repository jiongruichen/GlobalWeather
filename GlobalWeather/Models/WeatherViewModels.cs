using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobalWeather.Models
{
    public class WeatherViewModels
    {
        [DisplayName("Country")]
        public string CountryName { get; set; }

        public List<SelectListItem> CountryNames { get; set; }

        [DisplayName("City")]
        public string CityName { get; set; }

        public List<SelectListItem> CityNames { get; set; }
    }

    public class WeatherDetailViewModels
    {
        public string Location { get; set; }

        public string Time { get; set; }

        public string Wind { get; set; }

        public string Visibility { get; set; }

        public string SkyConditions { get; set; }

        public string Temperature { get; set; }

        public string DewPoint { get; set; }

        public string RelativeHumidity { get; set; }

        public string Pressure { get; set; }
    }

    public class WeatherMap
    {
        public long id { get; set; }

        public string name { get; set; }

        public int cod { get; set; }

        public int visibility { get; set; }

        public long dt { get; set; }

        public Coordinate coord { get; set; }

        public List<Weather> weather { get; set; }

        public WeatherMain main { get; set; }

        public Wind wind { get; set; }

        public Clouds clouds { get; set; }
    }

    public class Coordinate
    {
        public double lon { get; set; }

        public double lat { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }

        public string main { get; set; }

        public string description { get; set; }

        public string icon { get; set; }
    }

    public class WeatherMain
    {
        public double temp { get; set; }

        public double pressure { get; set; }

        public double humidity { get; set; }

        public double temp_min { get; set; }

        public double temp_max { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }

        public double deg { get; set; }
    }

    public class Clouds
    {
        public double all { get; set; }
    }
}