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
    public partial class frmQLAdmin : Form
    {
        public frmQLAdmin()
        {
            InitializeComponent();
        }

        private void LoadComboBoxQuyen()
        {
            try
            {
                string query = "SELECT MaQuyen, TenQuyen FROM QuyenHan";
                DataTable dt = DatabaseConnection.GetDataTable(query);
                cboQuyen.DataSource = dt;
                cboQuyen.DisplayMember = "TenQuyen";
                cboQuyen.ValueMember = "MaQuyen";
                cboQuyen.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load quyền: " + ex.Message);
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                string query = @"SELECT t.MaTaiKhoan, t.TenDangNhap, t.MatKhau, t.Email, 
                                        t.TrangThai, t.MaQuyen, q.TenQuyen 
                                 FROM TaiKhoan t 
                                 JOIN QuyenHan q ON t.MaQuyen = q.MaQuyen";

                DataTable dt = DatabaseConnection.GetDataTable(query);
                dgvTaiKhoan.DataSource = dt;

                if (dgvTaiKhoan.Columns.Contains("colMaTaiKhoan")) dgvTaiKhoan.Columns["colMaTaiKhoan"].Visible = false;
                if (dgvTaiKhoan.Columns.Contains("colMaQuyen")) dgvTaiKhoan.Columns["colMaQuyen"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void ClearInputFields()
        {
            txtTenDangNhap.Text = "";
            txtMatKhau.Text = "";
            txtEmail.Text = "";
            cboQuyen.SelectedIndex = -1;
            chkTrangThai.Checked = true;
            txtTenDangNhap.Focus();
        }

        private void frmQLAdmin_Load(object sender, EventArgs e)
        {
            dgvTaiKhoan.AutoGenerateColumns = false;
            LoadComboBoxQuyen();
            LoadDataToGrid();
            this.dgvTaiKhoan.SelectionChanged += new System.EventHandler(this.dgvTaiKhoan_SelectionChanged);
        }

        private void dgvTaiKhoan_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTaiKhoan.CurrentRow != null && dgvTaiKhoan.CurrentRow.DataBoundItem != null)
            {
                try
                {
                    DataRowView row = (DataRowView)dgvTaiKhoan.CurrentRow.DataBoundItem;

                    txtTenDangNhap.Text = row.Row.Table.Columns.Contains("TenDangNhap") ? row["TenDangNhap"].ToString() : "";
                    txtMatKhau.Text = row.Row.Table.Columns.Contains("MatKhau") ? row["MatKhau"].ToString() : "";
                    txtEmail.Text = row.Row.Table.Columns.Contains("Email") ? row["Email"].ToString() : "";

                    if (row.Row.Table.Columns.Contains("MaQuyen") && row["MaQuyen"] != DBNull.Value)
                        cboQuyen.SelectedValue = row["MaQuyen"];

                    if (row.Row.Table.Columns.Contains("TrangThai") && row["TrangThai"] != DBNull.Value)
                        chkTrangThai.Checked = Convert.ToBoolean(row["TrangThai"]);
                }
                catch { ClearInputFields(); }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text) || string.IsNullOrWhiteSpace(txtMatKhau.Text) || cboQuyen.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    string sqlTK = @"INSERT INTO TaiKhoan (TenDangNhap, MatKhau, Email, MaQuyen, TrangThai) 
                                     VALUES (@User, @Pass, @Email, @MaQuyen, @TrangThai);
                                     SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdTK = new SqlCommand(sqlTK, conn, transaction);
                    cmdTK.Parameters.AddWithValue("@User", txtTenDangNhap.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@Pass", txtMatKhau.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@MaQuyen", cboQuyen.SelectedValue);
                    cmdTK.Parameters.AddWithValue("@TrangThai", chkTrangThai.Checked);

                    int newMaTK = Convert.ToInt32(cmdTK.ExecuteScalar());

                    int maQuyen = Convert.ToInt32(cboQuyen.SelectedValue);
                    if (maQuyen == 1 || maQuyen == 2)
                    {
                        string sqlInfo = @"IF EXISTS (SELECT 1 FROM GiamThi WHERE Email = @Email)
                                            UPDATE GiamThi SET MaTaiKhoan = @MaTK WHERE Email = @Email
                                        ELSE
                                            INSERT INTO GiamThi (MaTaiKhoan, HoTen, Email) 
                                            VALUES (@MaTK, @HoTen, @Email)";

                        SqlCommand cmdInfo = new SqlCommand(sqlInfo, conn, transaction);
                        cmdInfo.Parameters.AddWithValue("@MaTK", newMaTK);
                        cmdInfo.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmdInfo.Parameters.AddWithValue("@HoTen", maQuyen == 1 ? "Quản trị viên" : "Giám thị mới");
                        cmdInfo.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Thêm tài khoản thành công!");
                    LoadDataToGrid();
                    ClearInputFields();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Lỗi thêm: " + ex.Message);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvTaiKhoan.CurrentRow == null) return;
            int maTK = Convert.ToInt32(dgvTaiKhoan.CurrentRow.Cells["colMaTaiKhoan"].Value);
            string emailMoi = txtEmail.Text.Trim();

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    string queryTK = "UPDATE TaiKhoan SET TenDangNhap=@User, MatKhau=@Pass, Email=@Email, MaQuyen=@Quyen, TrangThai=@Status WHERE MaTaiKhoan=@MaTK";
                    SqlCommand cmdTK = new SqlCommand(queryTK, conn, tran);
                    cmdTK.Parameters.AddWithValue("@User", txtTenDangNhap.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@Pass", txtMatKhau.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@Email", emailMoi);
                    cmdTK.Parameters.AddWithValue("@Quyen", cboQuyen.SelectedValue);
                    cmdTK.Parameters.AddWithValue("@Status", chkTrangThai.Checked);
                    cmdTK.Parameters.AddWithValue("@MaTK", maTK);
                    cmdTK.ExecuteNonQuery();

                    int maQuyen = Convert.ToInt32(cboQuyen.SelectedValue);
                    if (maQuyen == 1 || maQuyen == 2) 
                    {
                        string sqlRemoveOld = "UPDATE GiamThi SET MaTaiKhoan = NULL WHERE MaTaiKhoan = @MaTK";
                        SqlCommand cmdRemove = new SqlCommand(sqlRemoveOld, conn, tran);
                        cmdRemove.Parameters.AddWithValue("@MaTK", maTK);
                        cmdRemove.ExecuteNonQuery();

                        string sqlCheck = "SELECT COUNT(*) FROM GiamThi WHERE Email = @Email";
                        SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn, tran);
                        cmdCheck.Parameters.AddWithValue("@Email", emailMoi);
                        int exists = (int)cmdCheck.ExecuteScalar();

                        if (exists > 0)
                        {
                            string sqlUpdateGT = "UPDATE GiamThi SET MaTaiKhoan = @MaTK WHERE Email = @Email";
                            SqlCommand cmdUpdateGT = new SqlCommand(sqlUpdateGT, conn, tran);
                            cmdUpdateGT.Parameters.AddWithValue("@MaTK", maTK);
                            cmdUpdateGT.Parameters.AddWithValue("@Email", emailMoi);
                            cmdUpdateGT.ExecuteNonQuery();
                        }
                        else
                        {
                            string sqlInsertGT = "INSERT INTO GiamThi (MaTaiKhoan, HoTen, Email) VALUES (@MaTK, @HoTen, @Email)";
                            SqlCommand cmdInsertGT = new SqlCommand(sqlInsertGT, conn, tran);
                            cmdInsertGT.Parameters.AddWithValue("@MaTK", maTK);
                            cmdInsertGT.Parameters.AddWithValue("@Email", emailMoi);
                            cmdInsertGT.Parameters.AddWithValue("@HoTen", maQuyen == 1 ? "Quản trị viên" : "Giám thị mới");
                            cmdInsertGT.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                    MessageBox.Show("Cập nhật thông tin và liên kết giám thị thành công!");
                    LoadDataToGrid();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Lỗi sửa: " + ex.Message);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvTaiKhoan.CurrentRow == null) return;
            int maTK = Convert.ToInt32(dgvTaiKhoan.CurrentRow.Cells["colMaTaiKhoan"].Value);

            if (MessageBox.Show("Bạn chắc chắn xóa tài khoản?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    SqlTransaction tran = conn.BeginTransaction();
                    try
                    {
                        SqlCommand cmdLog = new SqlCommand("DELETE FROM NhatKyHoatDong WHERE MaTaiKhoan = @id", conn, tran);
                        cmdLog.Parameters.AddWithValue("@id", maTK);
                        cmdLog.ExecuteNonQuery();

                        SqlCommand cmdGT = new SqlCommand("UPDATE GiamThi SET MaTaiKhoan = NULL WHERE MaTaiKhoan = @id", conn, tran);
                        cmdGT.Parameters.AddWithValue("@id", maTK);
                        cmdGT.ExecuteNonQuery();

                        SqlCommand cmdTK = new SqlCommand("DELETE FROM TaiKhoan WHERE MaTaiKhoan = @id", conn, tran);
                        cmdTK.Parameters.AddWithValue("@id", maTK);
                        cmdTK.ExecuteNonQuery();

                        tran.Commit();
                        LoadDataToGrid();
                        MessageBox.Show("Xóa tài khoản thành công!");
                        ClearInputFields();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Lỗi xóa: " + ex.Message);
                    }
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"SELECT t.MaTaiKhoan, t.TenDangNhap, t.MatKhau, t.Email, 
                                        t.TrangThai, t.MaQuyen, q.TenQuyen 
                                 FROM TaiKhoan t 
                                 JOIN QuyenHan q ON t.MaQuyen = q.MaQuyen
                                 WHERE 1=1"; 
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
                {
                    query += " AND t.TenDangNhap LIKE @User";
                    parameters.Add(new SqlParameter("@User", "%" + txtTenDangNhap.Text.Trim() + "%"));
                }
                if (!string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    query += " AND t.Email LIKE @Email";
                    parameters.Add(new SqlParameter("@Email", "%" + txtEmail.Text.Trim() + "%"));
                }

                if (cboQuyen.SelectedIndex != -1)
                {
                    query += " AND t.MaQuyen = @TenQuyen";
                    parameters.Add(new SqlParameter("@TenQuyen", cboQuyen.SelectedValue));
                }

                DataTable dt = DatabaseConnection.GetDataTable(query, parameters.ToArray());

                dgvTaiKhoan.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy kết quả nào phù hợp!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            LoadDataToGrid();
        }
    }
}