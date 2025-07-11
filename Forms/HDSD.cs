using System;
using System.Drawing;
using System.Windows.Forms;
using WordUp.Models;

namespace WordUp.Forms
{
    public partial class HDSD : BaseForm
    {
        private Form1 mainform;
        public HDSD(Form1 MainForm)
        {
            InitializeComponent();
            this.Load += HDSD_Load;
            this.mainform = MainForm;
        }

        private void HDSD_Load(object sender, EventArgs e)
        {
            LoadHuongDanImage();
        }

        private void LoadHuongDanImage()
        {
            try
            {
                // Load ảnh gốc
                Image original = Image.FromFile("huongdan.png");

                // Tính toán kích thước mới dựa trên chiều rộng panel (trừ scrollbar)
                int availableWidth = panelHuongDan.ClientSize.Width - 20; // Trừ 20px cho padding

                // Tính tỷ lệ để giữ nguyên tỷ lệ khung hình
                double ratio = (double)original.Width / original.Height;
                int newWidth = availableWidth;
                int newHeight = (int)(newWidth / ratio);

                // Tạo ảnh mới với kích thước đã tính toán
                Bitmap resizedImage = new Bitmap(original, newWidth, newHeight);

                // Thiết lập cho PictureBox
                picHuongDan.Image = resizedImage;
                picHuongDan.Size = new Size(newWidth, newHeight);
                picHuongDan.Location = new Point(10, 10); // Một chút padding từ góc trên trái
                picHuongDan.SizeMode = PictureBoxSizeMode.Normal;

                // Giải phóng ảnh gốc
                original.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải ảnh hướng dẫn: " + ex.Message);
            }
        }
    }
}
