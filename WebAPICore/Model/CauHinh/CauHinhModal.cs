using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPICore.Model.CauHinh
{
    public class CauHinhModal
    {
        public int id_loai_tai_khoan { get; set; }
        public int id_nhom_quyen { get; set; }
        public string ten_nhom_quyen { get; set; }
    }
}
