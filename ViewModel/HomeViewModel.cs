using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class HomeViewModel: IPageViewModel, INotifyPropertyChanged
    {
        #region fields

        private string _name;
       
        private DBManager db = DBManager.getIstance();
        private WeatherMapManager wm = WeatherMapManager.getInstance();

        private int WeatherPositionCounter;
        private bool _canIncrementWeatherPositionCounter;
        private bool _canDecrementWeatherPositionCounter;

        private ObservableCollection<FavPlaces> _locations;
        private FavPlaces _currentLocation;
        private CityWeather _currentWeather;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region properties
        public string PageName
        {
            get
            {
                if (_name == null)
                    _name = "Home";

                return _name;
            }
        }

        public bool CanIncrementWeatherPositionCounter
        {
            get { return _canIncrementWeatherPositionCounter; }
            set
            {
                _canIncrementWeatherPositionCounter = value;
                OnPropertyChanged("CanIncrementWeatherPositionCounter");
            }
        }
        public bool CanDecrementWeatherPositionCounter
        {
            get { return _canDecrementWeatherPositionCounter; }
            set
            {
                _canDecrementWeatherPositionCounter = value;
                OnPropertyChanged("CanDecrementWeatherPositionCounter");
            }
        }
        public ObservableCollection<FavPlaces> LocationsList
        {
            get
            {
                if (_locations == null)
                    _locations = new ObservableCollection<FavPlaces>(db.Locations);

                return _locations;
            }
        }

        public FavPlaces CurrentLocation 
        {
            get => _currentLocation;
            set 
            {
                _currentLocation = value;
                WeatherPositionCounter = 0;
                CanIncrementWeatherPositionCounter = wm.CitiesWeather[CurrentLocation.Name].Count > WeatherPositionCounter+1;
                CanDecrementWeatherPositionCounter = WeatherPositionCounter > 0;
                CurrentWeather = wm.CitiesWeather[value.Name][WeatherPositionCounter];
                OnPropertyChanged("CurrentLocation");
            }
        }

        public CityWeather CurrentWeather
        {
            get => _currentWeather;
            set
            {
                _currentWeather = value;
                OnPropertyChanged("CurrentWeather");
            }
        }

        #endregion

        #region methods

        public HomeViewModel() 
        {
            db.LocationRemoved += HandleLocationRemoved;
            db.LocationAdded += HandleLocationAdded;
            WeatherPositionCounter = 0;
            CurrentLocation = LocationsList[0];
        }

        public void IncrementWeatherPositionCounter() 
        {
            WeatherPositionCounter += 1;
            CanIncrementWeatherPositionCounter = wm.CitiesWeather[CurrentLocation.Name].Count > WeatherPositionCounter+1;
            CanDecrementWeatherPositionCounter = WeatherPositionCounter > 0;
            CurrentWeather = wm.CitiesWeather[CurrentLocation.Name][WeatherPositionCounter];
        }

        public void DecrementWeatherPositionCounter()
        {
            WeatherPositionCounter -= 1;
            CanIncrementWeatherPositionCounter = wm.CitiesWeather[CurrentLocation.Name].Count > WeatherPositionCounter+1;
            CanDecrementWeatherPositionCounter = WeatherPositionCounter > 0;
            CurrentWeather = wm.CitiesWeather[CurrentLocation.Name][WeatherPositionCounter];
        }

        public void OnPropertyChanged(string propertyName) 
        {
            PropertyChangedEventHandler copy = PropertyChanged;
            if (copy != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleLocationRemoved(object sender, FavPlaces args) 
        {
            if (args != null) 
            {
                LocationsList.Remove(args);
                CurrentLocation = LocationsList[0];
            }
        }

        private void HandleLocationAdded(object sender, FavPlaces args)
        {
            if (args != null)
            {
                LocationsList.Add(args);
            }
        }

        #endregion
    }
}
