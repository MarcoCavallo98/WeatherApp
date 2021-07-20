using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WeatherApp
{
    class LocationsViewModel : IPageViewModel
    {
        #region fields
        public string PageName => "Manage locations"; //Title of page

        private DBManager db = DBManager.getIstance();

        private ObservableCollection<FavPlaces> _locations; //Collection of all locations displayed

        private ICommand _removeLocation;
        private ICommand _addLocation;

        #endregion

        #region properties

        public ObservableCollection<FavPlaces> LocationsList 
        {
            get 
            {
                if (_locations == null)
                    _locations = new ObservableCollection<FavPlaces>(db.Locations);

                return _locations;
            }
        }

        public ICommand RemoveLocation 
        {
            get
            {
                if (_removeLocation == null)
                    _removeLocation = new DeleteCommand();

                return _removeLocation;
            }

            private set => _removeLocation = value;
        }

        public ICommand AddLocation
        {
            get
            {
                if (_addLocation == null)
                    _addLocation = new AddCommand();

                return _addLocation;
            }

            private set => _addLocation = value;
        }

        #endregion

        #region methods

        public LocationsViewModel() 
        {
            //Registration to the events
            db.LocationRemoved += HandleLocationRemoved;
            db.LocationAdded += HandleLocationAdded;
        }

        //Event handlers
        private void HandleLocationRemoved(object sender, FavPlaces args) 
        { 
            if(args != null) 
            {
                LocationsList.Remove(args);
            }
        }

        private void HandleLocationAdded(object sender, FavPlaces args)
        {
            if (args != null)
            {
                LocationsList.Add(args);
            }
        }
        //End event handlers
        #endregion

        //COMMANDS
        private class DeleteCommand : ICommand
        {
            //This command allow the user to remove a location from the SQLite DB 

            public event EventHandler CanExecuteChanged;
            private DBManager manager = DBManager.getIstance();

            public bool CanExecute(object parameter)
            {
                return true; //Control will be done during Execute
            }

            public async void Execute(object parameter)
            {
                if (manager.Locations.Count > 1)
                {
                    FavPlaces place = (FavPlaces)parameter;
                    WeatherMapManager.getInstance().CitiesWeather.Remove(place.Name); 
                    await manager.RemoveFavPlaces(place); //This emit the LocationRemoved event
                }
            }
        }

        private class AddCommand : ICommand
        {
            //This command allow the user to add a location to the SQLite DB

            public event EventHandler CanExecuteChanged;
            private DBManager manager = DBManager.getIstance();

            public bool CanExecute(object parameter)
            {
                return true; 
            }

            public async void Execute(object parameter)
            {
                string place = (string)parameter;
                if (place != null && place.Length > 0)
                {
                    try
                    {
                        Status res = await WeatherMapManager.getInstance().GetCityWeather(place);
                        switch (res.code)
                        {
                            case System.Net.HttpStatusCode.OK:
                                await manager.AddFavPlaces(place); //This emit the LocationAdded event
                                break;
                            case System.Net.HttpStatusCode.NotFound:
                                MessageBox.Show("Unknown city", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            case System.Net.HttpStatusCode.InternalServerError:
                                MessageBox.Show("Service not available.\nPlease, try later!",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            default:
                                MessageBox.Show("Something went wrong.\nPlease, try later and check you connection!",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                        }
                    }
                    catch (Exception) 
                    {
                        MessageBox.Show("Please, check you connection!",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                }
            }
        }
    }
}
