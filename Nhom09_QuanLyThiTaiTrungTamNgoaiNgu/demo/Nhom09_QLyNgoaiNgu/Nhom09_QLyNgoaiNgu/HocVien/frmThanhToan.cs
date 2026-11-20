using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Nhom09_QLyNgoaiNgu.HocVien
{
    public partial class frmThanhToan : Form
    {
        private int _maTaiKhoanHienTai;
        private int _maThiSinhHienTai;

        public frmThanhToan(int maTaiKhoan)
        {
            InitializeComponent();
            _maTaiKhoanHienTai = maTaiKhoan;
        }

        private void frmThanhToan_Load(object sender, EventArgs e)
        {
            dgvCanDong.AutoGenerateColumns = false;
            dgvLichSu.AutoGenerateColumns = false;

            if (LayMaThiSinh())
            {
                LoadData();
                InitCombobox();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin thí sinh liên kết!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void InitCombobox()
        {
            if (cboPhuongThuc.Items.Count == 0)
            {
                cboPhuongThuc.Items.Add("Chuyển khoản Ngân hàng");
                cboPhuongThuc.Items.Add("Ví Momo");
                cboPhuongThuc.Items.Add("Tiền mặt tại văn phòng");
            }
            cboPhuongThuc.SelectedIndex = 0;
        }

        private bool LayMaThiSinh()
        {
            try
            {
                string sql = @"SELECT TS.MaThiSinh FROM ThiSinh TS 
                               JOIN TaiKhoan TK ON TS.Email = TK.Email 
                               WHERE TK.MaTaiKhoan = @MaTK";
                object result = DatabaseConnection.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@MaTK", _maTaiKhoanHienTai) });

                if (result != null)
                {
                    _maThiSinhHienTai = Convert.ToInt32(result);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        private void LoadData()
        {
            try
            {
                SqlParameter[] p1 = { new SqlParameter("@MaThiSinh", _maThiSinhHienTai) };
                DataTable dtCanDong = DatabaseConnection.GetDataTable("EXEC sp_LayDsCanThanhToan @MaThiSinh", p1);

                if (dtCanDong.Rows.Count > 0)
                {
                    dgvCanDong.DataSource = dtCanDong;
                }
                else
                {
                   
                    dgvCanDong.DataSource = null;
                }

                SqlParameter[] p2 = { new SqlParameter("@MaThiSinh", _maThiSinhHienTai) };
                DataTable dtLichSu = DatabaseConnection.GetDataTable("EXEC sp_LayLichSuThanhToan @MaThiSinh", p2);
                dgvLichSu.DataSource = dtLichSu;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (dgvCanDong.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn khoản phí cần thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvCanDong.SelectedRows[0];
            DataRowView drv = row.DataBoundItem as DataRowView;

            if (drv == null) return;

            string maDK = drv["MaDangKy"].ToString();
            string tenKyThi = drv["TenKyThi"].ToString();
            decimal soTien = Convert.ToDecimal(drv["LePhi"]);
            string phuongThuc = cboPhuongThuc.Text;

            string msg = $"XÁC NHẬN THANH TOÁN\n\n" +
                         $"- Kỳ thi: {tenKyThi}\n" +
                         $"- Số tiền: {soTien:N0} VNĐ\n" +
                         $"- Hình thức: {phuongThuc}\n\n" +
                         $"Bạn chắc chắn muốn thực hiện giao dịch?";

            if (MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string sql = "EXEC sp_ThucHienThanhToan @MaDangKy, @SoTien, @PhuongThuc, @GhiChu";
                    SqlParameter[] p = {
                        new SqlParameter("@MaDangKy", maDK),
                        new SqlParameter("@SoTien", soTien),
                        new SqlParameter("@PhuongThuc", phuongThuc),
                        new SqlParameter("@GhiChu", "Thanh toán qua App Winform")
                    };

                    DatabaseConnection.ExecuteNonQuery(sql, p);

                    MessageBox.Show("Thanh toán thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Giao dịch thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}