using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Net.Http;

namespace WeatherApp.Model
{
    sealed class WeatherMapManager
    {
        private string PATH = "./Resource/WeatherMapConfig.json";

        private string APIKey;

        private static WeatherMapManager manager;

        private HttpClient cl = new HttpClient();

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

        public async Task<string> GetCityWeather(string CityName, bool FiveDays) 
        {
            await ApplyConfig();
            Uri url;
            url = FiveDays == true ? Uris.FiveDays(CityName, APIKey) : Uris.CurrenTime(CityName, APIKey);
            HttpResponseMessage res = await cl.GetAsync(url);
            string body = await res.Content.ReadAsStringAsync();
            return body;
        }

        private static class Uris 
        { 
            public static Uri CurrenTime(string CityName, string APIKey) 
            {
                Uri url = new Uri($"http://api.openweathermap.org/data/2.5/weather?q={CityName}&appid={APIKey}");
                return url;
            }

            public static Uri FiveDays(string CityName, string APIKey) 
            {
                Uri url = new Uri($"http://api.openweathermap.org/data/2.5/forecast?q={CityName}&appid={APIKey}");
                return url;
            }
        }
    }
}
