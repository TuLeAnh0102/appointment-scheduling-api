using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPICore.Entities;
using WebAPICore.Helpers;
using WebAPICore.Model;
using WebAPICore.Model.NhanVien;
using WebAPICore.Model.Response;
using WebAPICore.Repository;

namespace WebAPICore.Services
{
    public interface IUserService
    {
        User GetById(int id);
        ResponseSingle Login(string username, string password);
        IEnumerable<User> GetAll();
        JToken CreateUser(UserModel obj);
        JToken UpdateUser(UserModel obj);
        JToken ResetPass(String username, string password, String key);
        JToken DanhSachNhanVien();
        JToken ThongTinNhanVien(int id);
    }
    public class UserService : IUserService
    {
        public User GetById(int id)
        {
            User user = new User();
            if (id == 1)
            {
                user.ma_can_bo = id;
                user.ten_can_bo = "LastName";
            }
            return user;
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            ResponseSingle response = UserRepository.CB_CHECK_LOGIN(user.ten_dang_nhap);

            if (response.data != null)
                throw new AppException("Cán bộ \"" + user.ten_dang_nhap + "\" đã tồn tại");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.password_hash = passwordHash;
            user.password_salt = passwordSalt;
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            List<User> lstUser = new List<User>();
            User user = new User();
            user.ma_can_bo = 1;
            user.ten_can_bo = "LastName";
            lstUser.Add(user);
            return lstUser;
        }

        public ResponseSingle Login(string username, string password)
        {
            ResponseSingle response = new ResponseSingle();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                response.SetError("Username hoặc password đang để trống");
                return response;
            }

            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("P_USERNAME", username);
                response = baseSQL.GetSingle("THONG_TIN_LOGIN", param);
                if (!response.success || response.data == null)
                {
                    response.SetError("User không tồn tại");
                    return response;
                }
                var user = response.data;
                // check if password is correct
                if (!VerifyPasswordHash(password, user.password_hash, user.password_salt))
                {
                    response.SetError("Password không đúng");
                    return response;
                }

                return response;
            }
        }

        public JToken CreateUser(UserModel obj)
        {
            ResponseExecute response = new ResponseExecute();

            if (string.IsNullOrWhiteSpace(obj.password))
            {
                response.SetError("Password không được để trống");
                return JsonHelper.ToJson(response);
            }

            //check username exist
            var checkLogin = UserRepository.CB_CHECK_LOGIN(obj.username);
            if (!checkLogin.success || checkLogin.data != null)
            {
                response.SetError("Nhân viên \"" + obj.username + "\" đã tồn tại");
                return JsonHelper.ToJson(response);
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(obj.password, out passwordHash, out passwordSalt);

            //insert user
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("P_USERNAME", obj.username);
                param.Add("P_HO_TEN", obj.ho_ten);
                param.Add("P_PASSWORD_HASH", passwordHash);
                param.Add("P_PASSWORD_SALT", passwordSalt);
                param.Add("P_DON_VI", obj.don_vi);
                param.Add("P_PASS_SHOW", obj.password);
                param.Add("P_ROLE_ID", obj.role_id);
                response = baseSQL.Execute("CREATE_USER", param);
                return JsonHelper.ToJson(response);
            }
        }

        public JToken UpdateUser(UserModel obj)
        {
            ResponseExecute response = new ResponseExecute();

            if (string.IsNullOrWhiteSpace(obj.password))
            {
                response.SetError("Password không được để trống");
                return JsonHelper.ToJson(response);
            }

            //check username exist
            // var checkLogin = UserRepository.CB_CHECK_LOGIN(obj.username);

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(obj.password, out passwordHash, out passwordSalt);
            //if (!checkLogin.success || checkLogin.data != null)
            //{
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("P_ID", obj.id);
                param.Add("P_USERNAME", obj.username);
                param.Add("P_HO_TEN", obj.ho_ten);
                param.Add("P_PASSWORD_HASH", passwordHash);
                param.Add("P_PASSWORD_SALT", passwordSalt);
                param.Add("P_DON_VI", obj.don_vi);
                param.Add("P_PASS_SHOW", obj.password);
                param.Add("P_ROLE_ID", obj.role_id);
                response = baseSQL.Execute("UPDATE_USER", param);
                return JsonHelper.ToJson(response);
            }
        }

        public JToken ResetPass(String username, String password, String key)
        {
            ResponseExecute response = new ResponseExecute();

            if (string.IsNullOrWhiteSpace(password))
            {
                response.SetError("Password không được để trống");
                return JsonHelper.ToJson(response);
            }

            if (key != "Admin12345!@#")
            {
                response.SetError("Key sai");
                return JsonHelper.ToJson(response);
            }

            //check username exist
            var checkLogin = UserRepository.CB_CHECK_LOGIN(username);
            if (!checkLogin.success && checkLogin.data == null)
            {
                response.SetError("Nhân viên \"" + username + "\" không tồn tại");
                return JsonHelper.ToJson(response);
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //insert user
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("P_USERNAME", username);
                param.Add("P_PASSWORD_SHOW", password);
                param.Add("P_PASSWORD_HASH", passwordHash);
                param.Add("P_PASSWORD_SALT", passwordSalt);

                response = baseSQL.Execute("RESET_PASS", param);
                return JsonHelper.ToJson(response);
            }
        }

        public JToken DanhSachNhanVien()
        {
            using (var baseSQl = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                var response = baseSQl.GetList("DANH_SACH_USER", param);
                return JsonHelper.ToJson(response);
            }
        }

        public JToken ThongTinNhanVien(int id)
        {
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("P_ID", id);
                var response = baseSQL.GetSingle("INFORMATION_USER", param);
                return JsonHelper.ToJson(response);
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
