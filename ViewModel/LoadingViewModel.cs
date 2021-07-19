using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class LoadingViewModel : INotifyPropertyChanged
    {
        private DBManager db = DBManager.getIstance();
        private WeatherMapManager wm = WeatherMapManager.getInstance();
        private string _loadingString;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> ErrorHandler;

        public string LoadingString
        {
            get { return _loadingString; }
            set
            {
                _loadingString = value;
                OnPropertyChanged("LoadingString");
            }
        }

        public LoadingViewModel()
        {
            Setup();
        }

        public async void Setup()
        {
            LoadingString = "Connectiong to DB and fetching favourite cities ...";
            await db.CreateDB();
            await db.DBConnect();
            await db.LoadFavPlaces();

            LoadingString = "Fetching weather of favourite cities ...";

            foreach (FavPlaces fp in db.Locations)
            {
                Status res = await wm.GetCityWeather(fp.Name);
                if (res.code != System.Net.HttpStatusCode.OK)
                    OnErrorOccurred(res.message);
            }

            LoadingString = "Done";
        }

        public void OnPropertyChanged(string propName)
        {
            PropertyChangedEventHandler copy = PropertyChanged;
            if (copy != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public void OnErrorOccurred(string message)
        {
            EventHandler<string> copy = ErrorHandler;
            if (copy != null)
                ErrorHandler(this, message);
        }
    }
}
