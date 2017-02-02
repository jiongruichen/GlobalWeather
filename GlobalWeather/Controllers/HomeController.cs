using GlobalWeather.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace GlobalWeather.Controllers
{
    public class HomeController : Controller
    {
        //private GlobalWeatherService.GlobalWeatherSoapClient _client;
        private GlobalWeatherService.GlobalWeatherSoap _client;
        private IWeatherMapService _weatherMapService;
        private ICountryService _countryService;

        public HomeController(GlobalWeatherService.GlobalWeatherSoap client, IWeatherMapService weatherMapService, ICountryService countryService)
        {
            _client = client;
            _weatherMapService = weatherMapService;
            _countryService = countryService;
        }

        public ActionResult Index()
        {
            var viewModel = new WeatherViewModels
            {
                CountryNames = GetCountries(),
                CityNames = new List<SelectListItem>()
            };

            return View(viewModel);
        }

        public JsonResult GetCityNames(string countryName)
        {
            var ret = new List<string>();
            
            var cities = _client.GetCitiesByCountry(countryName);

            ret = GetCities(cities);
            
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetWeatherDetails(string countryName, string cityName)
        {
            var weatherMap = _weatherMapService.GetWeatherDetails(cityName).Result;

            var viewModel = new WeatherDetailViewModels();

            if (weatherMap != null)
            {
                viewModel.Location = string.Format("Lon: {0}, Lat: {1}", weatherMap.coord.lon, weatherMap.coord.lat);
                viewModel.Time = UnixTimeStampToDateTime(weatherMap.dt).ToString("dd/MM/yyyy HH:mm:ss");
                viewModel.Wind = string.Format("Speed: {0} metre/sec, Degree: {1}", weatherMap.wind.speed.ToString(), weatherMap.wind.deg.ToString());
                viewModel.Visibility = weatherMap.visibility.ToString();
                viewModel.SkyConditions = weatherMap.weather[0].description;
                viewModel.Temperature = ConvertFromKelvin(weatherMap.main.temp).ToString() + "°C";
                viewModel.RelativeHumidity = (weatherMap.main.humidity / 100).ToString("P");
                viewModel.Pressure = weatherMap.main.pressure.ToString() + " hPa";
            }

            return PartialView("_WeatherDetails", viewModel);
        }
        
        private List<SelectListItem> GetCountries()
        {
            var ret = new List<SelectListItem>();
            var countries = _countryService.GetCountries();

            foreach(var country in countries)
            {
                ret.Add(new SelectListItem { Text = country, Value = country });
            }

            return ret;
        }

        private List<string> GetCities(string xml)
        {
            var ret = new List<string>();
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode node in doc.SelectNodes("//City"))
            {
                ret.Add(node.InnerText);
            }

            return ret;
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();

            return dtDateTime;
        }

        private double ConvertFromKelvin(double kel)
        {
            return kel - 273;
        }
    }
}