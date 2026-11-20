namespace Nhom09_QLyNgoaiNgu.HocVien
{
    partial class frmKetQua
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flpDanhSachKetQua = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flpDanhSachKetQua
            // 
            this.flpDanhSachKetQua.AutoScroll = true;
            this.flpDanhSachKetQua.AutoSize = true;
            this.flpDanhSachKetQua.BackColor = System.Drawing.Color.Gainsboro;
            this.flpDanhSachKetQua.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpDanhSachKetQua.Location = new System.Drawing.Point(0, 0);
            this.flpDanhSachKetQua.Name = "flpDanhSachKetQua";
            this.flpDanhSachKetQua.Size = new System.Drawing.Size(1153, 589);
            this.flpDanhSachKetQua.TabIndex = 0;
            // 
            // frmKetQua
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 589);
            this.Controls.Add(this.flpDanhSachKetQua);
            this.Name = "frmKetQua";
            this.Text = "frmKetQua";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmKetQua_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpDanhSachKetQua;
    }
}