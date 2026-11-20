using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmKetQuaTimKiem : Form
    {
        private string _tuKhoa = "";
        public frmKetQuaTimKiem(string tuKhoa)
        {
            InitializeComponent();
            _tuKhoa = tuKhoa;
        }

        private void frmKetQuaTimKiem_Load(object sender, EventArgs e)
        {
            this.Text = "Kết quả tìm kiếm cho: " + _tuKhoa;
            LoadKetQua();
        }
        private void LoadKetQua()
        {
            try
            {
                dgvKetQua.AutoGenerateColumns = false;
                string q = @"SELECT 
                                GT.HoTen,
                                PC.NhiemVu,
                                KT.TenKyThi,
                                CT.NgayThi,
                                CT.ThoiGianBatDau,
                                PT.TenPhong
                             FROM PhanCongCoiThi PC
                             JOIN GiamThi GT ON PC.MaGiamThi = GT.MaGiamThi
                             JOIN CaThi CT ON PC.MaCaThi = CT.MaCaThi
                             JOIN PhongThi PT ON CT.MaPhong = PT.MaPhong
                             JOIN KyThi KT ON CT.MaKyThi = KT.MaKyThi
                             WHERE GT.HoTen LIKE @TuKhoa";

                SqlParameter[] p = { new SqlParameter("@TuKhoa", "%" + _tuKhoa + "%") };

                DataTable dt = DatabaseConnection.GetDataTable(q, p);
                dgvKetQua.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }
    }
}