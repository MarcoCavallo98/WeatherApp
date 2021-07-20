using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Net.Http;
using System.Diagnostics;
using System.Globalization;

namespace WeatherApp
{
    sealed class WeatherMapManager
    {
        #region fields

        private string PATH = "./Resource/WeatherMapConfig.json";

        private string APIKey;

        private static WeatherMapManager manager;

        private HttpClient cl = new HttpClient();

        private Dictionary<string, List<CityWeather>> _citiesWeather;

        #endregion

        #region properties
        public Dictionary<string, List<CityWeather>> CitiesWeather 
        {
            get 
            {
                if (_citiesWeather == null)
                    _citiesWeather = new Dictionary<string, List<CityWeather>>();

                return _citiesWeather;
            }
        }

        #endregion

        #region methods

        private WeatherMapManager()
        {
        }

        private async Task ApplyConfig() 
        {
            if (APIKey == null)
            {
                FileStream fs = File.OpenRead(PATH);
                JsonElement data = await JsonSerializer.DeserializeAsync<JsonElement>(fs);
                APIKey = data.GetProperty("APIKey").ToString().Trim();
            }
        }

        public static WeatherMapManager getInstance() 
        {
            if (manager == null)
                manager = new WeatherMapManager();

            return manager;
        }

        public async Task<Status> GetCityWeather(string CityName) 
        {
            if(APIKey == null)
                await ApplyConfig();
            try
            {
                Uri url = new Uri($"http://api.openweathermap.org/data/2.5/forecast?q={CityName}&appid={APIKey}");
                HttpResponseMessage res = await cl.GetAsync(url);



                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                    return new Status(res.StatusCode, "An error has occurred.\nPlease try again.");

                JsonElement data = await JsonSerializer.DeserializeAsync<JsonElement>(res.Content.ReadAsStream());
                JsonElement list = data.GetProperty("list");
                List<CityWeather> cwList = CityWeatherCreator(list);

                CitiesWeather.Add(CityName, cwList);

                return new Status(res.StatusCode, res.ReasonPhrase);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<CityWeather> CityWeatherCreator(JsonElement list) 
        {
            List<CityWeather> cwList = new List<CityWeather>();
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            for (int i = 0; i < list.GetArrayLength(); i++)
            {
                string dt = list[i].GetProperty("dt_txt").ToString();
                double tempMin = Convert.ToDouble(list[i].GetProperty("main").GetProperty("temp_min").ToString(), provider);
                double tempMax = Convert.ToDouble(list[i].GetProperty("main").GetProperty("temp_max").ToString(), provider);
                double humidity = Convert.ToDouble(list[i].GetProperty("main").GetProperty("humidity").ToString(), provider);
                string weather = list[i].GetProperty("weather")[0].GetProperty("description").ToString();
                double windSpeed = Convert.ToDouble(list[i].GetProperty("wind").GetProperty("speed").ToString(), provider);
                string icon = list[i].GetProperty("weather")[0].GetProperty("icon").ToString();
                CityWeather cw = new CityWeather
                {
                    Dt = dt,
                    TempMin = tempMin,
                    TempMax = tempMax,
                    Humidity = humidity,
                    Weather = weather,
                    WindSpeed = windSpeed,
                    Icon = icon
                };

                cwList.Add(cw);
            }

            return cwList;
        }

        #endregion
    }
}
