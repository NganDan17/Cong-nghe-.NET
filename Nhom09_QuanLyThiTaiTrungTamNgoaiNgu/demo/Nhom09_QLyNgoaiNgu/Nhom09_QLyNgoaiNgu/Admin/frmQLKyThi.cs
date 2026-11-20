using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmQLKyThi : Form
    {
        public frmQLKyThi()
        {
            InitializeComponent();
        }

        private void frmQLKyThi_Load(object sender, EventArgs e)
        {
            LoadDanhMuc();
            LoadTreeView();
        }

        private void LoadDanhMuc()
        {
            try
            {
                string qPhong = "SELECT MaPhong, TenPhong, SucChua FROM PhongThi";
                DataTable dtPhong = DatabaseConnection.GetDataTable(qPhong);
                cboPhongThi.DataSource = dtPhong;
                cboPhongThi.DisplayMember = "TenPhong";
                cboPhongThi.ValueMember = "MaPhong";

                cboTrangThai.Items.Clear();
                cboTrangThai.Items.AddRange(new object[] { "Đang mở đăng ký", "Sắp mở", "Đã kết thúc", "Chưa mở" });
                cboTrangThai.SelectedIndex = 0;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load danh mục: " + ex.Message); }
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
                    nodeKy.Tag = rKy["MaKyThi"].ToString();

                    string sqlCa = @"SELECT CT.MaCaThi, CT.NgayThi, CT.ThoiGianBatDau, PT.TenPhong, 
                                            ISNULL(CT.SiSo, PT.SucChua) as SucChuaHienTai,
                                            (SELECT COUNT(*) FROM DanhSachThi WHERE MaCaThi = CT.MaCaThi) as DaDK
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
                        string siso = $"{rCa["DaDK"]}/{rCa["SucChuaHienTai"]}";

                        TreeNode nodeCa = new TreeNode($"{ngay} - {gio} - {phong} ({siso})");
                        nodeCa.Tag = rCa["MaCaThi"].ToString();
                        nodeKy.Nodes.Add(nodeCa);
                    }
                    trvDanhSach.Nodes.Add(nodeKy);
                }
                trvDanhSach.ExpandAll();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load TreeView: " + ex.Message); }
        }

        private void trvDanhSach_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.Level == 0)
            {
                LoadChiTietKyThi(e.Node.Tag.ToString());
                ClearInputCaThi();
            }
            else if (e.Node.Level == 1)
            {
                LoadChiTietCaThi(e.Node.Tag.ToString());
                if (e.Node.Parent != null)
                    LoadChiTietKyThi(e.Node.Parent.Tag.ToString());
            }
        }

        private void LoadChiTietKyThi(string id)
        {
            string q = "SELECT * FROM KyThi WHERE MaKyThi = @ID";
            DataTable dt = DatabaseConnection.GetDataTable(q, new SqlParameter[] { new SqlParameter("@ID", id) });
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                txtTenKyThi.Text = r["TenKyThi"].ToString();
                txtLePhi.Text = r["LePhi"].ToString();
                dtpNgayToChuc.Value = Convert.ToDateTime(r["NgayToChuc"]);
                cboTrangThai.Text = r["TrangThai"].ToString();
            }
        }

        private void LoadChiTietCaThi(string id)
        {
            string q = "SELECT CT.*, PT.SucChua FROM CaThi CT JOIN PhongThi PT ON CT.MaPhong = PT.MaPhong WHERE MaCaThi = @ID";
            DataTable dt = DatabaseConnection.GetDataTable(q, new SqlParameter[] { new SqlParameter("@ID", id) });
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                dtpNgayThiCa.Value = Convert.ToDateTime(r["NgayThi"]);
                txtGioBatDau.Text = Convert.ToDateTime(r["ThoiGianBatDau"]).ToString("HH:mm");
                txtGioKetThuc.Text = Convert.ToDateTime(r["ThoiGianKetThuc"]).ToString("HH:mm");
                cboPhongThi.SelectedValue = r["MaPhong"];
                txtSiSo.Text = r["SiSo"] != DBNull.Value ? r["SiSo"].ToString() : r["SucChua"].ToString();
            }
        }

        // --- NÚT LƯU CA THI ---
        private void btnLuuCa_Click(object sender, EventArgs e)
        {
            if (trvDanhSach.SelectedNode == null) { MessageBox.Show("Vui lòng chọn Kỳ thi hoặc Ca thi trên danh sách!"); return; }

            // Kiểm tra sĩ số > 0
            if (!int.TryParse(txtSiSo.Text, out int sisoNhap) || sisoNhap <= 0)
            {
                MessageBox.Show("Sĩ số phải là số nguyên dương!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSiSo.Focus();
                return;
            }

            // Kiểm tra ngày thi không phải quá khứ
            if (dtpNgayThiCa.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Ngày thi không được là ngày trong quá khứ!", "Lỗi ngày tháng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DateTime date = dtpNgayThiCa.Value.Date;
                TimeSpan tBatDau = TimeSpan.Parse(txtGioBatDau.Text);
                TimeSpan tKetThuc = TimeSpan.Parse(txtGioKetThuc.Text);

                if (tKetThuc <= tBatDau)
                {
                    MessageBox.Show("Giờ kết thúc phải sau giờ bắt đầu!");
                    return;
                }

                DateTime dtBatDau = date.Add(tBatDau);
                DateTime dtKetThuc = date.Add(tKetThuc);

                string maKy = trvDanhSach.SelectedNode.Level == 0 ? trvDanhSach.SelectedNode.Tag.ToString() : trvDanhSach.SelectedNode.Parent.Tag.ToString();
                bool isUpdate = trvDanhSach.SelectedNode.Level == 1;
                string maCa = isUpdate ? trvDanhSach.SelectedNode.Tag.ToString() : "";

                string q = isUpdate ?
                    @"UPDATE CaThi SET MaPhong=@Phong, NgayThi=@Ngay, ThoiGianBatDau=@BatDau, ThoiGianKetThuc=@KetThuc, SiSo=@SiSo WHERE MaCaThi=@MaCa" :
                    @"INSERT INTO CaThi (MaKyThi, MaPhong, NgayThi, ThoiGianBatDau, ThoiGianKetThuc, SiSo) VALUES (@MaKy, @Phong, @Ngay, @BatDau, @KetThuc, @SiSo)";

                List<SqlParameter> lst = new List<SqlParameter> {
                    new SqlParameter("@Phong", cboPhongThi.SelectedValue),
                    new SqlParameter("@Ngay", date),
                    new SqlParameter("@BatDau", dtBatDau),
                    new SqlParameter("@KetThuc", dtKetThuc),
                    new SqlParameter("@SiSo", sisoNhap)
                };

                if (isUpdate) lst.Add(new SqlParameter("@MaCa", maCa));
                else lst.Add(new SqlParameter("@MaKy", maKy));

                DatabaseConnection.ExecuteNonQuery(q, lst.ToArray());
                MessageBox.Show(isUpdate ? "Cập nhật Ca thi thành công!" : "Thêm Ca thi mới thành công!");
                LoadTreeView();
            }
            catch (FormatException) { MessageBox.Show("Giờ nhập không đúng định dạng HH:mm (VD: 08:30)"); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // --- NÚT THÊM KỲ THI ---
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKyThi.Text)) { MessageBox.Show("Tên kỳ thi không được để trống!"); return; }

            // Kiểm tra lệ phí không âm
            if (!decimal.TryParse(txtLePhi.Text, out decimal phi) || phi < 0)
            {
                MessageBox.Show("Lệ phí không được nhỏ hơn 0!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra ngày tổ chức không phải quá khứ
            if (dtpNgayToChuc.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Ngày tổ chức không được là ngày trong quá khứ!", "Lỗi ngày tháng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string q = "INSERT INTO KyThi (TenKyThi, NgayToChuc, LePhi, TrangThai) VALUES (@Ten, @Ngay, @Phi, @TT)";
                SqlParameter[] p = {
                    new SqlParameter("@Ten", txtTenKyThi.Text.Trim()),
                    new SqlParameter("@Ngay", dtpNgayToChuc.Value.Date),
                    new SqlParameter("@Phi", phi),
                    new SqlParameter("@TT", cboTrangThai.Text)
                };

                DatabaseConnection.ExecuteNonQuery(q, p);
                MessageBox.Show("Thêm kỳ thi thành công!");
                LoadTreeView();
                ClearInputKyThi();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (trvDanhSach.SelectedNode == null || trvDanhSach.SelectedNode.Level != 0)
            {
                MessageBox.Show("Chọn kỳ thi cần sửa trên TreeView!");
                return;
            }

            if (!decimal.TryParse(txtLePhi.Text, out decimal phi) || phi < 0)
            {
                MessageBox.Show("Lệ phí không hợp lệ!");
                return;
            }

            try
            {
                string id = trvDanhSach.SelectedNode.Tag.ToString();
                string q = "UPDATE KyThi SET TenKyThi=@Ten, NgayToChuc=@Ngay, LePhi=@Phi, TrangThai=@TT WHERE MaKyThi=@ID";
                SqlParameter[] p = {
                    new SqlParameter("@Ten", txtTenKyThi.Text.Trim()),
                    new SqlParameter("@Ngay", dtpNgayToChuc.Value.Date),
                    new SqlParameter("@Phi", phi),
                    new SqlParameter("@TT", cboTrangThai.Text),
                    new SqlParameter("@ID", id)
                };

                DatabaseConnection.ExecuteNonQuery(q, p);
                MessageBox.Show("Cập nhật kỳ thi thành công!");
                LoadTreeView();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (trvDanhSach.SelectedNode == null || trvDanhSach.SelectedNode.Level != 0) return;
            if (MessageBox.Show("Xóa kỳ thi sẽ xóa toàn bộ ca thi và dữ liệu liên quan. Tiếp tục?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    string id = trvDanhSach.SelectedNode.Tag.ToString();
                    // Lưu ý: Nên xóa CaThi trước (nếu DB không để Cascade Delete)
                    DatabaseConnection.ExecuteNonQuery("DELETE FROM CaThi WHERE MaKyThi = @ID", new SqlParameter[] { new SqlParameter("@ID", id) });
                    DatabaseConnection.ExecuteNonQuery("DELETE FROM KyThi WHERE MaKyThi = @ID", new SqlParameter[] { new SqlParameter("@ID", id) });
                    MessageBox.Show("Đã xóa kỳ thi!");
                    LoadTreeView();
                    ClearInputKyThi();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        private void btnXoaCa_Click(object sender, EventArgs e)
        {
            if (trvDanhSach.SelectedNode != null && trvDanhSach.SelectedNode.Level == 1)
            {
                if (MessageBox.Show("Xóa ca thi này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        string id = trvDanhSach.SelectedNode.Tag.ToString();
                        DatabaseConnection.ExecuteNonQuery("DELETE FROM CaThi WHERE MaCaThi = @ID", new SqlParameter[] { new SqlParameter("@ID", id) });
                        MessageBox.Show("Đã xóa ca thi!");
                        LoadTreeView();
                        ClearInputCaThi();
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
                }
            }
        }

        private void ClearInputKyThi()
        {
            txtTenKyThi.Clear();
            txtLePhi.Clear();
            dtpNgayToChuc.Value = DateTime.Now;
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;
            txtTenKyThi.Focus();
        }

        private void ClearInputCaThi()
        {
            dtpNgayThiCa.Value = DateTime.Now;
            txtGioBatDau.Clear();
            txtGioKetThuc.Clear();
            txtSiSo.Clear();
            if (cboPhongThi.Items.Count > 0) cboPhongThi.SelectedIndex = 0;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearInputKyThi();
            ClearInputCaThi();
            LoadTreeView();
        }
    }
}