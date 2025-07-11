using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WordUp.Controls
{
    public partial class MessageBubble : UserControl
    {
        private bool isCurrentUser;

        public MessageBubble()
        {
            InitializeComponent();
            this.Resize += MessageBubble_Resize;
        }

        public string MessageText
        {
            get => lblMessage.Text;
            set
            {
                lblMessage.Text = value;
                AdjustMessagePosition(); // cập nhật vị trí sau khi đổi nội dung
            }
        }

        public bool IsCurrentUser
        {
            get => isCurrentUser;
            set
            {
                isCurrentUser = value;
                UpdateBubbleStyle();
            }
        }

        private void UpdateBubbleStyle()
        {
            if (isCurrentUser)
            {
                lblMessage.BackColor = Color.LightSkyBlue;
                lblMessage.ForeColor = Color.Black;
                lblMessage.TextAlign = ContentAlignment.MiddleRight;
            }
            else
            {
                lblMessage.BackColor = Color.LightGray;
                lblMessage.ForeColor = Color.Black;
                lblMessage.TextAlign = ContentAlignment.MiddleLeft;
            }

            AdjustMessagePosition();
        }

        private void AdjustMessagePosition()
        {
            // Đảm bảo label đã có kích thước đúng
            lblMessage.MaximumSize = new Size(250, 0);
            lblMessage.AutoSize = true;

            if (isCurrentUser)
            {
                // Canh phải
                lblMessage.Left = this.Width - lblMessage.Width - this.Padding.Right;
            }
            else
            {
                // Canh trái
                lblMessage.Left = this.Padding.Left;
            }
        }

        private void MessageBubble_Resize(object sender, EventArgs e)
        {
            AdjustMessagePosition();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (lblMessage != null && lblMessage.Width > 0 && lblMessage.Height > 0)
            {
                int radius = 20;
                GraphicsPath path = CreateRoundedRectanglePath(lblMessage.ClientRectangle, radius);
                lblMessage.Region = new Region(path);
            }
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            Rectangle arcRect = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // Top-left
            path.AddArc(arcRect, 180, 90);

            // Top-right
            arcRect.X = bounds.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // Bottom-right
            arcRect.Y = bounds.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // Bottom-left
            arcRect.X = bounds.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }

    }
}
