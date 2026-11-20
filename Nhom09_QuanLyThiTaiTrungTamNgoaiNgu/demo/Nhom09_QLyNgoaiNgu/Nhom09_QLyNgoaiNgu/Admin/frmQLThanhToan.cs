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
    public partial class frmQLThanhToan : Form
    {
        int selectedMaDangKy = -1;

        public frmQLThanhToan()
        {
            InitializeComponent();
        }

        private void frmQLThanhToan_Load(object sender, EventArgs e)
        {
            if (txtSoTienCanDong != null)
            {
                txtSoTienCanDong.ReadOnly = false;
                txtSoTienCanDong.BackColor = Color.White;
                txtSoTienCanDong.Text = "0";
            }

            txtSoTienThu.ReadOnly = true;
            txtSoTienThu.BackColor = Color.WhiteSmoke;
            txtSoTienThu.Text = "0";

            LoadComboboxKyThi();

            cboTrangThai.Items.Clear();
            cboTrangThai.Items.Add("Tất cả");
            cboTrangThai.Items.Add("Chưa xong");
            cboTrangThai.Items.Add("Hoàn tất");
            cboTrangThai.SelectedIndex = 0;

            cboPhuongThuc.Items.Clear();
            cboPhuongThuc.Items.Add("Tiền mặt");
            cboPhuongThuc.Items.Add("Chuyển khoản");
            cboPhuongThuc.SelectedIndex = 0;

            btnXacNhan.Enabled = false;
            LoadData();
        }

        private void LoadComboboxKyThi()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT MaKyThi, TenKyThi FROM KyThi ORDER BY MaKyThi DESC";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    DataRow dr = dt.NewRow();
                    dr["MaKyThi"] = -1;
                    dr["TenKyThi"] = "--- Tất cả kỳ thi ---";
                    dt.Rows.InsertAt(dr, 0);
                    cboKyThi.DataSource = dt;
                    cboKyThi.DisplayMember = "TenKyThi";
                    cboKyThi.ValueMember = "MaKyThi";
                }
                catch (Exception ex) { MessageBox.Show("Lỗi tải kỳ thi: " + ex.Message); }
            }
        }

        private void LoadData()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_LayDanhSachThanhToan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    int maKyThi = -1;
                    if (cboKyThi.SelectedValue != null)
                        int.TryParse(cboKyThi.SelectedValue.ToString(), out maKyThi);

                    cmd.Parameters.AddWithValue("@MaKyThi", maKyThi != -1 ? maKyThi : (object)DBNull.Value);

                    int trangThai = -1;
                    if (cboTrangThai.SelectedIndex == 1) trangThai = 0;
                    if (cboTrangThai.SelectedIndex == 2) trangThai = 1;
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    cmd.Parameters.AddWithValue("@TuKhoa", txtTuKhoa.Text.Trim());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvThiSinh.AutoGenerateColumns = false;
                    dgvThiSinh.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadData();
        }


        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (selectedMaDangKy == -1)
            {
                MessageBox.Show("Vui lòng chọn học viên cần thanh toán!");
                return;
            }
            string inputMoney = txtSoTienCanDong.Text;

            string cleanMoney = new string(inputMoney.Where(c => char.IsDigit(c)).ToArray());

            decimal soTien = 0;
            decimal.TryParse(cleanMoney, out soTien);

            if (soTien <= 0)
            {
                MessageBox.Show("Số tiền thanh toán phải lớn hơn 0!");
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_ThucHienThanhToan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaDangKy", selectedMaDangKy);
                    cmd.Parameters.AddWithValue("@SoTien", soTien);
                    cmd.Parameters.AddWithValue("@PhuongThuc", cboPhuongThuc.Text);
                    cmd.Parameters.AddWithValue("@GhiChu", txtGhiChu.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Thanh toán thành công: " + soTien.ToString("N0") + " VNĐ", "Thông báo");

                    LoadData(); 

                    txtSoTienCanDong.Text = "0";
                    txtSoTienThu.Text = "0";
                    txtGhiChu.Text = "";
                    selectedMaDangKy = -1;
                    btnXacNhan.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thực hiện thanh toán: " + ex.Message);
                }
            }
        }

        private void dgvThiSinh_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvThiSinh.Rows.Count)
            {
                DataGridViewRow row = dgvThiSinh.Rows[e.RowIndex];

                if (row.Cells["colMaDangKy"].Value != DBNull.Value)
                {
                    selectedMaDangKy = Convert.ToInt32(row.Cells["colMaDangKy"].Value);
                }

                decimal conNo = 0;
                if (row.Cells["colConNo"].Value != DBNull.Value)
                {
                    decimal.TryParse(row.Cells["colConNo"].Value.ToString(), out conNo);
                }

                txtSoTienCanDong.Text = conNo.ToString("N0");
                txtSoTienThu.Text = conNo.ToString("N0");

                if (conNo > 0)
                {
                    btnXacNhan.Enabled = true;
                }
                else
                {
                    btnXacNhan.Enabled = false;
                }
            }
        }
    }
}