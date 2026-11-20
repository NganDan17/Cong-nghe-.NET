using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhom09_QLyNgoaiNgu
{

    internal class DatabaseConnection
    {
        private static string GetConnectionString()
        {
            var environment = Environment.GetEnvironmentVariable("QL_NGOAINGU_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(environment)) return environment;
            var settings = ConfigurationManager.ConnectionStrings["QL_NGOAINGU"];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("Chưa có cấu hình cơ sở dữ liệu. Chạy KhoiTao.ps1 trước khi mở ứng dụng.");
            return settings.ConnectionString;
        }
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
        public static int ExecuteNonQuery(string sqlQuery, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public static DataTable GetDataTable(string sqlQuery, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(sqlQuery, conn))
                {
                    if (parameters != null)
                    {
                        da.SelectCommand.Parameters.AddRange(parameters);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        public static object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection()) 
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        return cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi thực thi Scalar: " + ex.Message);
                }
            }
        }
    }
}
