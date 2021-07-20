using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{

    //This interface is implemented by all the ViewModels --> this let them to have some common properties
    //                                                        and keep them in a list
    public interface IPageViewModel
    {
        public string PageName 
        {
            get;
        }
    }
}
