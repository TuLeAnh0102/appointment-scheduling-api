using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPICore.Dtos;
using WebAPICore.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using WebAPICore.Model;
using WebAPICore.Model.Response;
using WebAPICore.Repository;
using System.Data;
using WebAPICore.Helpers;
using Microsoft.Extensions.Options;
using WebAPICore.Model.NhanVien;

namespace WebAPICore.Controllers
{
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [HttpGet("api/user/getall")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("api/user/login")]
        public JToken Login(string username, string password)
        {
            var response = new ResponseSingle();
            var userResponse = _userService.Login(username, password);
            if (!userResponse.success)
                response = userResponse;
            else
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, userResponse.data.id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var data = new
                {
                    token = tokenString,
                    id = userResponse.data.ID,
                    is_admin = userResponse.data.IS_ADMIN
                };

                response.success = true;
                response.data = data;
            }
            return JsonHelper.ToJson(response);
        }

        [HttpPost("api/user/create")]
        public JToken CreateUser(UserModel user)
        {
            return _userService.CreateUser(user);
        }

        

        [AllowAnonymous]
        [HttpPost("api/user/update")]
        public JToken CapNhatThongTinNhanVien(UserModel user)
        {
            return _userService.UpdateUser(user);
        }

        

        [AllowAnonymous]
        [HttpPost("api/nhan-vien/reset-pass")]
        public JToken ResetPass(string username, string password, string key)
        {
            return _userService.ResetPass(username,password, key);
        }

        [HttpGet("api/nhan-vien/danh-sach-nhan-vien")]
        public JToken DanhSachNhanVien()
        {
            return _userService.DanhSachNhanVien();
        }

        [HttpGet("api/user/informationUser")]
        public JToken ThongTinNhanVien(int id)
        {
            return _userService.ThongTinNhanVien(id);
        }
    }
}