using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmQLDiemThi : Form
    {
        private string currentLoaiChungChi = "";

        public frmQLDiemThi()
        {
            InitializeComponent();
            ConfigDataGridView();
        }

        private void frmQLDiemThi_Load(object sender, EventArgs e)
        {
            LoadCboKyThi();
        }

        private void ConfigDataGridView()
        {
            dgvBangDiem.AutoGenerateColumns = false;
            dgvBangDiem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Đảm bảo cột MaDangKy luôn tồn tại để xử lý dữ liệu ngầm
            if (dgvBangDiem.Columns["colMaDangKy"] == null)
            {
                dgvBangDiem.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colMaDangKy",
                    DataPropertyName = "MaDangKy",
                    Visible = false
                });
            }
        }

        private void LoadCboKyThi()
        {
            try
            {
                string sql = "SELECT MaKyThi, TenKyThi, LoaiChungChi FROM KyThi ORDER BY NgayToChuc DESC";
                DataTable dt = DatabaseConnection.GetDataTable(sql);

                cboKyThi.SelectedIndexChanged -= cboKyThi_SelectedIndexChanged; // Tạm ngắt event để tránh loop
                cboKyThi.DataSource = dt;
                cboKyThi.DisplayMember = "TenKyThi";
                cboKyThi.ValueMember = "MaKyThi";
                cboKyThi.SelectedIndexChanged += cboKyThi_SelectedIndexChanged;

                if (cboKyThi.Items.Count > 0) cboKyThi_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load kỳ thi: " + ex.Message); }
        }

        private void cboKyThi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboKyThi.SelectedItem == null) return;

            DataRowView row = (DataRowView)cboKyThi.SelectedItem;
            currentLoaiChungChi = row["LoaiChungChi"].ToString().ToUpper();

            SetupColumnsByCertificate();
            LoadBangDiem();
        }

        private void SetupColumnsByCertificate()
        {
            // TOEIC: Thường chỉ thi Nghe/Đọc (tổng điểm 990), ẩn Nói/Viết
            bool isTOEIC = currentLoaiChungChi.Contains("TOEIC");

            if (dgvBangDiem.Columns["colDiemNoi"] != null && dgvBangDiem.Columns["colDiemViet"] != null)
            {
                dgvBangDiem.Columns["colDiemNoi"].Visible = !isTOEIC;
                dgvBangDiem.Columns["colDiemViet"].Visible = !isTOEIC;
            }
        }

        private void LoadBangDiem()
        {
            if (cboKyThi.SelectedValue == null) return;
            int maKyThi = Convert.ToInt32(cboKyThi.SelectedValue);
            string tuKhoa = txtTimKiem.Text.Trim();

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_TimKiemKetQuaThi", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaKyThi", maKyThi);
                    cmd.Parameters.AddWithValue("@TuKhoa", tuKhoa);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvBangDiem.DataSource = dt;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải bảng điểm: " + ex.Message); }
        }

        private void dgvBangDiem_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string colName = dgvBangDiem.Columns[e.ColumnIndex].Name;
            if (colName.Contains("Diem"))
            {
                string val = e.FormattedValue.ToString();
                if (string.IsNullOrWhiteSpace(val)) return;

                if (!float.TryParse(val, out float num) || num < 0)
                {
                    e.Cancel = true;
                    MessageBox.Show("Vui lòng nhập số dương!");
                    return;
                }

                // Validation theo loại chứng chỉ
                if (currentLoaiChungChi.Contains("IELTS") && num > 9.0)
                {
                    e.Cancel = true; MessageBox.Show("Điểm IELTS tối đa là 9.0");
                }
                else if (currentLoaiChungChi.Contains("TOEIC") && num > 495)
                {
                    e.Cancel = true; MessageBox.Show("Điểm thành phần TOEIC tối đa là 495");
                }
                else if (currentLoaiChungChi.Contains("VSTEP") && num > 10.0)
                {
                    e.Cancel = true; MessageBox.Show("Điểm VSTEP tối đa là 10.0");
                }
            }
        }

        private void dgvBangDiem_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CalculateTotalScore(e.RowIndex);
        }

        private void CalculateTotalScore(int rowIndex)
        {
            float nghe = GetScore(rowIndex, "colDiemNghe");
            float doc = GetScore(rowIndex, "colDiemDoc");
            float noi = GetScore(rowIndex, "colDiemNoi");
            float viet = GetScore(rowIndex, "colDiemViet");
            float tong = 0;

            if (currentLoaiChungChi.Contains("TOEIC"))
            {
                // TOEIC = Nghe + Đọc (Max 990)
                tong = nghe + doc;
            }
            else if (currentLoaiChungChi.Contains("IELTS"))
            {
                // IELTS = Trung bình cộng, làm tròn đến 0.5 gần nhất
                float avg = (nghe + doc + noi + viet) / 4;
                tong = (float)(Math.Round(avg * 2, MidpointRounding.AwayFromZero) / 2);
            }
            else if (currentLoaiChungChi.Contains("VSTEP"))
            {
                // VSTEP = Trung bình cộng, làm tròn đến 0.5
                tong = (float)Math.Round((nghe + doc + noi + viet) / 4, 1);
            }

            if (dgvBangDiem.Columns["colTongDiem"] != null)
                dgvBangDiem.Rows[rowIndex].Cells["colTongDiem"].Value = tong;
        }

        private float GetScore(int rowIndex, string colName)
        {
            if (dgvBangDiem.Columns[colName] == null || !dgvBangDiem.Columns[colName].Visible) return 0;
            var val = dgvBangDiem.Rows[rowIndex].Cells[colName].Value;
            return (val != null && float.TryParse(val.ToString(), out float res)) ? res : 0;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            dgvBangDiem.EndEdit();
            if (dgvBangDiem.Rows.Count == 0) return;

            int count = 0;
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    foreach (DataGridViewRow row in dgvBangDiem.Rows)
                    {
                        if (row.IsNewRow || row.Cells["colMaDangKy"].Value == null) continue;

                        int maDK = Convert.ToInt32(row.Cells["colMaDangKy"].Value);
                        float n = GetScore(row.Index, "colDiemNghe");
                        float d = GetScore(row.Index, "colDiemDoc");
                        float s = GetScore(row.Index, "colDiemNoi");
                        float w = GetScore(row.Index, "colDiemViet");
                        float t = GetScore(row.Index, "colTongDiem");

                        // Logic xét Kết quả & Xếp loại
                        string kq = "Trượt";
                        string xl = "";

                        if (currentLoaiChungChi.Contains("TOEIC"))
                        {
                            if (t >= 450) kq = "Đạt";
                            xl = t >= 785 ? "Gold" : (t >= 605 ? "Blue" : "Green");
                        }
                        else
                        {
                            if (t >= 4.0) kq = "Đạt";
                            xl = "Band " + t;
                        }

                        SqlCommand cmd = new SqlCommand("sp_CapNhatDiem", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MaDangKy", maDK);
                        cmd.Parameters.AddWithValue("@DiemNghe", n);
                        cmd.Parameters.AddWithValue("@DiemDoc", d);
                        cmd.Parameters.AddWithValue("@DiemNoi", s);
                        cmd.Parameters.AddWithValue("@DiemViet", w);
                        cmd.Parameters.AddWithValue("@TongDiem", t);
                        cmd.Parameters.AddWithValue("@KetQua", kq);
                        cmd.Parameters.AddWithValue("@XepLoai", xl);

                        cmd.ExecuteNonQuery();
                        count++;
                    }
                    MessageBox.Show($"Đã lưu thành công điểm cho {count} thí sinh!", "Thành công");
                    LoadBangDiem();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi lưu điểm: " + ex.Message); }
        }

        private void btnInBangDiem_Click_1(object sender, EventArgs e)
        {
            if (dgvBangDiem.Rows.Count == 0) { MessageBox.Show("Không có dữ liệu để in!"); return; }

            try
            {
                // Sử dụng chính DataTable từ DataGridView để in để đảm bảo tính đồng bộ
                DataTable dtIn = (DataTable)dgvBangDiem.DataSource;
                dsBangDiem dsReport = new dsBangDiem();

                foreach (DataRow rowSQL in dtIn.Rows)
                {
                    DataRow rowReport = dsReport.Tables["dtKetQua"].NewRow();
                    rowReport["SBD"] = rowSQL["SBD"];
                    rowReport["HoTen"] = rowSQL["Họ Tên"];
                    rowReport["CCCD"] = rowSQL["CCCD"];
                    rowReport["TongDiem"] = rowSQL["TongDiem"];
                    rowReport["KetQua"] = rowSQL["KetQua"];
                    rowReport["TenKyThi"] = cboKyThi.Text;
                    dsReport.Tables["dtKetQua"].Rows.Add(rowReport);
                }

                InBangDiem rpt = new InBangDiem();
                rpt.SetDataSource(dsReport);
                frmInBaoCao frmViewer = new frmInBaoCao();
                frmViewer.crystalReportViewer1.ReportSource = rpt;
                frmViewer.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi in báo cáo: " + ex.Message); }
        }

        private void btnTimKiem_Click(object sender, EventArgs e) => LoadBangDiem();

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            LoadBangDiem();
        }
    }
}