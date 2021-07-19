using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class LocationsViewModel : IPageViewModel
    {
        public string PageName => "Manage locations";

        private DBManager db = DBManager.getIstance();

        private ObservableCollection<FavPlaces> _locations;

        public ObservableCollection<FavPlaces> LocationsList 
        {
            get 
            {
                if (_locations == null)
                    _locations = new ObservableCollection<FavPlaces>(db.Locations);

                return _locations;
            }
        }
    }
}
