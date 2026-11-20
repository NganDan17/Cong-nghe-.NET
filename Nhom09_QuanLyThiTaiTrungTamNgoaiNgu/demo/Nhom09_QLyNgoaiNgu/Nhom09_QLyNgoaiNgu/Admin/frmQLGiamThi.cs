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

namespace Nhom09_QLyNgoaiNgu
{
    public partial class frmQLGiamThi : Form
    {
        public frmQLGiamThi()
        {
            InitializeComponent();
        }
        private void LoadDataToGrid()
        {
            try
            {
                string query = @"
                                SELECT g.MaGiamThi, g.HoTen, g.SoDienThoai, g.Email, g.ChuyenMon, t.TenDangNhap
                                FROM GiamThi g
                                LEFT JOIN TaiKhoan t ON g.MaTaiKhoan = t.MaTaiKhoan";
                DataTable dtGiamThi = DatabaseConnection.GetDataTable(query);
                dgvGiamThi.DataSource = dtGiamThi;

                if (dgvGiamThi.Columns["colMaGiamThi"] != null)
                {
                    dgvGiamThi.Columns["colMaGiamThi"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }
        private void ClearInputFields()
        {
            txtHoTen.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";
            txtChuyenMon.Text = "";
            txtHoTen.Focus();
        }
        private void frmQLGiamThi_Load(object sender, EventArgs e)
        {

            dgvGiamThi.AutoGenerateColumns = false;

            dgvGiamThi.AllowUserToAddRows = false;

            this.dgvGiamThi.SelectionChanged += new System.EventHandler(this.dgvGiamThi_SelectionChanged);

            LoadDataToGrid();
        }
        private bool IsInputValid()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Họ tên là bắt buộc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void dgvGiamThi_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGiamThi.CurrentRow != null && dgvGiamThi.CurrentRow.DataBoundItem != null)
            {
                try
                {
                    DataRowView row = (DataRowView)dgvGiamThi.CurrentRow.DataBoundItem;

                    txtHoTen.Text = row["HoTen"].ToString();
                    txtSDT.Text = row["SoDienThoai"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtChuyenMon.Text = row["ChuyenMon"].ToString();
                }
                catch (Exception ex)
                {
                    ClearInputFields();
                }
            }
            else
            {
                ClearInputFields();
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!IsInputValid())
            {
                return; 
            }

            try
            {
                string query = @"INSERT INTO GiamThi (HoTen, SoDienThoai, Email, ChuyenMon) 
                                 VALUES (@HoTen, @SDT, @Email, @ChuyenMon)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@HoTen", txtHoTen.Text),
                    new SqlParameter("@SDT", txtSDT.Text),
                    new SqlParameter("@Email", txtEmail.Text),
                    new SqlParameter("@ChuyenMon", txtChuyenMon.Text)
                };

                int rowsAffected = DatabaseConnection.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Thêm giám thị thành công!");
                    LoadDataToGrid();   
                    ClearInputFields(); 
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627) 
                {
                    MessageBox.Show("Lỗi: SĐT hoặc Email này đã tồn tại.");
                }
                else
                {
                    MessageBox.Show("Lỗi khi thêm dữ liệu: " + ex.Message);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvGiamThi.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một giám thị để xóa.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa giám thị này?",
                                                  "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.No)
            {
                return;
            }

            int maGiamThi = Convert.ToInt32(dgvGiamThi.CurrentRow.Cells["colMaGiamThi"].Value);

            try
            {
                string query = "DELETE FROM GiamThi WHERE MaGiamThi = @MaGiamThi";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaGiamThi", maGiamThi)
                };

                int rowsAffected = DatabaseConnection.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa giám thị thành công!");
                    LoadDataToGrid();
                    ClearInputFields();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) 
                {
                    MessageBox.Show("Không thể xóa. Giám thị này đang có lịch phân công coi thi.");
                }
                else
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            
            if (dgvGiamThi.CurrentRow == null || dgvGiamThi.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Vui lòng chọn một giám thị để sửa.");
                return;
            }

            if (!IsInputValid())
            {
                return;
            }

            try
            {
                DataRowView row = (DataRowView)dgvGiamThi.CurrentRow.DataBoundItem;
                int maGiamThi = Convert.ToInt32(row["MaGiamThi"]);
                
                string query = @"UPDATE GiamThi 
                         SET HoTen = @HoTen, SoDienThoai = @SDT, Email = @Email, ChuyenMon = @ChuyenMon
                         WHERE MaGiamThi = @MaGiamThi";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@HoTen", txtHoTen.Text),
            new SqlParameter("@SDT", txtSDT.Text),
            new SqlParameter("@Email", txtEmail.Text),
            new SqlParameter("@ChuyenMon", txtChuyenMon.Text),
            new SqlParameter("@MaGiamThi", maGiamThi)
                };

                int rowsAffected = DatabaseConnection.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Cập nhật giám thị thành công!");
                    LoadDataToGrid();

                    foreach (DataGridViewRow r in dgvGiamThi.Rows)
                    {
                        if (r.DataBoundItem != null &&
                            ((DataRowView)r.DataBoundItem)["MaGiamThi"].ToString() == maGiamThi.ToString())
                        {
                            r.Selected = true;
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy giám thị để cập nhật. Có thể dữ liệu đã bị xóa.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Lỗi: Số điện thoại hoặc Email này đã thuộc về người khác.");
                }
                else
                {
                    MessageBox.Show("Lỗi SQL khi cập nhật: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
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
                    string query = "SELECT MaGiamThi, HoTen, SoDienThoai, Email, ChuyenMon " +
                                   "FROM GiamThi WHERE HoTen LIKE @HoTen";

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@HoTen", "%" + filter + "%")
                    };

                    DataTable dt = DatabaseConnection.GetDataTable(query, parameters);
                    dgvGiamThi.DataSource = dt;

                    ClearInputFields();
                    txtHoTen.Text = filter;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
                }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            LoadDataToGrid();
        }

      
    }
 }

