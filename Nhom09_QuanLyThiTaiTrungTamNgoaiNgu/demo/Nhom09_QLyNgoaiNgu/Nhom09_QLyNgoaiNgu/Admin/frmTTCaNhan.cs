using Microsoft.Data.SqlClient;
using Nhom09_QLyNgoaiNgu;
using System;
using System.Data;
using System.Windows.Forms;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmTTCaNhan : Form
    {
        public frmTTCaNhan()
        {
            InitializeComponent();
        }

        private void frmTTCaNhan_Load(object sender, EventArgs e)
        {
            LoadThongTin();

            txtMatKhauCu.PasswordChar = '*';
            txtMatKhauMoi.PasswordChar = '*';
            txtXacNhanMK.PasswordChar = '*';
            chkMK.Checked = false;
        }

        private void LoadThongTin()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = @"
                                SELECT 
                                    t.TenDangNhap, 
                                    t.Email, 
                                    q.TenQuyen, 
                                    gt.HoTen, 
                                    gt.SoDienThoai
                                FROM TaiKhoan t
                                JOIN QuyenHan q ON t.MaQuyen = q.MaQuyen
                                LEFT JOIN GiamThi gt ON t.MaTaiKhoan = gt.MaTaiKhoan
                                WHERE t.MaTaiKhoan = @ID";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ID", XemNhatKy.CurrentUserId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtTenDangNhap.Text = reader["TenDangNhap"].ToString(); 
                        txtVaiTro.Text = reader["TenQuyen"].ToString();       
                        txtEmail.Text = reader["Email"].ToString();
                        txtHoTen.Text = reader["HoTen"] != DBNull.Value ? reader["HoTen"].ToString() : "";
                        txtSDT.Text = reader["SoDienThoai"] != DBNull.Value ? reader["SoDienThoai"].ToString() : "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải thông tin: " + ex.Message);
                }
            }
        }

        private void btnLuuThongTin_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Cập nhật bảng TaiKhoan (Email và Tên đăng nhập)
                    string sqlTK = "UPDATE TaiKhoan SET Email = @Email, TenDangNhap = @TenDN WHERE MaTaiKhoan = @ID";
                    SqlCommand cmdTK = new SqlCommand(sqlTK, conn, transaction);
                    cmdTK.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@TenDN", txtTenDangNhap.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@ID", XemNhatKy.CurrentUserId);
                    cmdTK.ExecuteNonQuery();

                    // 2. Cập nhật hoặc Thêm mới vào bảng GiamThi (Dùng cho cả Admin và Giám thị)
                    string sqlUpsert = @"
                IF EXISTS (SELECT 1 FROM GiamThi WHERE MaTaiKhoan = @ID)
                    UPDATE GiamThi SET HoTen = @Ten, SoDienThoai = @SDT WHERE MaTaiKhoan = @ID
                ELSE
                    INSERT INTO GiamThi (MaTaiKhoan, HoTen, SoDienThoai, Email) 
                    VALUES (@ID, @Ten, @SDT, @Email)";

                    SqlCommand cmdUpsert = new SqlCommand(sqlUpsert, conn, transaction);
                    cmdUpsert.Parameters.AddWithValue("@Ten", txtHoTen.Text.Trim());
                    cmdUpsert.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                    cmdUpsert.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmdUpsert.Parameters.AddWithValue("@ID", XemNhatKy.CurrentUserId);
                    cmdUpsert.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Cập nhật thông tin thành công!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMatKhauCu.Text) || string.IsNullOrEmpty(txtMatKhauMoi.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu!", "Cảnh báo");
                return;
            }

            if (txtMatKhauMoi.Text != txtXacNhanMK.Text)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();

                    string sqlCheck = "SELECT COUNT(*) FROM TaiKhoan WHERE MaTaiKhoan = @ID AND MatKhau = @Pass";
                    SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn);
                    cmdCheck.Parameters.AddWithValue("@ID", XemNhatKy.CurrentUserId);
                    cmdCheck.Parameters.AddWithValue("@Pass", txtMatKhauCu.Text);

                    int count = (int)cmdCheck.ExecuteScalar();

                    if (count == 0)
                    {
                        MessageBox.Show("Mật khẩu cũ không chính xác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string sqlUpdate = "UPDATE TaiKhoan SET MatKhau = @NewPass WHERE MaTaiKhoan = @ID";
                    SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn);
                    cmdUpdate.Parameters.AddWithValue("@NewPass", txtMatKhauMoi.Text);
                    cmdUpdate.Parameters.AddWithValue("@ID", XemNhatKy.CurrentUserId);

                    cmdUpdate.ExecuteNonQuery();

                    MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo");
                    XemNhatKy.Record("Đã đổi mật khẩu cá nhân");

                    txtMatKhauCu.Clear();
                    txtMatKhauMoi.Clear();
                    txtXacNhanMK.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đổi mật khẩu: " + ex.Message);
                }
            }
        }

        private void chkMK_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMK.Checked)
            {
                txtMatKhauMoi.PasswordChar = '\0';
                txtXacNhanMK.PasswordChar = '\0';
            }
            else
            {
                txtMatKhauMoi.PasswordChar = '*';
                txtXacNhanMK.PasswordChar = '*';
            }
        }
    }
}