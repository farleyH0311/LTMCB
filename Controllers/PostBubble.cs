using System;
using System.Windows.Forms;
using WordUp.Models;

namespace WordUp.Controllers
{
    public partial class PostBubble : UserControl
    {
        public Post Post { get; private set; }

        public PostBubble(Post post)
        {
            InitializeComponent();
            this.Post = post;

            lblAuthor.Text = post.Author;
            lblContent.Text = post.Content;
            lblTimestamp.Text = post.Timestamp.ToString("dd/MM/yyyy HH:mm");
            lblLikes.Text = "❤️ " + post.Likes;
            lblComments.Text = "💬 " + post.Comments.Count;

            AdjustHeightBasedOnContent();

            AttachClickEventToAllControls(this);
        }
        private void AdjustHeightBasedOnContent()
        {
            this.PerformLayout();

            int bottom = Math.Max(lblLikes.Bottom, lblComments.Bottom);
            this.Height = bottom + 20; 
        }

        private void AttachClickEventToAllControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.Click += (s, e) => this.OnClick(e);
                if (control.HasChildren)
                    AttachClickEventToAllControls(control);
            }
        }
    }
}