using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace WordUp.Controllers
{
    partial class NotificationBubble : UserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblMessage = new Label();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI", 10F);
            lblMessage.Location = new Point(15, 15);
            lblMessage.MaximumSize = new Size(460, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(58, 28);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "Label";
            // 
            // NotificationBubble
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Transparent;
            Controls.Add(lblMessage);
            Margin = new Padding(10);
            Name = "NotificationBubble";
            Padding = new Padding(10);
            Size = new Size(500, 80);
            Paint += NotificationBubble_Paint;
            Resize += NotificationBubble_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        private void NotificationBubble_Paint(object sender, PaintEventArgs e)
        {
            int radius = 20;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = GetRoundedRectPath(this.ClientRectangle, radius))
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                g.FillPath(brush, path);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90); // Top-left
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90); // Top-right
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90); // Bottom-right
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90); // Bottom-left
            path.CloseFigure();

            return path;
        }

        private void NotificationBubble_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}