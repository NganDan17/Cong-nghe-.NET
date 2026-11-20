using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Nhom09_QLyNgoaiNgu;

namespace Nhom09_QLyNgoaiNgu.HocVien
{
    public partial class frmHoSo : Form
    {
        private int maThiSinhHienTai = -1;
        private string emailGoc = ""; 

        public frmHoSo()
        {
            InitializeComponent();
        }

        public frmHoSo(int maThiSinh)
        {
            InitializeComponent();
            this.maThiSinhHienTai = maThiSinh;
        }

        private void frmHoSo_Load_1(object sender, EventArgs e)
        {
            chkHienMatKhau.Checked = false;

            if (this.maThiSinhHienTai == -1)
            {
                int currentUserId = XemNhatKy.CurrentUserId;
                if (currentUserId > 0)
                {
                    this.maThiSinhHienTai = LayMaThiSinhTuTaiKhoan(currentUserId);
                }
            }

            if (this.maThiSinhHienTai != -1)
            {
                TaiThongTin();
                btnLuu.Enabled = true;
            }
            else
            {
                MessageBox.Show("Không tìm thấy hồ sơ thí sinh nào.", "Lỗi dữ liệu");
                btnLuu.Enabled = false;
            }
        }

        private int LayMaThiSinhTuTaiKhoan(int maTaiKhoan)
        {
            int maTS = -1;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = @"
                        SELECT TOP 1 ts.MaThiSinh 
                        FROM ThiSinh ts
                        JOIN TaiKhoan tk ON LTRIM(RTRIM(ts.Email)) = LTRIM(RTRIM(tk.Email))
                        WHERE tk.MaTaiKhoan = @MaTK";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaTK", maTaiKhoan);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value) maTS = Convert.ToInt32(result);
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
            return maTS;
        }

        private void TaiThongTin()
        {
            try
            {
                string sql = "EXEC sp_LayThongTinThiSinh @MaThiSinh";
                SqlParameter[] param = { new SqlParameter("@MaThiSinh", maThiSinhHienTai) };

                DataTable dt = DatabaseConnection.GetDataTable(sql, param);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtHoTen.Text = row["HoTen"].ToString();
                    txtCCCD.Text = row["CMND_CCCD"].ToString();
                    if (row["NgaySinh"] != DBNull.Value) dtpNgaySinh.Value = Convert.ToDateTime(row["NgaySinh"]);
                    cboGioiTinh.Text = row["GioiTinh"].ToString();
                    txtSDT.Text = row["SoDienThoai"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtDiaChi.Text = row["DiaChi"].ToString();

                    emailGoc = txtEmail.Text.Trim();

                    txtCCCD.ReadOnly = true;      
                    txtHoTen.ReadOnly = false;   
                    txtEmail.ReadOnly = false;    
                    txtSDT.ReadOnly = false;     
                    txtDiaChi.ReadOnly = false;   
                    dtpNgaySinh.Enabled = true;   
                    cboGioiTinh.Enabled = true;  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông tin: " + ex.Message);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtSDT.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập tên, số điện thoại và email.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string sqlThiSinh = @"
                        UPDATE ThiSinh 
                        SET HoTen = @HoTen, SoDienThoai = @SDT, Email = @Email, DiaChi = @DiaChi, 
                            NgaySinh = @NgaySinh, GioiTinh = @GioiTinh
                        WHERE MaThiSinh = @MaThiSinh";

                    SqlCommand cmdThiSinh = new SqlCommand(sqlThiSinh, conn, transaction);
                    cmdThiSinh.Parameters.AddWithValue("@MaThiSinh", maThiSinhHienTai);
                    cmdThiSinh.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                    cmdThiSinh.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                    cmdThiSinh.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmdThiSinh.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text.Trim());
                    cmdThiSinh.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value);
                    cmdThiSinh.Parameters.AddWithValue("@GioiTinh", cboGioiTinh.Text);
                    cmdThiSinh.ExecuteNonQuery();


                    string emailMoi = txtEmail.Text.Trim();
                    if (!string.Equals(emailGoc, emailMoi, StringComparison.OrdinalIgnoreCase))
                    {
                        string checkDup = "SELECT COUNT(*) FROM TaiKhoan WHERE Email = @EmailMoi AND Email != @EmailCu";
                        SqlCommand cmdCheck = new SqlCommand(checkDup, conn, transaction);
                        cmdCheck.Parameters.AddWithValue("@EmailMoi", emailMoi);
                        cmdCheck.Parameters.AddWithValue("@EmailCu", emailGoc);

                        if ((int)cmdCheck.ExecuteScalar() > 0)
                        {
                            throw new Exception("Email mới đã tồn tại trên hệ thống!");
                        }

                        string sqlTK = "UPDATE TaiKhoan SET Email = @EmailMoi WHERE Email = @EmailCu";
                        SqlCommand cmdTK = new SqlCommand(sqlTK, conn, transaction);
                        cmdTK.Parameters.AddWithValue("@EmailMoi", emailMoi);
                        cmdTK.Parameters.AddWithValue("@EmailCu", emailGoc);
                        cmdTK.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    emailGoc = emailMoi; 
                    MessageBox.Show("Cập nhật hồ sơ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMatKhauCu.Text) || string.IsNullOrWhiteSpace(txtMatKhauMoi.Text) || string.IsNullOrWhiteSpace(txtXacNhanMK.Text))
            {
                MessageBox.Show("Nhập đủ thông tin mật khẩu.", "Cảnh báo"); return;
            }
            if (txtMatKhauMoi.Text != txtXacNhanMK.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi"); return;
            }

            try
            {
                string sql = "EXEC sp_DoiMatKhauBangEmail @Email, @MkCu, @MkMoi";
                SqlParameter[] paras = {
                    new SqlParameter("@Email", txtEmail.Text.Trim()),
                    new SqlParameter("@MkCu", txtMatKhauCu.Text),
                    new SqlParameter("@MkMoi", txtMatKhauMoi.Text)
                };

                object result = DatabaseConnection.ExecuteScalar(sql, paras);
                if (result != null)
                {
                    int kq = Convert.ToInt32(result);
                    if (kq == 1)
                    {
                        MessageBox.Show("Đổi mật khẩu thành công!");
                        txtMatKhauCu.Clear(); txtMatKhauMoi.Clear(); txtXacNhanMK.Clear();
                    }
                    else if (kq == 0) MessageBox.Show("Mật khẩu cũ sai.");
                    else MessageBox.Show("Không tìm thấy tài khoản.");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void chkHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {

            if (chkHienMatKhau.Checked)
            {
                
                txtMatKhauCu.UseSystemPasswordChar = false;
                txtMatKhauCu.PasswordChar = '\0'; 

                txtMatKhauMoi.UseSystemPasswordChar = false;
                txtMatKhauMoi.PasswordChar = '\0';

                txtXacNhanMK.UseSystemPasswordChar = false;
                txtXacNhanMK.PasswordChar = '\0';
            }
            else
            {
                txtMatKhauCu.PasswordChar = '*'; 
                txtMatKhauMoi.PasswordChar = '*';
                txtXacNhanMK.PasswordChar = '*';
            }
        }

        
    }
}