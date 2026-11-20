using System;
using System.Windows.Forms;

namespace Nhom09_QLyNgoaiNgu.HocVien
{
    public partial class frmChiTietBaiThi : Form
    {
        string loaiChungChi;

        public frmChiTietBaiThi(string tenChungChi)
        {
            InitializeComponent();

            this.loaiChungChi = tenChungChi;
        }

        private void frmChiTietBaiThi_Load(object sender, EventArgs e)
        {
            lblTieuDe.Text = "THÔNG TIN KỲ THI " + this.loaiChungChi;

            switch (this.loaiChungChi)
            {
                case "IELTS":
                    lblGiaTien.Text = "4.750.000 VNĐ";
                    txtMoTa.Text = "Bài thi đánh giá 4 kỹ năng Nghe, Nói, Đọc, Viết.\nChứng chỉ có giá trị quốc tế rộng rãi nhất.";
                    break;

                case "TOEIC":
                    lblGiaTien.Text = "1.500.000 VNĐ";
                    txtMoTa.Text = "Bài thi trắc nghiệm Nghe và Đọc.\nTiêu chuẩn tuyển dụng của nhiều doanh nghiệp.";
                    break;

                case "VSTEP":
                    lblTieuDe.Text = "KỲ THI NĂNG LỰC NGOẠI NGỮ 6 BẬC (VSTEP)";
                    lblGiaTien.Text = "1.800.000 VNĐ";
                    txtMoTa.Text = "Chứng chỉ nội địa Việt Nam (B1, B2, C1).\nDành cho giáo viên, thạc sĩ, công chức.";
                    break;

                case "KET":
                    lblTieuDe.Text = "CAMBRIDGE ENGLISH: KEY (KET - A2)";
                    lblGiaTien.Text = "1.200.000 VNĐ";
                    txtMoTa.Text = "Chứng chỉ sơ cấp A2 dành cho học sinh THCS.";
                    break;

                case "PET":
                    lblTieuDe.Text = "CAMBRIDGE ENGLISH: PRELIMINARY (PET - B1)";
                    lblGiaTien.Text = "1.250.000 VNĐ";
                    txtMoTa.Text = "Chứng chỉ trung cấp B1. Được miễn thi tốt nghiệp/lớp 10 tại một số nơi.";
                    break;

                default:
                    lblGiaTien.Text = "Liên hệ";
                    txtMoTa.Text = "Chưa có thông tin cho loại chứng chỉ này.";
                    break;
            }
        }
        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}