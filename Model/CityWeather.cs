using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class CityWeather: INotifyPropertyChanged
    {
        #region fields

        private string _dt;
        private double _tempMin;
        private double _tempMax;
        private double _humidity;
        private string _weather;
        private double _windSpeed;
        private string _icon;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region properties

        public string Dt 
        { 
            get => _dt;
            set { _dt = value; OnPropertyChanged("Dt"); }
        }
        public double TempMin 
        { 
            get => _tempMin;
            set { _tempMin = Math.Round(value - 273.15, 2, MidpointRounding.AwayFromZero); Debug.WriteLine(value);  OnPropertyChanged("TempMin"); }
        }
        public double TempMax 
        { 
            get => _tempMax;
            set { _tempMax = Math.Round(value - 273.15, 2, MidpointRounding.AwayFromZero); OnPropertyChanged("TempMax"); }
        }
        public double Humidity
        { 
            get => _humidity;
            set { _humidity = value; OnPropertyChanged("Humidity"); }
        }
        public string Weather
        { 
            get => _weather;
            set { _weather = value; OnPropertyChanged("Weather"); }
        }
        public double WindSpeed 
        { 
            get => _windSpeed;
            set { _windSpeed = value; OnPropertyChanged("WindSpeed"); }
        }

        public string Icon 
        {
            get => _icon;
            set { _icon = $"http://openweathermap.org/img/w/{value}.png"; OnPropertyChanged("Icon"); }
        }

        #endregion

        #region methods

        public void OnPropertyChanged(string propertyName) 
        {
            PropertyChangedEventHandler copy = PropertyChanged;
            if (copy != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
