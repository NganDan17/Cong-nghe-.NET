using Microsoft.Data.SqlClient; 
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmQLPhongThi : Form
    {
        public frmQLPhongThi()
        {
            InitializeComponent();
        }

        private void frmQLPhongThi_Load(object sender, EventArgs e)
        {
            numSucChua.Maximum = 1000;
            numSucChua.Minimum = 0;
            LoadData();
            ResetInput();
            dgvPhongThi.CellClick += dgvPhongThi_CellClick;
        }

        private void LoadData(string tuKhoa = "")
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT MaPhong, TenPhong, SucChua, DiaDiem FROM PhongThi";

                    if (!string.IsNullOrWhiteSpace(tuKhoa))
                    {
                        sql += " WHERE MaPhong LIKE @TuKhoa OR TenPhong LIKE @TuKhoa OR DiaDiem LIKE @TuKhoa";
                    }

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (!string.IsNullOrWhiteSpace(tuKhoa))
                    {
                        cmd.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvPhongThi.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void dgvPhongThi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataRowView drv = dgvPhongThi.Rows[e.RowIndex].DataBoundItem as DataRowView;
                    if (drv != null)
                    {
                        txtMaPhong.Text = drv["MaPhong"].ToString();
                        txtTenPhong.Text = drv["TenPhong"].ToString();
                        txtDiaDiem.Text = drv["DiaDiem"].ToString();
                        if (drv["SucChua"] != DBNull.Value)
                        {
                            numSucChua.Value = Convert.ToDecimal(drv["SucChua"]);
                        }
                        txtMaPhong.ReadOnly = true;
                        btnThem.Enabled = false;
                        btnSua.Enabled = true;
                        btnXoa.Enabled = true;
                    }
                    else
                    {
                        ResetInput();
                    }
                }
                catch (Exception ex)
                {
                    ResetInput();
                }
            }
        }

        private void ResetInput()
        {
            txtMaPhong.Clear();
            txtTenPhong.Clear();
            txtDiaDiem.Clear();
            numSucChua.Value = 30;
            txtTimKiem.Clear();

            txtMaPhong.ReadOnly = false;
            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            txtMaPhong.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaPhong.Text) || string.IsNullOrWhiteSpace(txtTenPhong.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã và Tên phòng!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM PhongThi WHERE MaPhong = @MaPhong", conn);
                    checkCmd.Parameters.AddWithValue("@MaPhong", txtMaPhong.Text.Trim());
                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Mã phòng này đã tồn tại!", "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = "INSERT INTO PhongThi (MaPhong, TenPhong, SucChua, DiaDiem) VALUES (@MaPhong, @TenPhong, @SucChua, @DiaDiem)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaPhong", txtMaPhong.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenPhong", txtTenPhong.Text.Trim());
                    cmd.Parameters.AddWithValue("@SucChua", (int)numSucChua.Value);
                    cmd.Parameters.AddWithValue("@DiaDiem", txtDiaDiem.Text.Trim());

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm phòng thành công!", "Thông báo");
                    LoadData();
                    ResetInput();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaPhong.Text)) return;

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = "UPDATE PhongThi SET TenPhong = @TenPhong, SucChua = @SucChua, DiaDiem = @DiaDiem WHERE MaPhong = @MaPhong";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaPhong", txtMaPhong.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenPhong", txtTenPhong.Text.Trim());
                    cmd.Parameters.AddWithValue("@SucChua", (int)numSucChua.Value);
                    cmd.Parameters.AddWithValue("@DiaDiem", txtDiaDiem.Text.Trim());

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật thành công!", "Thông báo");
                    LoadData();
                    ResetInput();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaPhong.Text)) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa phòng '{txtTenPhong.Text}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM CaThi WHERE MaPhong = @MaPhong", conn);
                        checkCmd.Parameters.AddWithValue("@MaPhong", txtMaPhong.Text);

                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            MessageBox.Show("Không thể xóa phòng đang có lịch thi!", "Ràng buộc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        SqlCommand cmd = new SqlCommand("DELETE FROM PhongThi WHERE MaPhong = @MaPhong", conn);
                        cmd.Parameters.AddWithValue("@MaPhong", txtMaPhong.Text);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Xóa thành công!", "Thông báo");
                        LoadData();
                        ResetInput();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi SQL: " + ex.Message);
                    }
                }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ResetInput();
            LoadData();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadData(txtTimKiem.Text.Trim());
        }

       
    }
}