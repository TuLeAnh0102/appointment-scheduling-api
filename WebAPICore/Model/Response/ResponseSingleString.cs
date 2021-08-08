using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPICore.Model.Response
{
    public class ResponseSingleString
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string data { get; set; }
        public string store_name { get; set; }
        public int store_type { get; set; }
        public SQLDynamicParameters param { get; set; }
    }
}
