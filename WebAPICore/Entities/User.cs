using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPICore.Entities
{
    public class User
    {
        public int ma_can_bo { get; set; }
        public string ten_can_bo { get; set; }
        public string ten_dang_nhap { get; set; }
        public byte[] password_hash { get; set; }
        public byte[] password_salt { get; set; }
    }
}
