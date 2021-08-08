using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPICore.Model;

namespace WebAPICore.Repository
{
    public class CommonRepository
    {
        public static JToken GetDanhMucTinh()
        {
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                var response = baseSQL.GetList("GET_DANH_MUC_TINH", param);
                return JsonHelper.ToJson(response);
            }
        }

        public static JToken GetDanhMucHuyen(string ma_tinh)
        {
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("P_ma_tinh", ma_tinh);
                var response = baseSQL.GetList("GET_DANH_MUC_HUYEN", param);
                return JsonHelper.ToJson(response);
            }
        }
        public static JToken GetAllDanhMucHuyen()
        {
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                var response = baseSQL.GetList("GET_ALL_DANH_MUC_HUYEN", param);
                return JsonHelper.ToJson(response);
            }
        }

        public static JToken GetAllDanhMucXa()
        {
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                var response = baseSQL.GetList("GET_ALL_DANH_MUC_Xa", param);
                return JsonHelper.ToJson(response);
            }
        }

        public static JToken GetDanhMucXa(string ma_huyen)
        {
            using (var baseSQL = new BaseSQL())
            {
                var param = new SQLDynamicParameters();
                param.Add("P_ma_huyen", ma_huyen);
                var response = baseSQL.GetList("GET_DANH_MUC_XA", param);
                return JsonHelper.ToJson(response);
            }
        }

        public static JToken Return400()
        {
            var errorModel = new { error = "Invalid token !!" };
            return JsonHelper.ToJson(errorModel);
        }
    }
}
