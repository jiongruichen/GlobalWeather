using GlobalWeather.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GlobalWeather
{
    public interface IWeatherMapService
    {
        ServiceResult<WeatherMap> GetWeatherDetails(string cityName);
    }

    public class WeatherMapService : WeatherMapServiceBase, IWeatherMapService
    {
        public ServiceResult<WeatherMap> GetWeatherDetails(string cityName)
        {
            var appKey = ConfigurationManager.AppSettings["OpenWeatherMapKey"];
            var results = this.ExecuteOperation<WeatherMap>(c => c.GetAsync("weather?appid=" + appKey + "&q=" + cityName));

            return results;
        }
    }

    public class WeatherMapServiceBase : IDisposable
    {
        private HttpClient client;

        public WeatherMapServiceBase()
        {
            var uri = ConfigurationManager.AppSettings["OpenWeatherMapURI"];

            client = new HttpClient();

            client.BaseAddress = new Uri(uri, UriKind.Absolute);
        }

        public ServiceResult<TResult> ExecuteOperation<TResult>(Func<HttpClient, Task<HttpResponseMessage>> operation)
        {
            var ret = new ServiceResult<TResult> { Success = false, Error = "Unable to proceed details" };

            try
            {
                var response = operation(this.client).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    ret.Error = "";
                    ret.Result = JsonConvert.DeserializeObject<TResult>(result);
                    ret.Success = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }

        public void Dispose()
        {
            if (this.client != null)
            {
                client.Dispose();
            }
        }
    }

    public class ServiceResult<T>
    {
        public ServiceResult()
        {
            Error = "";
        }

        public virtual bool Success { get; set; }

        public string Error { get; set; }

        public virtual T Result { get; set; }

        public string Key { get; set; }

        public static ServiceResult<T> AsSuccess(T result = default(T))
        {
            return new ServiceResult<T>() { Success = true, Result = result };
        }

        public static ServiceResult<T> AsError(string error = null)
        {
            return new ServiceResult<T>() { Success = false, Result = default(T), Error = error };
        }

        public static ServiceResult<T> AsError(T result, string error = null)
        {
            return new ServiceResult<T>() { Success = false, Result = result, Error = error };
        }

        public static ServiceResult<T> CopyFrom<Y>(ServiceResult<Y> other)
        {
            var copy = new ServiceResult<T>() { Success = other.Success, Error = other.Error };
            if (other.Result is T)
                copy.Result = (T)(object)other.Result;

            return copy;
        }

        public static ServiceResult<T> CopyFrom<Y>(ServiceResult<Y> other, T result)
        {
            var obj = CopyFrom(other);
            obj.Result = result;
            return obj;
        }
    }
}