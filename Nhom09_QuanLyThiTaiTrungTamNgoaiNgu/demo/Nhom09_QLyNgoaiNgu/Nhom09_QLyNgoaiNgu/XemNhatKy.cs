using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nhom09_QLyNgoaiNgu;
using Microsoft.Data.SqlClient;
using Microsoft.Data;
namespace Nhom09_QLyNgoaiNgu
{
    internal class XemNhatKy
    {
        public static int CurrentUserId = 0;

        public static void Record(string hanhDong)
        {
            if (CurrentUserId == 0) return; 

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO NhatKyHoatDong (MaTaiKhoan, ThoiGian, HanhDong) VALUES (@MaTK, GETDATE(), @HanhDong)";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@MaTK", CurrentUserId);
                    cmd.Parameters.AddWithValue("@HanhDong", hanhDong);

                    cmd.ExecuteNonQuery();
                }
                catch
                {
                }
            }
        }
    }
}
