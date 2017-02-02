using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalWeather;
using GlobalWeather.Controllers;
using Moq;
using GlobalWeather.Models;
using Newtonsoft.Json;

namespace GlobalWeather.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private Mock<GlobalWeatherService.GlobalWeatherSoap> client;
        private Mock<IWeatherMapService> weatherMapService;
        private Mock<ICountryService> countryService;
        private HomeController target;

        [TestInitialize]
        public void Init()
        {
            client = new Mock<GlobalWeatherService.GlobalWeatherSoap>();
            weatherMapService = new Mock<IWeatherMapService>();
            countryService = new Mock<ICountryService>();
            target = new HomeController(client.Object, weatherMapService.Object, countryService.Object);
        }

        [TestMethod]
        public void Index()
        {
            var countries = new List<string> { "Australia", "United States" };
            countryService.Setup(s => s.GetCountries()).Returns(countries);

            ViewResult result = target.Index() as ViewResult;
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetCityNames()
        {
            var countryName = "Australia";
            var cities = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><string xmlns=\"http://www.webserviceX.NET\"><NewDataSet><Table><Country>Australia</Country><City>Sydney Airport</City></Table></NewDataSet></string>";
            client.Setup(s => s.GetCitiesByCountry(It.IsAny<string>())).Returns(cities);

            JsonResult result = target.GetCityNames(countryName) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetWeatherDetails()
        {
            var countryName = "Australia";
            var cityName = "Sydney Airport";
            var data = "{\"coord\":{\"lon\":151.21,\"lat\":-33.87},\"weather\":[{\"id\":500,\"main\":\"Rain\",\"description\":\"light rain\",\"icon\":\"10n\"}],\"base\":\"stations\",\"main\":{\"temp\":295.291,\"pressure\":1022.65,\"humidity\":96,\"temp_min\":295.291,\"temp_max\":295.291,\"sea_level\":1028.67,\"grnd_level\":1022.65},\"wind\":{\"speed\":0.82,\"deg\":17.5047},\"rain\":{\"3h\":0.6075},\"clouds\":{\"all\":92},\"dt\":1486073421,\"sys\":{\"message\":0.0111,\"country\":\"AU\",\"sunrise\":1485976708,\"sunset\":1486025936},\"id\":2147714,\"name\":\"Sydney\",\"cod\":200}";

            var weatherDetails = new ServiceResult<WeatherMap>
            {
                Result = JsonConvert.DeserializeObject<WeatherMap>(data),
                Error = "",
                Success = true
            };

            weatherMapService.Setup(s => s.GetWeatherDetails(It.IsAny<string>())).Returns(weatherDetails);

            PartialViewResult result = target.GetWeatherDetails(countryName, cityName) as PartialViewResult;

            Assert.IsNotNull(result);
        }
    }
}
