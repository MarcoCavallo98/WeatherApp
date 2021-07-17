using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class HomeViewModel: IPageViewModel
    {
        private string _name;
        public string PageName
        {
            get
            {
                if (_name == null)
                    _name = "Home";

                return _name;
            }
        }
    }
}
