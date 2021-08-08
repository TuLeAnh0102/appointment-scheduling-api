using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebAPICore.Repository;
using Microsoft.AspNetCore.Authorization;
using WebAPICore.Model.CauHinh;

namespace WebAPICore.Controllers
{
    [Authorize]
    [ApiController]
    public class CauHinhHeThongController : ControllerBase
    {
        // to khai van tai
        [HttpGet("api/cau-hinh/get-loai-tai-khoan")]
        public JToken GetLoaiTaiKhoan()
        {
            return CauHinhHeThongRepository.getLoaiTaiKhoan();
        }
        [HttpPost("api/cau-hinh/them-them-nhom-quyen")]
        public JToken ThemNhomQuyen(CauHinhModal obj)
        {
            return CauHinhHeThongRepository.modifyNhomQuyen(obj);
        }

    }
}
