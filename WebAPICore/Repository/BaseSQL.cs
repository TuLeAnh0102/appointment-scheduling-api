using Dapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPICore.Model;
using WebAPICore.Model.Response;

namespace WebAPICore.Repository
{
    public class BaseSQL : IDisposable
    {
        private string strConnect { get; set; }

        public BaseSQL()
        {
            strConnect = Startup.ConnectString;
        }
        public void Dispose()
        {
        }

        /// <summary>
        /// Hàm trả về một danh sách gồm nhiều record
        /// </summary>
        /// <param name="store"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ResponseList GetList(string store, SQLDynamicParameters param)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(strConnect))
            {

                var Response = new ResponseList();
                Response.store_name = store;
                Response.store_type = 2;
                try
                {
                    var data = db.Query<dynamic>(store, param, commandType: CommandType.StoredProcedure);

                    Response.data = data;
                    if (data.Count() == 0)
                    {
                        Response.message = "Không tìm thấy dữ liệu";
                    }
                    else
                    {
                        Response.message = "Lấy dữ liệu thành công";
                    }

                    Response.success = true;
                }
                catch (Exception ex)
                {
                    Response.data = new List<dynamic>();
                    Response.message = ex.Message;
                    Response.success = false;
                }
                finally
                {
                    Response.param = param;
                    db.Close();
                }
                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        public ResponseList GetListOtherDB(string store, SQLDynamicParameters param, string connectDB)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(connectDB))
            {

                var Response = new ResponseList();
                Response.store_name = store;
                Response.store_type = 2;
                try
                {
                    var data = db.Query<dynamic>(store, param, commandType: CommandType.StoredProcedure);

                    Response.data = data;
                    if (data.Count() == 0)
                    {
                        Response.message = "Không tìm thấy dữ liệu";
                    }
                    else
                    {
                        Response.message = "Lấy dữ liệu thành công";
                    }

                    Response.success = true;
                }
                catch (Exception ex)
                {
                    Response.data = new List<dynamic>();
                    Response.message = ex.Message;
                    Response.success = false;
                }
                finally
                {
                    Response.param = param;
                    db.Close();
                }
                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        public ResponseList GetList(string store, object obj)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(strConnect))
            {
                var Response = new ResponseList();
                Response.store_name = store;
                Response.store_type = 2;
                var param = new SQLDynamicParameters();
                try
                {
                    param.Add("RS", dbType: SqlDbType.NVarChar, direction: ParameterDirection.Output);
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        string key = "P_" + prop.Name.ToUpper();
                        if (key.IndexOf("_TEMP") == -1)
                        {
                            var value = prop.GetValue(obj);
                            if (key.IndexOf("_OUT") != -1)
                                param.Add(key, value, direction: ParameterDirection.Output);
                            else if (key.IndexOf("_INOUT") != -1)
                                param.Add(key, value, direction: ParameterDirection.InputOutput);
                            else param.Add(key, value);
                        }
                    }

                    var data = db.Query<dynamic>(store, param, commandType: CommandType.StoredProcedure);
                    Response.data = data;
                    if (data.Count() == 0)
                    {
                        Response.message = "Không tìm thấy dữ liệu";
                    }
                    else
                    {
                        Response.message = "Lấy dữ liệu thành công";
                    }

                    Response.success = true;
                }
                catch (Exception ex)
                {
                    Response.data = new List<dynamic>();
                    Response.message = ex.Message;
                    Response.success = false;
                }
                finally
                {
                    Response.param = param;
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        public ResponsePageList GetPageList(string store, object obj)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(strConnect))
            {
                var Response = new ResponsePageList();
                Response.store_name = store;
                Response.store_type = 2;
                var param = new SQLDynamicParameters();
                try
                {
                    param.Add("RS", dbType: SqlDbType.NVarChar, direction: ParameterDirection.Output);
                    param.Add("P_TOTAL_ROW", 0, direction: ParameterDirection.Output);
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        string key = "P_" + prop.Name.ToUpper();
                        if (key.IndexOf("_TEMP") == -1)
                        {
                            var value = prop.GetValue(obj);
                            if (key.IndexOf("_OUT") != -1)
                                param.Add(key, value, direction: ParameterDirection.Output);
                            else if (key.IndexOf("_INOUT") != -1)
                                param.Add(key, value, direction: ParameterDirection.InputOutput);
                            else param.Add(key, value);
                        }
                    }

                    Response.data = db.Query<dynamic>(store, param, commandType: CommandType.StoredProcedure);
                    Response.success = true;

                    int total_row = param.Get<int>("P_TOTAL_ROW");
                    int size = param.Get<int>("P_SIZE");
                    int total_page = total_row / size;
                    total_page = (total_row % size == 0) ? total_page : (total_page + 1);
                    Response.total_page = total_page;
                    Response.total_row = total_row;

                    if (Response.data.Count() > 0)
                    {
                        Response.message = "Lấy dữ liệu thành công";
                    }
                    else
                    {
                        Response.message = "Không tìm thấy dữ liệu";
                    }
                }
                catch (Exception ex)
                {
                    Response.data = new List<dynamic>();
                    Response.message = ex.Message;
                    Response.success = false;
                }
                finally
                {
                    Response.param = param;
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        /// <summary>
        /// Hàm trả về một record
        /// </summary>
        /// <param name="store"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ResponseSingle GetSingle(string store, SQLDynamicParameters param)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(strConnect))
            {
                var Response = new ResponseSingle();
                Response.store_name = store;
                Response.store_type = 1;
                try
                {
                    var data = db.Query<dynamic>(store, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    Response.data = data;
                    Response.message = "Lấy dữ liệu thành công";
                    Response.success = true;
                }
                catch (Exception ex)
                {
                    Response.data = null;
                    Response.message = ex.Message;
                    Response.success = false;
                }
                finally
                {
                    Response.param = param;
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        public ResponseSingle GetSingle(string store, object obj)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(strConnect))
            {
                var param = new SQLDynamicParameters();
                var Response = new ResponseSingle();
                Response.store_name = store;
                Response.store_type = 1;
                try
                {
                    param.Add("RS", dbType: SqlDbType.NVarChar, direction: ParameterDirection.Output);
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        string key = "P_" + prop.Name.ToUpper();
                        if (key.IndexOf("_TEMP") == -1)
                        {
                            var value = prop.GetValue(obj);
                            if (key.IndexOf("_OUT") != -1)
                                param.Add(key, value, direction: ParameterDirection.Output);
                            else if (key.IndexOf("_INOUT") != -1)
                                param.Add(key, value, direction: ParameterDirection.InputOutput);
                            else param.Add(key, value);
                        }
                    }

                    var data = db.Query<dynamic>(store, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    Response.data = data;
                    Response.message = "Lấy dữ liệu thành công";
                    Response.success = true;
                }
                catch (Exception ex)
                {
                    Response.data = null;
                    Response.message = ex.Message;
                    Response.success = false;
                }
                finally
                {
                    Response.param = param;
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        public ResponseSingleClass<T> GetSingleClass<T>(string store, SQLDynamicParameters param)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(strConnect))
            {
                var Response = new ResponseSingleClass<T>();
                Response.store_name = store;
                Response.store_type = 1;
                try
                {
                    var data = db.Query<T>(store, param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    Response.data = data;
                    Response.message = "Lấy dữ liệu thành công";
                    Response.success = true;
                }
                catch (Exception ex)
                {
                    Response.data = default(T);
                    Response.message = ex.Message;
                    Response.success = false;
                }
                finally
                {
                    Response.param = param;
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        /// <summary>
        /// Dùng chung để thực thi các store không xác định
        /// </summary>
        /// <param name="store"></param>
        /// <param name="param"></param>
        public ResponseExecute Execute(string store, SQLDynamicParameters param)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(strConnect))
            {
                var Response = new ResponseExecute();
                Response.store_name = store;
                Response.store_type = 0;
                try
                {
                    db.Execute(store, param, commandType: CommandType.StoredProcedure);
                    Response.id = 1;
                    Response.message = "Thực thi thành công";
                    Response.success = true;
                }
                catch (Exception ex)
                {
                    Response.id = -1;
                    Response.success = false;
                    Response.message = ex.Message;
                }
                finally
                {
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        public ResponseExecute Execute(string store, SQLDynamicParameters param, string connectDB)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(connectDB))
            {
                var Response = new ResponseExecute();
                Response.store_name = store;
                Response.store_type = 0;
                try
                {
                    db.Execute(store, param, commandType: CommandType.StoredProcedure);
                    Response.id = 1;
                    Response.message = "Thực thi thành công";
                    Response.success = true;
                }
                catch (Exception ex)
                {
                    Response.id = -1;
                    Response.success = true;
                    Response.message = ex.Message;
                }
                finally
                {
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        /// <summary>
        /// Hàm dùng để insert, update cho một đối tượng
        /// </summary>
        /// <param name="store"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseExecute Execute(string store, object obj)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = store;
            using (var db = new SqlConnection(strConnect))
            {
                var Response = new ResponseExecute();
                Response.store_name = store;
                Response.store_type = 0;
                var param = new SQLDynamicParameters();
                try
                {
                    JObject jobj = JObject.FromObject(obj);
                    foreach (var prop in jobj)
                    {
                        var key = prop.Key;
                        var value = prop.Value.ToString();
                        if ((key.IndexOf("p_") == -1))
                        {
                            param.Add("p_" + key, value);
                        }
                        else
                        {
                            param.Add(key, value);
                        }
                    }

                    db.Execute(store, param, commandType: CommandType.StoredProcedure);

                    Response.id = 1;
                    Response.success = true;
                    Response.message = "Thực thi thành công";
                }
                catch (Exception ex)
                {
                    Response.id = -1;
                    Response.success = false;
                    Response.message = ex.Message;
                }
                finally
                {
                    Response.param = param;
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }

        private void WriteLog(LOG log)
        {
            if (!String.IsNullOrEmpty(Startup.PathLog))
            {
                var path2Folder = Startup.PathLog + "\\";
                var now = DateTime.Now;
                path2Folder += "\\" + now.Year.ToString() + "\\" + now.Month.ToString();
                bool folderExists = Directory.Exists(path2Folder);
                if (!folderExists)
                    Directory.CreateDirectory(path2Folder);

                var filename = now.ToString("dd-MM-yyyy") + ".txt";
                var path2File = path2Folder + "\\" + filename;
                if (!System.IO.File.Exists(path2File))
                    System.IO.File.Create(path2File).Dispose();

                string db_connect = "db_connect: " + log.db_connect;
                string db_response = "db_response: " + log.db_response;
                string store_name = "store_name: " + log.store;

                using (FileStream fs = new FileStream(path2File, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write("\r\n");
                    writer.Write(store_name + "\r\n");
                    writer.Write(db_connect + "\r\n");
                    writer.Write(db_response + "\r\n");
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        private string GetNow()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public ResponseExecute ExecuteSQL(string sql, SQLDynamicParameters param)
        {
            var log = new LOG();
            log.db_connect = GetNow();
            log.store = sql;
            using (var db = new SqlConnection(strConnect))
            {
                var Response = new ResponseExecute();
                Response.store_name = sql;
                Response.store_type = 0;
                try
                {
                    db.Execute(sql, param, commandType: CommandType.Text);
                    Response.id = 1;
                    Response.message = "Thực thi thành công";
                }
                catch (Exception ex)
                {
                    Response.id = -1;
                    Response.message = ex.Message;
                }
                finally
                {
                    db.Close();
                }

                log.db_response = GetNow();
                WriteLog(log);
                return Response;
            }
        }
    }
}
