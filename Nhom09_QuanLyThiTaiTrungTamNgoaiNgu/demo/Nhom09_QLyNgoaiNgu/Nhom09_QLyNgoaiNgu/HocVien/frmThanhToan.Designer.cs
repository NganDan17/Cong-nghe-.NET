namespace Nhom09_QLyNgoaiNgu.HocVien
{
    partial class frmThanhToan
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabCanDong = new System.Windows.Forms.TabPage();
            this.dgvCanDong = new System.Windows.Forms.DataGridView();
            this.colMaDK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTenKyThi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSoTien = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnThanhToan = new System.Windows.Forms.Button();
            this.cboPhuongThuc = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabLichSu = new System.Windows.Forms.TabPage();
            this.dgvLichSu = new System.Windows.Forms.DataGridView();
            this.colKyThi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDaDong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgayThanhToan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhuongThuc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TabControl.SuspendLayout();
            this.tabCanDong.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCanDong)).BeginInit();
            this.tabLichSu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLichSu)).BeginInit();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.tabCanDong);
            this.TabControl.Controls.Add(this.tabLichSu);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(1077, 594);
            this.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.TabControl.TabIndex = 0;
            // 
            // tabCanDong
            // 
            this.tabCanDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(205)))), ((int)(((byte)(216)))));
            this.tabCanDong.Controls.Add(this.dgvCanDong);
            this.tabCanDong.Controls.Add(this.btnThanhToan);
            this.tabCanDong.Controls.Add(this.cboPhuongThuc);
            this.tabCanDong.Controls.Add(this.label1);
            this.tabCanDong.Location = new System.Drawing.Point(4, 25);
            this.tabCanDong.Name = "tabCanDong";
            this.tabCanDong.Padding = new System.Windows.Forms.Padding(3);
            this.tabCanDong.Size = new System.Drawing.Size(1069, 565);
            this.tabCanDong.TabIndex = 0;
            this.tabCanDong.Text = "Lệ phí cần đóng";
            // 
            // dgvCanDong
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(221)))), ((int)(((byte)(87)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvCanDong.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCanDong.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(205)))), ((int)(((byte)(216)))));
            this.dgvCanDong.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(33)))), ((int)(((byte)(63)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCanDong.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCanDong.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCanDong.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMaDK,
            this.colTenKyThi,
            this.colSoTien});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCanDong.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCanDong.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvCanDong.EnableHeadersVisualStyles = false;
            this.dgvCanDong.Location = new System.Drawing.Point(3, 3);
            this.dgvCanDong.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.dgvCanDong.Name = "dgvCanDong";
            this.dgvCanDong.RowHeadersWidth = 51;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            this.dgvCanDong.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCanDong.RowTemplate.Height = 24;
            this.dgvCanDong.Size = new System.Drawing.Size(1063, 219);
            this.dgvCanDong.TabIndex = 12;
            // 
            // colMaDK
            // 
            this.colMaDK.DataPropertyName = "MaDangKy";
            this.colMaDK.HeaderText = "Mã ĐK";
            this.colMaDK.MinimumWidth = 6;
            this.colMaDK.Name = "colMaDK";
            this.colMaDK.Visible = false;
            this.colMaDK.Width = 125;
            // 
            // colTenKyThi
            // 
            this.colTenKyThi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTenKyThi.DataPropertyName = "TenKyThi";
            this.colTenKyThi.HeaderText = "Tên kỳ thi";
            this.colTenKyThi.MinimumWidth = 6;
            this.colTenKyThi.Name = "colTenKyThi";
            // 
            // colSoTien
            // 
            this.colSoTien.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colSoTien.DataPropertyName = "LePhi";
            this.colSoTien.HeaderText = "Số tiền";
            this.colSoTien.MinimumWidth = 6;
            this.colSoTien.Name = "colSoTien";
            // 
            // btnThanhToan
            // 
            this.btnThanhToan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThanhToan.BackColor = System.Drawing.Color.OrangeRed;
            this.btnThanhToan.ForeColor = System.Drawing.Color.White;
            this.btnThanhToan.Location = new System.Drawing.Point(793, 487);
            this.btnThanhToan.Name = "btnThanhToan";
            this.btnThanhToan.Size = new System.Drawing.Size(253, 57);
            this.btnThanhToan.TabIndex = 3;
            this.btnThanhToan.Text = "THANH TOÁN NGAY";
            this.btnThanhToan.UseVisualStyleBackColor = false;
            this.btnThanhToan.Click += new System.EventHandler(this.btnThanhToan_Click);
            // 
            // cboPhuongThuc
            // 
            this.cboPhuongThuc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPhuongThuc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPhuongThuc.FormattingEnabled = true;
            this.cboPhuongThuc.Items.AddRange(new object[] {
            "Chuyển khoản QR",
            "Ví Momo",
            "Thẻ tín dụng"});
            this.cboPhuongThuc.Location = new System.Drawing.Point(497, 416);
            this.cboPhuongThuc.Name = "cboPhuongThuc";
            this.cboPhuongThuc.Size = new System.Drawing.Size(408, 24);
            this.cboPhuongThuc.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(174, 409);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(293, 31);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hình thức thanh toán:";
            // 
            // tabLichSu
            // 
            this.tabLichSu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(205)))), ((int)(((byte)(216)))));
            this.tabLichSu.Controls.Add(this.dgvLichSu);
            this.tabLichSu.Location = new System.Drawing.Point(4, 25);
            this.tabLichSu.Name = "tabLichSu";
            this.tabLichSu.Padding = new System.Windows.Forms.Padding(3);
            this.tabLichSu.Size = new System.Drawing.Size(1069, 565);
            this.tabLichSu.TabIndex = 1;
            this.tabLichSu.Text = "Lịch sử giao dịch";
            // 
            // dgvLichSu
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(221)))), ((int)(((byte)(87)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvLichSu.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvLichSu.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(205)))), ((int)(((byte)(216)))));
            this.dgvLichSu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(33)))), ((int)(((byte)(63)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLichSu.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvLichSu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLichSu.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colKyThi,
            this.colDaDong,
            this.colNgayThanhToan,
            this.colPhuongThuc});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLichSu.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvLichSu.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvLichSu.EnableHeadersVisualStyles = false;
            this.dgvLichSu.Location = new System.Drawing.Point(3, 3);
            this.dgvLichSu.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.dgvLichSu.Name = "dgvLichSu";
            this.dgvLichSu.RowHeadersWidth = 51;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black;
            this.dgvLichSu.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvLichSu.RowTemplate.Height = 24;
            this.dgvLichSu.Size = new System.Drawing.Size(1063, 377);
            this.dgvLichSu.TabIndex = 13;
            // 
            // colKyThi
            // 
            this.colKyThi.DataPropertyName = "TenKyThi";
            this.colKyThi.HeaderText = "Kỳ thi";
            this.colKyThi.MinimumWidth = 6;
            this.colKyThi.Name = "colKyThi";
            this.colKyThi.Width = 125;
            // 
            // colDaDong
            // 
            this.colDaDong.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDaDong.DataPropertyName = "SoTien";
            this.colDaDong.HeaderText = "Đã đóng";
            this.colDaDong.MinimumWidth = 6;
            this.colDaDong.Name = "colDaDong";
            // 
            // colNgayThanhToan
            // 
            this.colNgayThanhToan.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colNgayThanhToan.DataPropertyName = "NgayThanhToan";
            this.colNgayThanhToan.HeaderText = "Ngày giao dịch";
            this.colNgayThanhToan.MinimumWidth = 6;
            this.colNgayThanhToan.Name = "colNgayThanhToan";
            // 
            // colPhuongThuc
            // 
            this.colPhuongThuc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPhuongThuc.DataPropertyName = "PhuongThuc";
            this.colPhuongThuc.HeaderText = "Hình thức thanh toán";
            this.colPhuongThuc.MinimumWidth = 6;
            this.colPhuongThuc.Name = "colPhuongThuc";
            // 
            // frmThanhToan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1077, 594);
            this.Controls.Add(this.TabControl);
            this.Name = "frmThanhToan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thanh Toán Lệ Phí";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmThanhToan_Load);
            this.TabControl.ResumeLayout(false);
            this.tabCanDong.ResumeLayout(false);
            this.tabCanDong.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCanDong)).EndInit();
            this.tabLichSu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLichSu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage tabCanDong;
        private System.Windows.Forms.TabPage tabLichSu;
        private System.Windows.Forms.Button btnThanhToan;
        private System.Windows.Forms.ComboBox cboPhuongThuc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvCanDong;
        private System.Windows.Forms.DataGridView dgvLichSu;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaDK;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTenKyThi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSoTien;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKyThi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDaDong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgayThanhToan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhuongThuc;
    }
}