using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebAPICore.Model;
using WebAPICore.Model.Common;
using WebAPICore.Model.Response;
using WebAPICore.Repository;

namespace WebAPICore.Controllers
{
    [Authorize]
    [ApiController]
    public class CommonController : Controller
    {
        [AllowAnonymous]
        [HttpGet("api/common/danh-muc-tinh")]
        public JToken DanhMucTinh()
        { 
            return CommonRepository.GetDanhMucTinh();
        }
        [AllowAnonymous]
        [HttpGet("api/common/danh-muc-huyen")]
        public JToken DanhMucHuyen(string ma_tinh)
        {
            return CommonRepository.GetDanhMucHuyen(ma_tinh);
        }

        [AllowAnonymous]
        [HttpGet("api/common/all-danh-muc-huyen")]
        public JToken AllDanhMucHuyen()
        {
            return CommonRepository.GetAllDanhMucHuyen();
        }

        [AllowAnonymous]
        [HttpGet("api/common/all-danh-muc-xa")]
        public JToken AllDanhMucXa()
        {
            return CommonRepository.GetAllDanhMucXa();
        }

        [AllowAnonymous]
        [HttpGet("api/common/danh-muc-xa")]
        public JToken DanhMucXa(string ma_huyen)
        {
            return CommonRepository.GetDanhMucXa(ma_huyen);
        }
    }
}
