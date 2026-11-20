using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Nhom09_QLyNgoaiNgu;
namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmNhatKyHoatDong : Form
    {
        public frmNhatKyHoatDong()
        {
            InitializeComponent();
        }

        private void frmNhatKyHoatDong_Load(object sender, EventArgs e)
        {

            DateTime now = DateTime.Now;
            dtpTuNgay.Value = new DateTime(now.Year, now.Month, 1);
            dtpTuNgay.Value = now;

            LoadData();
        }
      
        private void LoadData()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();

                    string sql = @"
                        SELECT n.MaNhatKy, n.ThoiGian, n.HanhDong, t.TenDangNhap 
                        FROM NhatKyHoatDong n
                        LEFT JOIN TaiKhoan t ON n.MaTaiKhoan = t.MaTaiKhoan
                        WHERE (n.ThoiGian BETWEEN @TuNgay AND @DenNgay)
                        ORDER BY n.ThoiGian DESC"; 

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    DateTime fromDate = dtpTuNgay.Value.Date;
                    DateTime toDate = dtpTuNgay.Value.Date.AddDays(1).AddSeconds(-1);

                    cmd.Parameters.AddWithValue("@TuNgay", fromDate);
                    cmd.Parameters.AddWithValue("@DenNgay", toDate);
                   

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvNhatKy.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải nhật ký: " + ex.Message);
                }
            }
        }
        private void btnXem_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            
            dtpTuNgay.Value = DateTime.Now.AddDays(-30); // Xem 30 ngày gần nhất
            dtpTuNgay.Value = DateTime.Now;
            LoadData();
        }
    }
}
