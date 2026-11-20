using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmXepPhongThi : Form
    {
        private string _maCaThiHienTai = "";
        private string _maKyThiHienTai = "";
        private int _sucChuaCaThi = 0;
        private int _daXepHienTai = 0;

        public frmXepPhongThi()
        {
            InitializeComponent();
        }

        private void frmXepPhongThi_Load(object sender, EventArgs e)
        {
            LoadTreeView();
        }

        private void LoadTreeView()
        {
            string selectedTag = trvDanhSach.SelectedNode?.Tag?.ToString();
            trvDanhSach.Nodes.Clear();
            try
            {
                string sqlKy = "SELECT MaKyThi, TenKyThi FROM KyThi ORDER BY NgayToChuc DESC";
                DataTable dtKy = DatabaseConnection.GetDataTable(sqlKy);

                foreach (DataRow rKy in dtKy.Rows)
                {
                    TreeNode nodeKy = new TreeNode(rKy["TenKyThi"].ToString());
                    nodeKy.Tag = "K_" + rKy["MaKyThi"].ToString();

                    string sqlCa = @"SELECT CT.MaCaThi, CT.NgayThi, CT.ThoiGianBatDau, PT.TenPhong, 
                                     ISNULL(CT.SiSo, PT.SucChua) AS SucChuaThucTe,
                                     (SELECT COUNT(*) FROM DanhSachThi WHERE MaCaThi = CT.MaCaThi) as DaXep
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
                        string siSo = $"{rCa["DaXep"]}/{rCa["SucChuaThucTe"]}";

                        TreeNode nodeCa = new TreeNode($"{ngay} {gio} - {phong} ({siSo})");
                        nodeCa.Tag = rCa["MaCaThi"].ToString();
                        nodeKy.Nodes.Add(nodeCa);
                    }
                    trvDanhSach.Nodes.Add(nodeKy);
                }
                trvDanhSach.ExpandAll();

                if (!string.IsNullOrEmpty(selectedTag))
                {
                    foreach (TreeNode nKy in trvDanhSach.Nodes)
                    {
                        if (nKy.Tag.ToString() == selectedTag) { trvDanhSach.SelectedNode = nKy; break; }
                        foreach (TreeNode nCa in nKy.Nodes)
                        {
                            if (nCa.Tag.ToString() == selectedTag) { trvDanhSach.SelectedNode = nCa; break; }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi Load TreeView: " + ex.Message); }
        }

        private void trvDanhSach_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null && !e.Node.Tag.ToString().StartsWith("K_"))
            {
                _maCaThiHienTai = e.Node.Tag.ToString();
                _maKyThiHienTai = e.Node.Parent.Tag.ToString().Replace("K_", "");

                LoadDanhSachThiSinhTrongPhong(_maCaThiHienTai);
                LoadThiSinhChuaXepPhong(_maKyThiHienTai, _maCaThiHienTai);
                UpdateThongTinSiSo(_maCaThiHienTai);
            }
            else
            {
                _maCaThiHienTai = "";
                _maKyThiHienTai = "";
                dgvDanhSach.DataSource = null;
                cboThiSinh.DataSource = null;
                lblSiSo.Text = "0/0";
            }
        }

      private void LoadDanhSachThiSinhTrongPhong(string maCa)
{
    try
    {
        string q = @"SELECT DS.MaDanhSach, DK.MaDangKy, TS.HoTen, TS.NgaySinh, TS.CMND_CCCD, DS.SoBaoDanh, DS.SoGhe
                     FROM DanhSachThi DS
                     JOIN DangKyThi DK ON DS.MaDangKy = DK.MaDangKy
                     JOIN ThiSinh TS ON DK.MaThiSinh = TS.MaThiSinh
                     WHERE DS.MaCaThi = @MaCa
                     ORDER BY DS.SoGhe ASC, DS.SoBaoDanh ASC";

        DataTable dt = DatabaseConnection.GetDataTable(q, new SqlParameter[] { new SqlParameter("@MaCa", maCa) });
        dgvDanhSach.DataSource = dt;
        
        if (dgvDanhSach.Columns.Contains("MaDangKy")) dgvDanhSach.Columns["MaDangKy"].Visible = false;
        if (dgvDanhSach.Columns.Contains("MaDanhSach")) dgvDanhSach.Columns["MaDanhSach"].Visible = false;
        
        txtSBD.DataBindings.Clear();
        txtSoGhe.DataBindings.Clear();
        
        txtSBD.DataBindings.Add("Text", dgvDanhSach.DataSource, "SoBaoDanh", true, DataSourceUpdateMode.Never);
        txtSoGhe.DataBindings.Add("Text", dgvDanhSach.DataSource, "SoGhe", true, DataSourceUpdateMode.Never);
    }
    catch (Exception ex) { MessageBox.Show("Lỗi load danh sách: " + ex.Message); }
}

        private void LoadThiSinhChuaXepPhong(string maKy, string maCa)
        {
            if (string.IsNullOrEmpty(maKy) || string.IsNullOrEmpty(maCa))
            {
                cboThiSinh.DataSource = null;
                return;
            }

            try
            {

                string query = @"
            SELECT 
                DK.MaDangKy, 
                TS.HoTen + ' - ' + TS.CMND_CCCD AS HienThi
            FROM DangKyThi DK
            JOIN ThiSinh TS ON DK.MaThiSinh = TS.MaThiSinh
            WHERE DK.MaKyThi = @MaKy 
              AND DK.TrangThaiThanhToan = 1 
              AND NOT EXISTS (
                  SELECT 1 
                  FROM DanhSachThi DST 
                  WHERE DST.MaDangKy = DK.MaDangKy
              )";

                SqlParameter[] parameters = {
            new SqlParameter("@MaKy", maKy)
        };

                DataTable dt = DatabaseConnection.GetDataTable(query, parameters);

                cboThiSinh.DataSource = dt;
                cboThiSinh.DisplayMember = "HienThi";
                cboThiSinh.ValueMember = "MaDangKy";

                cboThiSinh.AutoCompleteSource = AutoCompleteSource.ListItems;

                cboThiSinh.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                cboThiSinh.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách thí sinh chưa xếp phòng: " + ex.Message,
                                "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateThongTinSiSo(string maCa)
        {
            try
            {
                string q = @"SELECT 
                                (SELECT COUNT(*) FROM DanhSachThi WHERE MaCaThi = @MaCa) as DaXep,
                                ISNULL(CT.SiSo, PT.SucChua) as SucChuaThucTe
                             FROM CaThi CT
                             JOIN PhongThi PT ON CT.MaPhong = PT.MaPhong
                             WHERE CT.MaCaThi = @MaCa";

                DataTable dt = DatabaseConnection.GetDataTable(q, new SqlParameter[] { new SqlParameter("@MaCa", maCa) });

                if (dt.Rows.Count > 0)
                {
                    _daXepHienTai = Convert.ToInt32(dt.Rows[0]["DaXep"]);
                    _sucChuaCaThi = Convert.ToInt32(dt.Rows[0]["SucChuaThucTe"]);
                    lblSiSo.Text = $"{_daXepHienTai}/{_sucChuaCaThi}";
                }
            }
            catch { lblSiSo.Text = "0/0"; }
        }

        private void dgvDanhSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDanhSach.CurrentRow != null)
            {
                try
                {
                    DataRowView drv = (DataRowView)dgvDanhSach.CurrentRow.DataBoundItem;

                    txtSBD.Text = drv["SoBaoDanh"].ToString();
                    txtSoGhe.Text = drv["SoGhe"].ToString();


                    string ten = drv["HoTen"].ToString();

                    string cccd = drv["CMND_CCCD"].ToString();

                    cboThiSinh.DropDownStyle = ComboBoxStyle.DropDown;
                    cboThiSinh.Text = $"{ten} - {cccd}";

                    cboThiSinh.Enabled = false;
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_maCaThiHienTai)) { MessageBox.Show("Vui lòng chọn Ca thi!"); return; }
            if (cboThiSinh.SelectedValue == null) { MessageBox.Show("Vui lòng chọn thí sinh!"); return; }
            if (string.IsNullOrWhiteSpace(txtSBD.Text)) { MessageBox.Show("Vui lòng nhập Số báo danh!"); return; }

            if (_daXepHienTai >= _sucChuaCaThi)
            {
                MessageBox.Show($"Ca thi đã đủ sĩ số ({_sucChuaCaThi})!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string q = "INSERT INTO DanhSachThi (MaDangKy, MaCaThi, SoBaoDanh, SoGhe) VALUES (@MaDK, @MaCa, @SBD, @Ghe)";
                SqlParameter[] p = {
                    new SqlParameter("@MaDK", cboThiSinh.SelectedValue),
                    new SqlParameter("@MaCa", _maCaThiHienTai),
                    new SqlParameter("@SBD", txtSBD.Text.Trim()),
                    new SqlParameter("@Ghe", txtSoGhe.Text.Trim())
                };

                DatabaseConnection.ExecuteNonQuery(q, p);
                RefreshData();
                ClearInput();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi thêm: " + ex.Message); }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0) { MessageBox.Show("Chọn thí sinh cần xóa!"); return; }

            if (MessageBox.Show("Xóa thí sinh này khỏi phòng thi?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    DataRowView drv = (DataRowView)dgvDanhSach.SelectedRows[0].DataBoundItem;
                    string maDS = drv["MaDanhSach"].ToString();

                    string q = "DELETE FROM DanhSachThi WHERE MaDanhSach = @ID";
                    DatabaseConnection.ExecuteNonQuery(q, new SqlParameter[] { new SqlParameter("@ID", maDS) });

                    RefreshData();
                    ClearInput();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0) return;
            try
            {
                DataRowView drv = (DataRowView)dgvDanhSach.SelectedRows[0].DataBoundItem;
                string maDS = drv["MaDanhSach"].ToString();

                string q = "UPDATE DanhSachThi SET SoBaoDanh = @SBD, SoGhe = @Ghe WHERE MaDanhSach = @ID";
                SqlParameter[] p = {
                    new SqlParameter("@SBD", txtSBD.Text.Trim()),
                    new SqlParameter("@Ghe", txtSoGhe.Text.Trim()),
                    new SqlParameter("@ID", maDS)
                };
                DatabaseConnection.ExecuteNonQuery(q, p);
                MessageBox.Show("Cập nhật thành công!");
                RefreshData();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi cập nhật: " + ex.Message); }
        }

        private void RefreshData()
        {
            LoadDanhSachThiSinhTrongPhong(_maCaThiHienTai);
            LoadThiSinhChuaXepPhong(_maKyThiHienTai, _maCaThiHienTai);
            UpdateThongTinSiSo(_maCaThiHienTai);

            if (trvDanhSach.SelectedNode != null)
            {
                string textOld = trvDanhSach.SelectedNode.Text;
                if (textOld.Contains("("))
                {
                    string part1 = textOld.Substring(0, textOld.LastIndexOf('('));
                    trvDanhSach.SelectedNode.Text = $"{part1}({_daXepHienTai}/{_sucChuaCaThi})";
                }
            }
        }

        private void ClearInput()
        {
            txtSBD.Clear();
            txtSoGhe.Clear();

            cboThiSinh.Enabled = true; 
            cboThiSinh.DropDownStyle = ComboBoxStyle.DropDownList;
            cboThiSinh.SelectedIndex = -1; 

            txtSBD.Focus();
        }
    }
}