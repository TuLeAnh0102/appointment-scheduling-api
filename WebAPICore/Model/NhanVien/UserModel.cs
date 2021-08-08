using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPICore.Model.NhanVien
{
    public class UserModel
    {
        public UserModel()
        {
            this.username = string.Empty;
            this.ho_ten = string.Empty;
            this.password = string.Empty;
            this.don_vi = string.Empty;
        }

        public int id { get; set; }
        public string username { get; set; }
        public string ho_ten { get; set; }
        public string password { get; set; }
        public string don_vi { get; set; }
        public int role_id { get; set; }

    }
}
