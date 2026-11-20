using Nhom09_QLyNgoaiNgu.Admin;
using System;
using System.Windows.Forms;

namespace Nhom09_QLyNgoaiNgu.HocVien
{
    public partial class frmHocVien : Form
    {
        private int _maTaiKhoanHienTai;

        public frmHocVien(int maTaiKhoan)
        {
            InitializeComponent();

            this._maTaiKhoanHienTai = maTaiKhoan;
        }

        private void btnLichThi_Click(object sender, EventArgs e)
        {
            frmLichThi f = new frmLichThi(_maTaiKhoanHienTai);

            this.Hide();      
            f.ShowDialog();   
            this.Show();      
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            frmThanhToan f = new frmThanhToan(_maTaiKhoanHienTai);
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void btnKetQua_Click(object sender, EventArgs e)
        {
            frmKetQua f = new frmKetQua();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void btnHoSo_Click(object sender, EventArgs e)
        {
            frmHoSo f = new frmHoSo();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                this.Hide();

                frmDangNhap login = new frmDangNhap();
                login.ShowDialog();

                this.Close();
            }
        }

        private void frmHocVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnQuyDinh_Click(object sender, EventArgs e)
        {
            Nhom09_QLyNgoaiNgu.Admin.frmQuyDinh fQuydinh = new Nhom09_QLyNgoaiNgu.Admin.frmQuyDinh();
            fQuydinh.ShowDialog();

        }

        private void fpl_Chonkythi_SizeChanged(object sender, EventArgs e)
        {
            int cardWidth = 250;

            int margin = 20;    

            int totalCards = 5;

            int totalContentWidth = (cardWidth + margin) * totalCards;

            int panelWidth = sender is FlowLayoutPanel panel ? panel.ClientSize.Width : ((Control)sender).ClientSize.Width;

            int paddingLeft = (panelWidth - totalContentWidth) / 2;

            if (paddingLeft > 0)
            {
                ((FlowLayoutPanel)sender).Padding = new Padding(paddingLeft, 0, 0, 0);
            }
            else
            {
                ((FlowLayoutPanel)sender).Padding = new Padding(0, 0, 0, 0);
            }
        }

        private void btnChonIELTS_Click(object sender, EventArgs e)
        {
            frmChiTietBaiThi frm = new frmChiTietBaiThi("IELTS");

 
            this.Hide(); 
            frm.ShowDialog(); 
            this.Show(); 
        }

        private void btnChonTOEIC_Click(object sender, EventArgs e)
        {
            frmChiTietBaiThi frm = new frmChiTietBaiThi("TOEIC");
            this.Hide();
            frm.ShowDialog();
            this.Show();
        }

        private void btnChonVSTEP_Click(object sender, EventArgs e)
        {
            frmChiTietBaiThi frm = new frmChiTietBaiThi("VSTEP");
            this.Hide();
            frm.ShowDialog();
            this.Show();
        }

        private void btnChonKET_Click(object sender, EventArgs e)
        {
            frmChiTietBaiThi frm = new frmChiTietBaiThi("KET");

            this.Hide();
            frm.ShowDialog();
            this.Show();
        }

        private void btnChonPET_Click(object sender, EventArgs e)
        {
            frmChiTietBaiThi frm = new frmChiTietBaiThi("PET");

            this.Hide();
            frm.ShowDialog();
            this.Show();
        }

        private void btnQuyDinhChungFooter_Click(object sender, EventArgs e)
        {
            Nhom09_QLyNgoaiNgu.Admin.frmQuyDinh fQuydinh = new Nhom09_QLyNgoaiNgu.Admin.frmQuyDinh();
            fQuydinh.ShowDialog();
        }

        private void btnLichThiFooter_Click(object sender, EventArgs e)
        {
            frmLichThi f = new frmLichThi(_maTaiKhoanHienTai);

            this.Hide();     
            f.ShowDialog(); 
            this.Show();
        }

        private void btnThanhToanFooter_Click(object sender, EventArgs e)
        {
            frmThanhToan f = new frmThanhToan(_maTaiKhoanHienTai);
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void btnKetQuaFooter_Click(object sender, EventArgs e)
        {
            frmKetQua f = new frmKetQua();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }
    }
}