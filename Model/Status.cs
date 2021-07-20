using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    struct Status
    {
        //Struct for HTTP code and message response

        public System.Net.HttpStatusCode code;
        public string message;

        public Status(System.Net.HttpStatusCode code, string message) 
        {
            this.code = code;
            this.message = message;
        }
    }
}
