using System;
using System.Drawing;
using System.Windows.Forms;

namespace WordUp.Controllers
{
    public partial class NotificationBubble : UserControl
    {
        public string PostId { get; set; }
        public string NotificationId { get; set; }
        public event EventHandler BubbleClicked;

        public NotificationBubble(string message, bool isRead, string postId, string notificationId)
        {
            InitializeComponent();

            lblMessage.Text = message;
            PostId = postId;
            NotificationId = notificationId;

            this.BackColor = isRead ? Color.White : Color.LightYellow;
            this.Cursor = Cursors.Hand;

            this.Click += Bubble_Click;
            lblMessage.Click += Bubble_Click;
        }

        private void Bubble_Click(object sender, EventArgs e)
        {
            BubbleClicked?.Invoke(this, e);
        }
    }
}