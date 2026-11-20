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
using System.Text.RegularExpressions;

namespace Nhom09_QLyNgoaiNgu
{
    public partial class frmQLThiSinh : Form
    {
        public frmQLThiSinh()
        {
            InitializeComponent();
        }

        private void LoadDataToGrid()
        {
            try
            {
                string query = "SELECT MaThiSinh, HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, CMND_CCCD, DiaChi FROM ThiSinh";
                DataTable dtThiSinh = DatabaseConnection.GetDataTable(query);
                dgvThiSinh.DataSource = dtThiSinh;

                if (dgvThiSinh.Columns["colMaThiSinh"] != null)
                {
                    dgvThiSinh.Columns["colMaThiSinh"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputFields()
        {
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            if (cboGioiTinh.Items.Count > 0) cboGioiTinh.SelectedIndex = 0;
            txtSDT.Clear();
            txtEmail.Clear();
            txtCCCD.Clear();
            txtDiaChi.Clear();
            txtHoTen.Focus();
        }

        private bool IsInputValid()
        {
            // 1. Kiểm tra trống các trường bắt buộc
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtCCCD.Text))
            {
                MessageBox.Show("Họ tên và CMND/CCCD là bắt buộc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Kiểm tra CCCD: Đúng 12 số
            if (!Regex.IsMatch(txtCCCD.Text.Trim(), @"^\d{12}$"))
            {
                MessageBox.Show("CCCD phải bao gồm đúng 12 chữ số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCCCD.Focus();
                return false;
            }

            // 3. Kiểm tra Số điện thoại: 10 đến 11 số
            if (!Regex.IsMatch(txtSDT.Text.Trim(), @"^\d{10,11}$"))
            {
                MessageBox.Show("Số điện thoại phải có từ 10 đến 11 chữ số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return false;
            }

            // 4. Kiểm tra Email: Bắt buộc kết thúc bằng @gmail.com
            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                MessageBox.Show("Email không hợp lệ. Vui lòng sử dụng địa chỉ @gmail.com", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // 5. Kiểm tra Ngày sinh: Phải nhỏ hơn ngày hiện tại và đủ 8 tuổi
            DateTime ngaySinh = dtpNgaySinh.Value;
            DateTime ngayHienTai = DateTime.Now;
            int tuoi = ngayHienTai.Year - ngaySinh.Year;

            // Hiệu chỉnh tuổi nếu chưa đến ngày sinh nhật trong năm nay
            if (ngaySinh > ngayHienTai.AddYears(-tuoi)) tuoi--;

            if (ngaySinh >= ngayHienTai)
            {
                MessageBox.Show("Ngày sinh phải nhỏ hơn ngày hiện tại.", "Lỗi ngày sinh", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (tuoi < 8)
            {
                MessageBox.Show("Thí sinh phải từ đủ 8 tuổi trở lên.", "Lỗi độ tuổi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void frmQLThiSinh_Load(object sender, EventArgs e)
        {
            dgvThiSinh.AutoGenerateColumns = false;
            dgvThiSinh.AllowUserToAddRows = false;
            dgvThiSinh.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Thiết lập giới hạn ký tự tối đa cho TextBox ngay từ giao diện
            txtCCCD.MaxLength = 12;
            txtSDT.MaxLength = 11;

            LoadDataToGrid();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!IsInputValid()) return;

            try
            {
                string query = @"INSERT INTO ThiSinh (HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, CMND_CCCD, DiaChi) 
                                 VALUES (@HoTen, @NgaySinh, @GioiTinh, @SDT, @Email, @CCCD, @DiaChi)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@HoTen", txtHoTen.Text.Trim()),
                    new SqlParameter("@NgaySinh", dtpNgaySinh.Value),
                    new SqlParameter("@GioiTinh", cboGioiTinh.Text),
                    new SqlParameter("@SDT", txtSDT.Text.Trim()),
                    new SqlParameter("@Email", txtEmail.Text.Trim()),
                    new SqlParameter("@CCCD", txtCCCD.Text.Trim()),
                    new SqlParameter("@DiaChi", txtDiaChi.Text.Trim())
                };

                if (DatabaseConnection.ExecuteNonQuery(query, parameters) > 0)
                {
                    MessageBox.Show("Thêm thí sinh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToGrid();
                    ClearInputFields();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                    MessageBox.Show("Lỗi: SĐT, Email hoặc CCCD đã tồn tại trong hệ thống.");
                else
                    MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvThiSinh.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một thí sinh để sửa.");
                return;
            }
            if (!IsInputValid()) return;

            int maThiSinh = Convert.ToInt32(dgvThiSinh.CurrentRow.Cells["colMaThiSinh"].Value);

            try
            {
                string query = @"UPDATE ThiSinh SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, 
                                 SoDienThoai=@SDT, Email=@Email, CMND_CCCD=@CCCD, DiaChi=@DiaChi 
                                 WHERE MaThiSinh=@MaThiSinh";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@HoTen", txtHoTen.Text.Trim()),
                    new SqlParameter("@NgaySinh", dtpNgaySinh.Value),
                    new SqlParameter("@GioiTinh", cboGioiTinh.Text),
                    new SqlParameter("@SDT", txtSDT.Text.Trim()),
                    new SqlParameter("@Email", txtEmail.Text.Trim()),
                    new SqlParameter("@CCCD", txtCCCD.Text.Trim()),
                    new SqlParameter("@DiaChi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@MaThiSinh", maThiSinh)
                };

                if (DatabaseConnection.ExecuteNonQuery(query, parameters) > 0)
                {
                    MessageBox.Show("Cập nhật thí sinh thành công!");
                    LoadDataToGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string filter = txtHoTen.Text.Trim();
            if (string.IsNullOrEmpty(filter))
            {
                LoadDataToGrid();
            }
            else
            {
                try
                {
                    string query = "SELECT * FROM ThiSinh WHERE HoTen LIKE @Filter OR CMND_CCCD LIKE @Filter";
                    SqlParameter[] parameters = { new SqlParameter("@Filter", "%" + filter + "%") };
                    dgvThiSinh.DataSource = DatabaseConnection.GetDataTable(query, parameters);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
                }
            }
        }

        private void dgvThiSinh_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvThiSinh.CurrentRow != null && dgvThiSinh.CurrentRow.DataBoundItem != null)
            {
                DataRowView row = (DataRowView)dgvThiSinh.CurrentRow.DataBoundItem;
                txtHoTen.Text = row["HoTen"].ToString();
                cboGioiTinh.Text = row["GioiTinh"].ToString();
                txtSDT.Text = row["SoDienThoai"].ToString();
                txtEmail.Text = row["Email"].ToString();
                txtCCCD.Text = row["CMND_CCCD"].ToString();
                txtDiaChi.Text = row["DiaChi"].ToString();

                if (row["NgaySinh"] != DBNull.Value)
                    dtpNgaySinh.Value = Convert.ToDateTime(row["NgaySinh"]);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            LoadDataToGrid();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvThiSinh.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn thí sinh cần xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int maThiSinh = Convert.ToInt32(dgvThiSinh.CurrentRow.Cells["colMaThiSinh"].Value);
                try
                {
                    string query = "DELETE FROM ThiSinh WHERE MaThiSinh = @MaThiSinh";
                    SqlParameter[] parameters = { new SqlParameter("@MaThiSinh", maThiSinh) };

                    if (DatabaseConnection.ExecuteNonQuery(query, parameters) > 0)
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadDataToGrid();
                        ClearInputFields();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                        MessageBox.Show("Không thể xóa. Thí sinh này đã có dữ liệu liên quan.");
                    else
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
            }
        }
    }
}