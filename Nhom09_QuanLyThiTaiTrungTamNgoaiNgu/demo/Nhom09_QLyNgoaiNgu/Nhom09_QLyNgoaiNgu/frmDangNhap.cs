using Microsoft.Data.SqlClient;
using Nhom09_QLyNgoaiNgu; 
using System;
using System.Windows.Forms;

namespace Nhom09_QLyNgoaiNgu
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string u = txtDangNhap.Text.Trim();
            string p = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT MaTaiKhoan, MaQuyen, TrangThai FROM TaiKhoan WHERE TenDangNhap = @user AND MatKhau = @pass";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@user", u);
                    cmd.Parameters.AddWithValue("@pass", p);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        bool trangThai = Convert.ToBoolean(reader["TrangThai"]);
                        int maQuyen = Convert.ToInt32(reader["MaQuyen"]);

                        int maTaiKhoan = Convert.ToInt32(reader["MaTaiKhoan"]);

                        if (trangThai == false)
                        {
                            MessageBox.Show("Tài khoản này đã bị KHÓA!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }

                        MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        XemNhatKy.CurrentUserId = maTaiKhoan;
                        XemNhatKy.Record("Đăng nhập hệ thống");

                        this.Hide();

                        switch (maQuyen)
                        {
                            case 1: // Admin
                                Nhom09_QLyNgoaiNgu.Admin.frmAdmin fAdmin = new Nhom09_QLyNgoaiNgu.Admin.frmAdmin();
                                fAdmin.ShowDialog();
                                break;

                            case 2: // Giám thị
                                Nhom09_QLyNgoaiNgu.Admin.frmAdmin fGiamThi = new Nhom09_QLyNgoaiNgu.Admin.frmAdmin(maQuyen);

                                this.Hide(); 
                                fGiamThi.ShowDialog(); 
                                
                                break;

                            case 3: // Học viên
                                Nhom09_QLyNgoaiNgu.HocVien.frmHocVien fHocVien = new Nhom09_QLyNgoaiNgu.HocVien.frmHocVien(maTaiKhoan);
                                fHocVien.ShowDialog();
                                break;

                            default:
                                MessageBox.Show("Không xác định được quyền hạn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }

                        Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        private void chkHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHienMatKhau.Checked)
                txtMatKhau.PasswordChar = '\0';
            else
                txtMatKhau.PasswordChar = '*';
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}