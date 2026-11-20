using Microsoft.Extensions.Logging;
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
    public partial class frmAdmin : Form
    {
        private int _maQuyen;

        public frmAdmin()
        {
            InitializeComponent();
            customizeDesing();
        }

        public frmAdmin(int maQuyen)
        {
            InitializeComponent();
            this._maQuyen = maQuyen;
            customizeDesing();

            PhanQuyenGiaoDien();
        }

        private void customizeDesing()
        {
            panelQLNDSubMenu.Visible = false;
            panelQLTCSubMenu.Visible = false;
            panelQLKQTCSubMenu.Visible = false;
            panelCHHTSubMenu.Visible = false;
            panelTKSubMenu.Visible = false;
        }

        private void hideSubMenu()
        {
            if (panelQLNDSubMenu.Visible == true) panelQLNDSubMenu.Visible = false;
            if (panelQLTCSubMenu.Visible == true) panelQLTCSubMenu.Visible = false;
            if (panelQLKQTCSubMenu.Visible == true) panelQLKQTCSubMenu.Visible = false;
            if (panelCHHTSubMenu.Visible == true) panelCHHTSubMenu.Visible = false;
            if (panelTKSubMenu.Visible == true) panelTKSubMenu.Visible = false;
        }

        private void showSubMenu(Panel submenu)
        {
            if (submenu.Visible == false)
            {
                hideSubMenu();
                submenu.Visible = true;
            }
            else
                submenu.Visible = false;
        }
        private void btn_QLTCu_Click(object sender, EventArgs e) => showSubMenu(panelQLTCSubMenu);
        private void btn_QLNDung_Click(object sender, EventArgs e) => showSubMenu(panelQLNDSubMenu);
        private void btn_QLKQua_TChinh_Click(object sender, EventArgs e) => showSubMenu(panelQLKQTCSubMenu);
        private void btn_CHinh_Click(object sender, EventArgs e) => showSubMenu(panelCHHTSubMenu);
        private void btn_TKhoan_Click(object sender, EventArgs e) => showSubMenu(panelTKSubMenu);

        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();

            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void btn_QLTSinh_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLThiSinh());
            hideSubMenu();
        }

        private void btn_QLTKhoan_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLAdmin());
            hideSubMenu();
        }

        private void btn_QLGThi_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLGiamThi());
            hideSubMenu();
        }

        private void btn_PCCThi_Click(object sender, EventArgs e)
        {
            openChildForm(new frmPhanCongCoiThi());
            hideSubMenu();
        }

        private void btn_QLDKPThi_Click(object sender, EventArgs e)
        {
            openChildForm(new frmXepPhongThi());
            hideSubMenu();
        }

        private void btn_QLKThi_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLKyThi());
            hideSubMenu();
        }

        private void btn_QLDThi_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLDiemThi());
            hideSubMenu();
        }

        private void btn_QLTToan_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLThanhToan());
            hideSubMenu();
        }

        private void btn_QLCChi_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLChungChi());
            hideSubMenu();
        }

        private void btn_QLPThi_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLPhongThi());
            hideSubMenu();
        }

        private void btn_QLQHan_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQLQuyenHan());
            hideSubMenu();
        }

        private void btn_NKHDong_Click(object sender, EventArgs e)
        {
            openChildForm(new frmNhatKyHoatDong());
            hideSubMenu();
        }

        private void btn_TTCNhan_Click(object sender, EventArgs e)
        {
            openChildForm(new frmTTCaNhan());
            hideSubMenu();
        }
        private void btn_QLQDinh_Click(object sender, EventArgs e)
        {
            openChildForm(new frmQuyDinh());
            hideSubMenu();
        }
        private void btn_DXuat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?",
                                              "Xác nhận đăng xuất",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    XemNhatKy.Record("Đăng xuất khỏi hệ thống");
                    XemNhatKy.CurrentUserId = 0;

                    this.Hide();

                    frmDangNhap frmLogin = new frmDangNhap();
                    frmLogin.ShowDialog();

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi đăng xuất: " + ex.Message);
                }
            }
        }

        private void PhanQuyenGiaoDien()
        {
            //cho Admin=1
            if (_maQuyen != 1)
            {
                // Các chức năng cấm giám thị sử dụng
                if (btn_QLGThi != null) btn_QLGThi.Enabled = false;
                if (btn_QLTSinh != null) btn_QLTSinh.Enabled = false;
                if (btn_QLTKhoan != null) btn_QLTKhoan.Enabled = false;
                if (btn_QLQHan != null) btn_QLQHan.Enabled = false;
                if (btn_NKHDong != null) btn_NKHDong.Enabled = false;
                if (btn_QLTCu != null) btn_QLTCu.Visible = false;
            }
        }

       
    } 
} 