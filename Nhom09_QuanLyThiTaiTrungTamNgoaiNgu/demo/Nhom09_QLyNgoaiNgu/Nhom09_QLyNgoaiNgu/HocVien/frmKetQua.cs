using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Nhom09_QLyNgoaiNgu;

namespace Nhom09_QLyNgoaiNgu.HocVien
{
    public partial class frmKetQua : Form
    {
        private int maThiSinhHienTai = -1;

        public frmKetQua()
        {
            InitializeComponent();
        }

        public frmKetQua(int maThiSinh)
        {
            InitializeComponent();
            this.maThiSinhHienTai = maThiSinh;
        }

        private void frmKetQua_Load(object sender, EventArgs e)
        {
            if (this.maThiSinhHienTai == -1)
            {
                this.maThiSinhHienTai = LayMaThiSinhTuTaiKhoan(XemNhatKy.CurrentUserId);
            }

            if (this.maThiSinhHienTai != -1)
            {
                TaiDuLieuKetQua();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin thí sinh trùng khớp với Email tài khoản này.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private int LayMaThiSinhTuTaiKhoan(int maTaiKhoan)
        {
            int maTS = -1;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();

                    string sql = @"
                        SELECT ts.MaThiSinh 
                        FROM ThiSinh ts
                        JOIN TaiKhoan tk ON ts.Email = tk.Email 
                        WHERE tk.MaTaiKhoan = @MaTK";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaTK", maTaiKhoan);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        maTS = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lấy thông tin (Kiểm tra cột Email): " + ex.Message);
                }
            }
            return maTS;
        }

        private void TaiDuLieuKetQua()
        {
            if (flpDanhSachKetQua == null) return;

            flpDanhSachKetQua.Controls.Clear();
            flpDanhSachKetQua.AutoScroll = true;

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sqlQuery = "EXEC sp_LayKetQuaThiSinh @MaThiSinh";
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@MaThiSinh", maThiSinhHienTai);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        HienThiThongBaoRong();
                        return;
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        TaoTheKetQua(row);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void TaoTheKetQua(DataRow row)
        {
            string tenKyThi = row["TenChungChi"].ToString();

            bool daCoDiem = row["DiemNghe"] != DBNull.Value;

            string ngayThi = row["NgayThi"] != DBNull.Value
                ? Convert.ToDateTime(row["NgayThi"]).ToString("dd/MM/yyyy")
                : "Chưa xếp lịch";

            string diemNghe = daCoDiem ? row["DiemNghe"].ToString() : "-";
            string diemNoi = daCoDiem ? row["DiemNoi"].ToString() : "-";
            string diemDoc = daCoDiem ? row["DiemDoc"].ToString() : "-";
            string diemViet = daCoDiem ? row["DiemViet"].ToString() : "-";
            string tongDiem = daCoDiem ? row["TongDiem"].ToString() : "--";

            Panel pnlCard = new Panel();
            pnlCard.Width = flpDanhSachKetQua.Width - 30;
            pnlCard.Height = 130; 
            pnlCard.BackColor = Color.White;
            pnlCard.Margin = new Padding(5, 5, 5, 15);
            pnlCard.BorderStyle = BorderStyle.FixedSingle;

            // Tên kỳ thi
            Label lblTen = new Label();
            lblTen.Text = tenKyThi;
            lblTen.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTen.ForeColor = Color.FromArgb(0, 51, 102);
            lblTen.AutoSize = true;
            lblTen.Location = new Point(15, 15);
            pnlCard.Controls.Add(lblTen);

            // Ngày thi
            Label lblNgay = new Label();
            lblNgay.Text = "Ngày thi: " + ngayThi;
            lblNgay.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblNgay.ForeColor = Color.Gray;
            lblNgay.AutoSize = true;
            lblNgay.Location = new Point(17, 45);
            pnlCard.Controls.Add(lblNgay);

            // Điểm chi tiết
            Label lblChiTiet = new Label();
            lblChiTiet.Text = $"Nghe: {diemNghe}   |   Nói: {diemNoi}   |   Đọc: {diemDoc}   |   Viết: {diemViet}";
            lblChiTiet.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lblChiTiet.ForeColor = daCoDiem ? Color.Black : Color.DimGray;
            lblChiTiet.AutoSize = true;
            lblChiTiet.Location = new Point(17, 80);
            pnlCard.Controls.Add(lblChiTiet);

            // Tổng điểm 
            Label lblTong = new Label();
            lblTong.Text = "Tổng: " + tongDiem;
            lblTong.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTong.ForeColor = Color.DarkRed; 
            lblTong.AutoSize = true;
            lblTong.Location = new Point(pnlCard.Width - 140, 45);
            lblTong.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlCard.Controls.Add(lblTong);

            flpDanhSachKetQua.Controls.Add(pnlCard);
        }

        private void HienThiThongBaoRong()
        {
            Label lbl = new Label();
            lbl.Text = "Bạn chưa tham gia kỳ thi nào.";
            lbl.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            lbl.ForeColor = Color.Gray;
            lbl.AutoSize = true;
            lbl.Margin = new Padding(20);
            flpDanhSachKetQua.Controls.Add(lbl);
        }
    }
}