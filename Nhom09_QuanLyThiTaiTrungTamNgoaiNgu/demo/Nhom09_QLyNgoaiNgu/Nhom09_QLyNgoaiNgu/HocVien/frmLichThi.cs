using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Nhom09_QLyNgoaiNgu.HocVien
{
    public partial class frmLichThi : Form
    {
        private int _maTaiKhoanHienTai;
        private int _maThiSinhHienTai;
        private string _loaiChungChiCanLoc; 

  
        public frmLichThi(int maTaiKhoan, string loaiChungChi = "")
        {
            InitializeComponent();
            _maTaiKhoanHienTai = maTaiKhoan;
            _loaiChungChiCanLoc = loaiChungChi;
        }

        private void frmLichThi_Load(object sender, EventArgs e)
        {
           
            if (!LayMaThiSinhTuTaiKhoan())
            {
                this.Close(); 
                return;
            }

            if (!string.IsNullOrEmpty(_loaiChungChiCanLoc))
                this.Text = "Lịch thi " + _loaiChungChiCanLoc; 
            else
                this.Text = "Tất cả lịch thi";

            SetupGrid();
            LoadData();
        }

        private bool LayMaThiSinhTuTaiKhoan()
        {
            try
            {
                string sql = @"SELECT TS.MaThiSinh 
                               FROM ThiSinh TS 
                               JOIN TaiKhoan TK ON TS.Email = TK.Email 
                               WHERE TK.MaTaiKhoan = @MaTK";

                object result = DatabaseConnection.ExecuteScalar(sql, new SqlParameter[] {
                    new SqlParameter("@MaTK", _maTaiKhoanHienTai)
                });

                if (result != null)
                {
                    _maThiSinhHienTai = Convert.ToInt32(result);
                    return true;
                }
                else
                {
                    MessageBox.Show("Tài khoản này chưa được liên kết với hồ sơ Thí sinh nào!\nVui lòng liên hệ Admin.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xác thực người dùng: " + ex.Message);
                return false;
            }
        }

        private void SetupGrid()
        {
            dgvLichThi.AutoGenerateColumns = false;
            dgvLichThi.Columns.Clear();

            dgvLichThi.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "MaKyThi", Visible = false });

            dgvLichThi.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên Kỳ Thi", DataPropertyName = "TenKyThi", Width = 250 });

            dgvLichThi.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày Thi",
                DataPropertyName = "NgayToChuc",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy", Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvLichThi.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Chứng Chỉ", DataPropertyName = "LoaiChungChi", Width = 100 });

            dgvLichThi.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Lệ Phí",
                DataPropertyName = "LePhi",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } // Định dạng số tiền
            });

            dgvLichThi.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng Thái Kỳ Thi", DataPropertyName = "TrangThai", Width = 120 });

            dgvLichThi.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTrangThaiDK", 
                HeaderText = "Tình Trạng Của Bạn",
                DataPropertyName = "TrangThaiDangKy",
                Width = 150
            });
        }

        private void LoadData()
        {
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@MaThiSinh", _maThiSinhHienTai),
                    new SqlParameter("@TuKhoa", txtTimKiem.Text.Trim()),
                    new SqlParameter("@LoaiChungChi", _loaiChungChiCanLoc) 
                };

                string sql = "EXEC sp_LayLichThiChoHocVien @MaThiSinh, @TuKhoa, @LoaiChungChi";

                DataTable dt = DatabaseConnection.GetDataTable(sql, p);
                dgvLichThi.DataSource = dt;

                foreach (DataGridViewRow row in dgvLichThi.Rows)
                {
                    var cellValue = row.Cells["colTrangThaiDK"].Value?.ToString();
                    if (cellValue == "Đã đăng ký")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch thi: " + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            if (dgvLichThi.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn kỳ thi muốn đăng ký!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvLichThi.SelectedRows[0];
            string trangThaiDK = row.Cells["colTrangThaiDK"].Value.ToString();
            string tenKyThi = row.Cells[1].Value.ToString(); 
            string maKyThi = row.Cells[0].Value.ToString(); 

            if (trangThaiDK == "Đã đăng ký")
            {
                MessageBox.Show("Bạn đã đăng ký kỳ thi này rồi!", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Xác nhận đăng ký tham gia kỳ thi:\n{tenKyThi}?\n\n(Lưu ý: Bạn cần thanh toán lệ phí để hoàn tất hồ sơ)",
                                "Xác nhận đăng ký", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string sql = "EXEC sp_HocVienDangKyThi @MaThiSinh, @MaKyThi";
                    SqlParameter[] p = {
                        new SqlParameter("@MaThiSinh", _maThiSinhHienTai),
                        new SqlParameter("@MaKyThi", maKyThi)
                    };

                    DatabaseConnection.ExecuteNonQuery(sql, p);

                    MessageBox.Show("Đăng ký thành công!\nVui lòng vào mục 'Thanh toán' để đóng lệ phí.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadData();
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Lỗi CSDL: " + sqlEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message);
                }
            }
        }
    }
}