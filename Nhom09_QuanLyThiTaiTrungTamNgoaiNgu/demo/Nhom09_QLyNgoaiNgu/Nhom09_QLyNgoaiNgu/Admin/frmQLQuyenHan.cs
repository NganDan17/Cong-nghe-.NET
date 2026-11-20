using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient; 

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmQLQuyenHan : Form
    {
        bool isLoading = false;

        public frmQLQuyenHan()
        {
            InitializeComponent();
        }

        private void frmQLQuyenHan_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            isLoading = true;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM QuyenHan", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvQuyenHan.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
            isLoading = false; 
        }
        private void DgvQuyenHan_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (isLoading) return;
            if (dgvQuyenHan.Rows[e.RowIndex].IsNewRow) return;

            var row = dgvQuyenHan.Rows[e.RowIndex];

            string maStr = row.Cells["colMaQuyen"].Value?.ToString();
            string tenQuyen = row.Cells["colTenQuyen"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(tenQuyen)) return;

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("", conn);

                    if (!string.IsNullOrEmpty(maStr))
                    {
                        string sqlCheck = "SELECT COUNT(*) FROM QuyenHan WHERE TenQuyen = @Ten AND MaQuyen != @Ma";
                        SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn);
                        cmdCheck.Parameters.AddWithValue("@Ten", tenQuyen.Trim());
                        cmdCheck.Parameters.AddWithValue("@Ma", maStr);

                        if ((int)cmdCheck.ExecuteScalar() > 0)
                        {
                            MessageBox.Show($"Tên quyền '{tenQuyen}' đã tồn tại!", "Trùng lặp");
                            LoadData(); 
                            return;
                        }

                        cmd.CommandText = "UPDATE QuyenHan SET TenQuyen = @Ten WHERE MaQuyen = @Ma";
                        cmd.Parameters.AddWithValue("@Ma", maStr);
                    }
                    else
                    {
                        string sqlCheck = "SELECT COUNT(*) FROM QuyenHan WHERE TenQuyen = @Ten";
                        SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn);
                        cmdCheck.Parameters.AddWithValue("@Ten", tenQuyen.Trim());

                        if ((int)cmdCheck.ExecuteScalar() > 0)
                        {
                            MessageBox.Show($"Tên quyền '{tenQuyen}' đã tồn tại!", "Trùng lặp");
                            LoadData();
                            return;
                        }

                        cmd.CommandText = "INSERT INTO QuyenHan (TenQuyen) VALUES (@Ten)";
                    }

                    cmd.Parameters.AddWithValue("@Ten", tenQuyen.Trim());
                    cmd.ExecuteNonQuery();

                    if (string.IsNullOrEmpty(maStr)) LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lưu: " + ex.Message);
                }
            }
        }
        private void DgvQuyenHan_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string maCanXoa = e.Row.Cells["colMaQuyen"].Value?.ToString();
            string tenCanXoa = e.Row.Cells["colTenQuyen"].Value?.ToString();

            if (string.IsNullOrEmpty(maCanXoa)) return;

            if (maCanXoa == "1" || (tenCanXoa != null && tenCanXoa.ToLower().Contains("admin")))
            {
                MessageBox.Show("Không được phép xóa quyền Quản Trị Viên!", "Cấm xóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
                return;
            }

            if (MessageBox.Show($"Bạn có chắc muốn xóa quyền '{tenCanXoa}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();

                    SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM TaiKhoan WHERE MaQuyen = @Ma", conn);
                    cmdCheck.Parameters.AddWithValue("@Ma", maCanXoa);

                    if ((int)cmdCheck.ExecuteScalar() > 0)
                    {
                        MessageBox.Show($"Không thể xóa! Đang có Tài khoản thuộc quyền '{tenCanXoa}'.", "Ràng buộc dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        e.Cancel = true;
                        return;
                    }

                    SqlCommand cmd = new SqlCommand("DELETE FROM QuyenHan WHERE MaQuyen = @Ma", conn);
                    cmd.Parameters.AddWithValue("@Ma", maCanXoa);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa: " + ex.Message);
                    e.Cancel = true;
                }
            }
        }
    }
}