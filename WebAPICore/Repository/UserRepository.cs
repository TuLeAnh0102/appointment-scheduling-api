using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebAPICore.Model;
using WebAPICore.Model.NhanVien;
using WebAPICore.Model.Response;

namespace WebAPICore.Repository
{
    public class UserRepository
    {
        public static bool HT_CHECK_REFRESH_TOKEN(string refresh_token)
        {
            using (var vpdt = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("RS", dbType: SqlDbType.NVarChar, direction: ParameterDirection.Output);
                param.Add("P_REFRESH_TOKEN", refresh_token);
                var Response = vpdt.GetSingle("HT_CHECK_REFRESH_TOKEN", param);
                return (Response.data != null);
            }
        }

        public static ResponseSingle CB_CHECK_LOGIN(string username)
        {
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("P_USERNAME", username);
                var response = baseSQL.GetSingle("THONG_TIN_LOGIN", param);
                return response;
            }
        }
    }
}
