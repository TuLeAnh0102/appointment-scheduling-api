using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPICore.Model
{
    public class ResponseAPI
    {
        public bool success { get; set; }
        public string data { get; set; }
        public string message { get; set; }
    }
}
