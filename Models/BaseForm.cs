using System;
using System.Drawing;
using System.Windows.Forms;

namespace WordUp.Models
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            this.Size = new Size(1000, 675);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (Control ctrl in this.Controls)
            {
                ctrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
        }
    }
}
