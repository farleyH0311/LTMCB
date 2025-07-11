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
                string imagePath = Path.Combine(Application.StartupPath, "Resources", "huongdan.png");
                Image original = Image.FromFile(imagePath);
                int availableWidth = panelHuongDan.ClientSize.Width - 20; 

                double ratio = (double)original.Width / original.Height;
                int newWidth = availableWidth;
                int newHeight = (int)(newWidth / ratio);

                Bitmap resizedImage = new Bitmap(original, newWidth, newHeight);

                picHuongDan.Image = resizedImage;
                picHuongDan.Size = new Size(newWidth, newHeight);
                picHuongDan.Location = new Point(10, 10); 
                picHuongDan.SizeMode = PictureBoxSizeMode.Normal;

                original.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải ảnh hướng dẫn: " + ex.Message);
            }
        }
    }
}
