using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPICore.Model.Response
{
    public class ResponseSingle
    {
        private bool _success;
        private string _message;
        private dynamic _data;
        private string _store_name;
        private int _store_type;
        private SQLDynamicParameters _param;

        public bool success { get => _success; set => _success = value; }
        public string message { get => _message; set => _message = value; }
        public dynamic data { get => _data; set => _data = value; }
        public string store_name { get => _store_name; set => _store_name = value; }
        public int store_type { get => _store_type; set => _store_type = value; }
        public SQLDynamicParameters param { get => _param; set => _param = value; }

        public void SetError(string _message)
        {
            this._success = false;
            this._message = _message;
            this._data = null;
        }
    }
    public class ResponseSingleClass<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public string store_name { get; set; }
        public int store_type { get; set; }
        public SQLDynamicParameters param { get; set; }
    }
}
