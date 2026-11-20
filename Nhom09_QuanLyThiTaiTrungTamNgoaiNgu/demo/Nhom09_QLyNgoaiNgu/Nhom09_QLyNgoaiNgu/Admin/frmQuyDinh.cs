using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Nhom09_QLyNgoaiNgu.Admin
{
    public partial class frmQuyDinh : Form
    {
        private Color mainBackgroundColor = Color.FromArgb(235, 245, 251);

        public frmQuyDinh()
        {
            InitializeComponent();
            SetupGiaoDien();
            LoadDuLieuVaoTab();
        }

        private void SetupGiaoDien()
        {
            this.BackColor = mainBackgroundColor;

            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(180, 50);
            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            if (e.Index < 0 || e.Index >= tabControl.TabPages.Count) return;

            TabPage tabPage = tabControl.TabPages[e.Index];
            Rectangle tabRect = tabControl.GetTabRect(e.Index);

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;


            Color bgColor = mainBackgroundColor;

            Color activeColor1 = Color.FromArgb(41, 128, 185); 
            Color activeColor2 = Color.FromArgb(109, 213, 250); 
            Color inactiveColor = Color.White;

            using (SolidBrush b = new SolidBrush(bgColor)) e.Graphics.FillRectangle(b, tabRect);

            Rectangle paddedRect = new Rectangle(tabRect.X + 4, tabRect.Y + 4, tabRect.Width - 8, tabRect.Height - 8);
            GraphicsPath path = new GraphicsPath();
            int r = 20;
            path.AddArc(paddedRect.X, paddedRect.Y, r, r, 180, 90);
            path.AddArc(paddedRect.Right - r, paddedRect.Y, r, r, 270, 90);
            path.AddArc(paddedRect.Right - r, paddedRect.Bottom - r, r, r, 0, 90);
            path.AddArc(paddedRect.X, paddedRect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();

            if (e.State == DrawItemState.Selected)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(paddedRect, activeColor1, activeColor2, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(inactiveColor)) e.Graphics.FillPath(brush, path);
                using (Pen pen = new Pen(Color.LightGray)) e.Graphics.DrawPath(pen, path);
            }

            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            Font font = (e.State == DrawItemState.Selected) ? new Font("Segoe UI", 11, FontStyle.Bold) : new Font("Segoe UI", 10, FontStyle.Regular);
            Color textColor = (e.State == DrawItemState.Selected) ? Color.White : Color.Gray;

            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(tabPage.Text, font, textBrush, tabRect, sf);
            }
        }

 
        private void LoadDuLieuVaoTab()
        {
            tabControl1.TabPages.Clear();

            foreach (var item in ExamData.Data)
            {
                string title = item.Key;
                string content = item.Value;

                TabPage page = new TabPage(title);
                page.BackColor = mainBackgroundColor;

                Panel pnlPadding = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), BackColor = mainBackgroundColor };

                RichTextBox rtb = new RichTextBox();
                rtb.Dock = DockStyle.Fill;
                rtb.BorderStyle = BorderStyle.None;
                rtb.BackColor = mainBackgroundColor;
                rtb.ReadOnly = true;
                rtb.ScrollBars = RichTextBoxScrollBars.Vertical;
                rtb.Font = new Font("Segoe UI", 11, FontStyle.Regular);
                rtb.ForeColor = Color.Black; 

                rtb.Text = content;
                FormatLines(rtb);
                HighlightKeywords(rtb);

                pnlPadding.Controls.Add(rtb);
                page.Controls.Add(pnlPadding);
                tabControl1.TabPages.Add(page);
            }
        }

        private void FormatLines(RichTextBox rtb)
        {
            int currentPos = 0;
            foreach (string line in rtb.Lines)
            {
                int len = line.Length;
                if (len == 0) { currentPos++; continue; }
                rtb.Select(currentPos, len);

                bool isHeader = (line.ToUpper() == line) && !line.Contains("•") && line.Trim().Length > 3;
                if (isHeader)
                {
                    rtb.SelectionFont = new Font("Segoe UI", 13, FontStyle.Bold);
                    rtb.SelectionColor = Color.FromArgb(0, 51, 102);
                    rtb.SelectionBackColor = Color.FromArgb(209, 236, 241);
                }
                else if (line.Trim().StartsWith("•"))
                {
                    int bulletIndex = line.IndexOf("•");
                    if (bulletIndex >= 0)
                    {
                        rtb.Select(currentPos + bulletIndex, 1);
                        rtb.SelectionColor = Color.FromArgb(255, 102, 0); 
                        rtb.SelectionFont = new Font("Segoe UI", 16, FontStyle.Bold);

                        rtb.Select(currentPos + bulletIndex + 1, len - bulletIndex - 1);
                        rtb.SelectionColor = Color.Black;
                        rtb.SelectionFont = new Font("Segoe UI", 11, FontStyle.Regular);
                    }
                }
                currentPos += len + 1;
            }
        }

        private void HighlightKeywords(RichTextBox rtb)
        {
            ApplyHighlight(rtb, @"\d+([.,]\d+)*(\+)?", Color.Red, true);
            ApplyHighlight(rtb, @"(Cấm|Lưu ý|Không|Quan trọng|Tuyệt đối)", Color.Red, true);
            ApplyHighlight(rtb, @"(IELTS|TOEIC|VSTEP|KET|PET|Cambridge)", Color.Blue, true);
            ApplyHighlight(rtb, @"(phút|điểm|kỹ năng)", Color.FromArgb(255, 128, 0), false);
        }

        private void ApplyHighlight(RichTextBox rtb, string pattern, Color foreColor, bool bold, Color? backColor = null)
        {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(rtb.Text);
            foreach (Match m in matches)
            {
                rtb.Select(m.Index, m.Length);
                rtb.SelectionColor = foreColor;
                if (bold) rtb.SelectionFont = new Font(rtb.SelectionFont, FontStyle.Bold);
                if (backColor.HasValue) rtb.SelectionBackColor = backColor.Value;
            }
        }
    }

    public static class ExamData
    {
        public static Dictionary<string, string> Data = new Dictionary<string, string>()
        {
             { "IELTS", @"IELTS LÀ GÌ VÀ CÓ TÁC DỤNG GÌ?
IELTS là chứng chỉ tiếng Anh quốc tế được công nhận tại hơn 11.000 tổ chức toàn cầu.
• Du học Anh, Úc, Mỹ, Canada, New Zealand.
• Xin học bổng (6.0 – 7.0).
• Định cư/di trú (IELTS General 5.0 – 7.0).
• Xin việc tại doanh nghiệp lớn, ngân hàng, tập đoàn quốc tế.

NỘI QUY PHÒNG THI IELTS
• Đến sớm 45 phút, mang CMND/CCCD và thẻ dự thi.
• Chỉ được mang: bút chì 2B, tẩy, bút bi xanh/đen.
• Cấm: điện thoại, smartwatch, giấy nháp, tài liệu.
• Kiểm tra tai nghe trong 2 phút đầu Listening.

QUY ĐỊNH ĐIỂM SỐ
• Thang điểm: 0.0 – 9.0
• 5.5: Mức sử dụng cơ bản.
• 6.0–6.5: Chuẩn du học.
• 7.0+: Học bổng, ngành khó.

LÝ DO CHỌN TRUNG TÂM
• Quy trình quốc tế, bảo mật cao.
• Tai nghe chuẩn Listening, phòng Speaking cách âm.
• Đăng ký – thanh toán – tra cứu kết quả trực tuyến." },

        { "TOEIC", @"TOEIC LÀ GÌ VÀ TÁC DỤNG
TOEIC là chứng chỉ tiếng Anh quốc tế chuyên về giao tiếp trong môi trường làm việc.
• Xin việc tại doanh nghiệp, công ty nước ngoài.
• Chuẩn đầu ra đại học (450–650 tùy trường).
• Bổ sung hồ sơ thăng chức / nâng lương.

NỘI QUY PHÒNG THI TOEIC
• Có mặt trước giờ thi 30–45 phút.
• Chỉ được mang: bút chì 2B, tẩy, bút bi.
• Cấm: điện thoại, smartwatch, giấy nháp.
• Không đổi vị trí chỗ ngồi khi đã phát đề.

QUY ĐỊNH ĐIỂM SỐ
• 2 kỹ năng: 10–990 điểm.
• 450–550: Mức trung bình.
• 650–750: Khá.
• 800+: Giỏi (phù hợp xin việc quốc tế).

LÝ DO CHỌN TRUNG TÂM
• Quy trình tổ chức theo chuẩn quốc tế.
• Thi trên giấy hoặc máy tính tùy lựa chọn.
• Công bố điểm nhanh, hỗ trợ cấp chứng chỉ." },

        { "VSTEP", @"VSTEP LÀ GÌ VÀ TÁC DỤNG
VSTEP là kỳ thi đánh giá năng lực tiếng Anh do Việt Nam phát triển.
• Chuẩn đầu ra các trường đại học VN.
• Thi công chức/viên chức yêu cầu B1/B2/C1.
• Bổ sung hồ sơ xét tốt nghiệp.

NỘI QUY PHÒNG THI VSTEP
• Đến trước 30–45 phút. Mang CCCD/CMND.
• Không mang điện thoại, tài liệu, đồng hồ thông minh.
• Không tự ý rời phòng thi.

QUY ĐỊNH ĐIỂM SỐ
• Thang điểm 0–10.
• 4.0: B1 (Bậc 3).
• 6.0: B2 (Bậc 4).
• 8.5: C1 (Bậc 5).

LÝ DO CHỌN TRUNG TÂM
• Tổ chức thi theo quy chế Bộ GD&ĐT.
• Thi trên máy, kết quả nhanh.
• Phòng Speaking riêng, đảm bảo âm thanh." },

        { "KET", @"KET (CAMBRIDGE A2 KEY)
KET là chứng chỉ tiếng Anh trình độ A2 cho học sinh phổ thông.
• Định hướng học Cambridge sau này (PET, FCE…).
• Đánh giá năng lực tiếng Anh chuẩn quốc tế.

NỘI QUY PHÒNG THI KET
• Có mặt sớm 30 phút.
• Chỉ dùng bút chì 2B và tẩy.
• Không mang điện thoại, tai nghe, tài liệu.

QUY ĐỊNH ĐIỂM SỐ
• Đạt KET: 120–132 điểm.
• Pass with Merit: 133–139.
• Pass with Distinction: 140–150.

LÝ DO CHỌN TRUNG TÂM
• Giám khảo Cambridge đào tạo.
• Mô hình thi chuẩn quốc tế.
• Hỗ trợ thí sinh nhỏ tuổi ổn định tâm lý." },

        { "PET", @"PET (CAMBRIDGE B1 PRELIMINARY)
PET là chứng chỉ tiếng Anh trình độ B1 thuộc hệ Cambridge.
• Tiền đề cho FCE, CAE, CPE.
• Xét tuyển chương trình song ngữ hoặc quốc tế.

NỘI QUY PHÒNG THI PET
• Có mặt trước giờ thi 30 phút.
• Chỉ sử dụng bút chì 2B, tẩy.
• Cấm điện thoại, tài liệu, thiết bị điện tử.

QUY ĐỊNH ĐIỂM SỐ
• Đạt PET: 140–152.
• Pass with Merit: 153–159.
• Pass with Distinction: 160–170.

LÝ DO CHỌN TRUNG TÂM
• Trang thiết bị thi đạt chuẩn Cambridge.
• Môi trường thi nghiêm túc, hỗ trợ thân thiện." }
        };
    }
}