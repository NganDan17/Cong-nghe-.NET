using Microsoft.Data.SqlClient; 
using Microsoft.Extensions.Logging;
using Nhom09_QLyNgoaiNgu;      
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmQLChungChi : Form
    {
        public frmQLChungChi()
        {
            InitializeComponent();
            ConfigGrids();
        }

        private void frmQLChungChi_Load_1(object sender, EventArgs e)
        {
            LoadCboKyThi();
        }

        private void ConfigGrids()
        {
            dgvChoCap.AutoGenerateColumns = false;
            dgvChoCap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvDaCap.AutoGenerateColumns = false;
            dgvDaCap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadCboKyThi()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT MaKyThi, TenKyThi FROM KyThi ORDER BY NgayToChuc DESC";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["MaKyThi"] = -1;
                        dr["TenKyThi"] = "--- Không có dữ liệu kỳ thi ---";
                        dt.Rows.InsertAt(dr, 0);
                    }

                    cboKyThi.DisplayMember = "TenKyThi";
                    cboKyThi.ValueMember = "MaKyThi";
                    cboKyThi.DataSource = dt;

                    cboKyThi.SelectedIndexChanged -= CboKyThi_SelectedIndexChanged; 
                    cboKyThi.SelectedIndexChanged += CboKyThi_SelectedIndexChanged; 

                    if (cboKyThi.SelectedValue != null && (int)cboKyThi.SelectedValue != -1)
                    {
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải danh sách kỳ thi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CboKyThi_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (cboKyThi.SelectedValue == null) return;

            int maKyThi;
            if (!int.TryParse(cboKyThi.SelectedValue.ToString(), out maKyThi) || maKyThi == -1) return;

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd1 = new SqlCommand("sp_LayDSChoCapChungChi", conn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@MaKyThi", maKyThi);

                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);

                    if (dgvChoCap.Columns.Contains("colMaKetQua"))
                    {
                        dgvChoCap.Columns["colMaKetQua"].DataPropertyName = "MaKetQua";
                    }

                    if (!dt1.Columns.Contains("SoHieuMoi")) dt1.Columns.Add("SoHieuMoi", typeof(string));

                    dgvChoCap.DataSource = dt1;

                    SqlCommand cmd2 = new SqlCommand("sp_LayDSDaCapChungChi", conn);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@MaKyThi", maKyThi);

                    SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);
                    dgvDaCap.DataSource = dt2;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
                }
            }
        }

        private void btnCapSoHieu_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng này đã bị tắt. Vui lòng nhập số hiệu trực tiếp vào bảng.", "Thông báo");
        }


        private void btnLuuChungChi_Click_1(object sender, EventArgs e)
        {
            if (dgvChoCap.Rows.Count == 0) return;

            bool check = false;
            foreach (DataGridViewRow r in dgvChoCap.Rows)
            {
                if (r.IsNewRow) continue;
                if (r.Cells["colSoHieuMoi"].Value != null && !string.IsNullOrWhiteSpace(r.Cells["colSoHieuMoi"].Value.ToString()))
                {
                    check = true;
                    break;
                }
            }

            if (!check)
            {
                MessageBox.Show("Vui lòng nhập Số hiệu chứng chỉ vào cột 'Số hiệu (Mới)'!", "Chưa nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    int count = 0;

                    foreach (DataGridViewRow row in dgvChoCap.Rows)
                    {
                        if (row.IsNewRow) continue;

                        DataRowView drv = row.DataBoundItem as DataRowView;
                        if (drv == null) continue;

                        if (!drv.Row.Table.Columns.Contains("MaKetQua"))
                        {
                            string cacCotCoSan = "";
                            foreach (DataColumn col in drv.Row.Table.Columns) cacCotCoSan += col.ColumnName + ", ";

                            MessageBox.Show($"LỖI: Trong dữ liệu tải về không có cột 'MaKetQua'.\nCác cột hiện có là: {cacCotCoSan}\n\n-> Hãy kiểm tra lại câu lệnh SQL SELECT.", "Thiếu cột");
                            return;
                        }

                        var valMa = drv["MaKetQua"];

                        var valSo = row.Cells["colSoHieuMoi"].Value;

                        if (valSo != null && !string.IsNullOrWhiteSpace(valSo.ToString()))
                        {
                            if (valMa == DBNull.Value || valMa == null)
                            {
                                MessageBox.Show($"Dữ liệu Mã (ID) bị Rỗng tại dòng học viên: {drv["HoTen"]}. Vui lòng kiểm tra lại CSDL.", "Lỗi dữ liệu");
                                continue;
                            }

                            SqlCommand cmd = new SqlCommand("sp_CapChungChi", conn);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@MaKetQua", Convert.ToInt32(valMa));
                            cmd.Parameters.AddWithValue("@SoChungChi", valSo.ToString());

                            cmd.ExecuteNonQuery();
                            XemNhatKy.Record($"Cấp chứng chỉ số hiệu {valSo} cho học viên ID: {valMa}");
                            count++;
                        }
                    }

                    if (count > 0)
                    {
                        MessageBox.Show($"Đã lưu thành công {count} chứng chỉ!", "Thành công");
                        LoadData(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnInDanhSach_Click(object sender, EventArgs e)
        {
            if (dgvDaCap.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trên bảng để in!");
                return;
            }

            IndsChungChi ds = new IndsChungChi();

            foreach (DataGridViewRow row in dgvDaCap.Rows)
            {
                if (row.IsNewRow) continue;

                string hoTen = row.Cells["collHoTen"].Value?.ToString();
                string maCC = ""; 
                string soCC = row.Cells["colSoChungChi"].Value?.ToString();
                string ngayCap = row.Cells["colNgayCap"].Value?.ToString();
                string trangThai = row.Cells["colTrangThai"].Value?.ToString();

                if (trangThai != null && trangThai.Contains("Đã cấp"))
                {
                    ds.dtChungChi.AdddtChungChiRow(hoTen, maCC, soCC, ngayCap, trangThai);
                }
            }

            if (ds.dtChungChi.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu 'Đã cấp' để in!");
                return;
            }

            try
            {
                InChungChi rpt = new InChungChi();

                rpt.SetDataSource(ds);

                frmInBaoCao f = new frmInBaoCao();
                f.crystalReportViewer1.ReportSource = rpt;
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị: " + ex.Message);
            }
        }
    }
}