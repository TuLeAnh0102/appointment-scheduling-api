using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPICore.Model.Response
{
    public class ResponseExecute
    {
        private int _id;
        private string _message;
        private string _store_name;
        private int _store_type;
        private bool _success;
        private SQLDynamicParameters _param;

        public int id { get => _id; set => _id = value; }
        public string message { get => _message; set => _message = value; }
        public string store_name { get => _store_name; set => _store_name = value; }
        public int store_type { get => _store_type; set => _store_type = value; }
        public SQLDynamicParameters param { get => _param; set => _param = value; }
        public bool success { get => _success; set => _success = value; }

        public void SetError(string message)
        {
            this._id = 0;
            this._message = message;
        }
    }
}
