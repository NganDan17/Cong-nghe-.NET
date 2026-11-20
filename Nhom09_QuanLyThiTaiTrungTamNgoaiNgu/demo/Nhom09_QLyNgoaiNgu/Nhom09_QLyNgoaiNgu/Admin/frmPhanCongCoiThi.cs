using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Nhom09_QLyNgoaiNgu;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmPhanCongCoiThi : Form
    {
        private string _maCaThiHienTai = "";

        public frmPhanCongCoiThi()
        {
            InitializeComponent();
        }

        private void frmPhanCongCoiThi_Load(object sender, EventArgs e)
        {
            LoadDanhMuc();  
            LoadTreeView(); 

            dgvPhanCong.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPhanCong.MultiSelect = false;
        }

        private void LoadDanhMuc()
        {
            try
            {
                string qGT = "SELECT MaGiamThi, HoTen FROM GiamThi";
                DataTable dtGT = DatabaseConnection.GetDataTable(qGT);
                cboGiamThi.DataSource = dtGT;
                cboGiamThi.DisplayMember = "HoTen";
                cboGiamThi.ValueMember = "MaGiamThi";
                cboGiamThi.SelectedIndex = -1;

                cboNhiemVu.Items.Clear();
                cboNhiemVu.Items.AddRange(new string[] {
                    "Giám thị 1", "Giám thị 2", "Giám thị hành lang", "Thư ký", "Giám sát"
                });
                cboNhiemVu.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load danh mục: " + ex.Message);
            }
        }

        private void LoadTreeView()
        {
            trvDanhSach.Nodes.Clear();
            try
            {
                string sqlKy = "SELECT MaKyThi, TenKyThi FROM KyThi ORDER BY NgayToChuc DESC";
                DataTable dtKy = DatabaseConnection.GetDataTable(sqlKy);

                foreach (DataRow rKy in dtKy.Rows)
                {
                    TreeNode nodeKy = new TreeNode(rKy["TenKyThi"].ToString());
                    nodeKy.Tag = "K_" + rKy["MaKyThi"].ToString();

                    string sqlCa = @"SELECT CT.MaCaThi, CT.NgayThi, CT.ThoiGianBatDau, PT.TenPhong 
                                     FROM CaThi CT
                                     JOIN PhongThi PT ON CT.MaPhong = PT.MaPhong
                                     WHERE CT.MaKyThi = @MaKy
                                     ORDER BY CT.ThoiGianBatDau ASC";

                    SqlParameter[] p = { new SqlParameter("@MaKy", rKy["MaKyThi"]) };
                    DataTable dtCa = DatabaseConnection.GetDataTable(sqlCa, p);

                    foreach (DataRow rCa in dtCa.Rows)
                    {
                        string ngay = DateTime.Parse(rCa["NgayThi"].ToString()).ToString("dd/MM");
                        string gio = DateTime.Parse(rCa["ThoiGianBatDau"].ToString()).ToString("HH:mm");
                        string phong = rCa["TenPhong"].ToString();

                        TreeNode nodeCa = new TreeNode($"{ngay} - {gio} - {phong}");
                        nodeCa.Tag = rCa["MaCaThi"].ToString();
                        nodeKy.Nodes.Add(nodeCa);
                    }

                    trvDanhSach.Nodes.Add(nodeKy);
                }
                trvDanhSach.ExpandAll();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load TreeView: " + ex.Message); }
        }

        private void LoadDanhSachPhanCong(string maCa)
        {
            try
            {
                string q = @"SELECT PC.MaPhanCong, PC.MaGiamThi, GT.HoTen, PC.NhiemVu 
                             FROM PhanCongCoiThi PC
                             JOIN GiamThi GT ON PC.MaGiamThi = GT.MaGiamThi
                             WHERE PC.MaCaThi = @MaCa";

                DataTable dt = DatabaseConnection.GetDataTable(q, new SqlParameter[] { new SqlParameter("@MaCa", maCa) });
                dgvPhanCong.DataSource = dt;

                if (dgvPhanCong.Columns.Contains("MaPhanCong")) dgvPhanCong.Columns["MaPhanCong"].Visible = false;
                if (dgvPhanCong.Columns.Contains("MaGiamThi")) dgvPhanCong.Columns["MaGiamThi"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách phân công: " + ex.Message);
            }
        }

        private void trvDanhSach_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;

            try
            {
                if (e.Node.Tag != null && !e.Node.Tag.ToString().StartsWith("K_"))
                {
                    _maCaThiHienTai = e.Node.Tag.ToString();
                    LoadDanhSachPhanCong(_maCaThiHienTai);
                    ClearInputPhanCong();
                }
                else
                {
                    _maCaThiHienTai = null;
                    dgvPhanCong.DataSource = null;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi chọn dòng: " + ex.Message); }
        }

        private void dgvPhanCong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPhanCong.Rows[e.RowIndex];

                if (row.DataBoundItem is DataRowView drv)
                {
                    if (drv["MaGiamThi"] != DBNull.Value)
                    {
                        cboGiamThi.SelectedValue = drv["MaGiamThi"];
                    }

                    if (drv["NhiemVu"] != DBNull.Value)
                    {
                        cboNhiemVu.Text = drv["NhiemVu"].ToString();
                    }

                    if (drv["MaPhanCong"] != DBNull.Value)
                    {
                        btnSua.Tag = drv["MaPhanCong"].ToString();
                    }
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_maCaThiHienTai))
            {
                MessageBox.Show("Vui lòng chọn Ca thi trong danh sách bên trái!"); return;
            }
            if (cboGiamThi.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Giám thị!"); return;
            }

            try
            {
                string q = @"INSERT INTO PhanCongCoiThi (MaCaThi, MaGiamThi, NhiemVu)
                             VALUES (@MaCa, @MaGT, @NhiemVu)";

                SqlParameter[] p = {
                    new SqlParameter("@MaCa", _maCaThiHienTai),
                    new SqlParameter("@MaGT", cboGiamThi.SelectedValue),
                    new SqlParameter("@NhiemVu", cboNhiemVu.Text)
                };

                DatabaseConnection.ExecuteNonQuery(q, p);
                MessageBox.Show("Thêm thành công!");
                LoadDanhSachPhanCong(_maCaThiHienTai);
                ClearInputPhanCong();
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                    MessageBox.Show("Giám thị này đã được phân công trong ca thi này rồi!");
                else
                    MessageBox.Show("Lỗi SQL: " + sqlEx.Message);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi hệ thống: " + ex.Message); }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvPhanCong.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa!");
                return;
            }

            try
            {
                DataRowView drv = (DataRowView)dgvPhanCong.SelectedRows[0].DataBoundItem;
                string maPC = drv["MaPhanCong"].ToString();

                string q = @"UPDATE PhanCongCoiThi SET MaGiamThi=@MaGT, NhiemVu=@NhiemVu 
                     WHERE MaPhanCong=@ID";

                SqlParameter[] p = {
            new SqlParameter("@MaGT", cboGiamThi.SelectedValue),
            new SqlParameter("@NhiemVu", cboNhiemVu.Text),
            new SqlParameter("@ID", maPC)
        };

                DatabaseConnection.ExecuteNonQuery(q, p);
                MessageBox.Show("Cập nhật thành công!");

                LoadDanhSachPhanCong(_maCaThiHienTai);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvPhanCong.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!");
                return;
            }

            if (MessageBox.Show("Bạn muốn hủy phân công này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DataRowView drv = (DataRowView)dgvPhanCong.SelectedRows[0].DataBoundItem;
                    string maPC = drv["MaPhanCong"].ToString();

                    string q = "DELETE FROM PhanCongCoiThi WHERE MaPhanCong = @ID";

                    DatabaseConnection.ExecuteNonQuery(q, new SqlParameter[] { new SqlParameter("@ID", maPC) });

                    MessageBox.Show("Đã xóa!");
                    LoadDanhSachPhanCong(_maCaThiHienTai);
                    ClearInputPhanCong();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = cboGiamThi.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Vui lòng nhập tên Giám thị cần tìm!");
                cboGiamThi.Focus();
                return;
            }

            frmKetQuaTimKiem frm = new frmKetQuaTimKiem(keyword);

            frm.ShowDialog();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            cboGiamThi.SelectedIndex = -1;
            cboGiamThi.Text = ""; 
            cboNhiemVu.SelectedIndex = 0; 

            dgvPhanCong.ClearSelection();

            if (dgvPhanCong.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = ""; 
            }
        }

        private void ClearInputPhanCong()
        {
            cboGiamThi.SelectedIndex = -1;
            cboNhiemVu.SelectedIndex = 0;
            if (dgvPhanCong.DataSource is DataTable dt) dt.DefaultView.RowFilter = ""; 
        }
    }
}