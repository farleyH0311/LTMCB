namespace WordUp.Controllers
{
    partial class PostBubble
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblAuthor;
        private Label lblContent;
        private Label lblTimestamp;
        private Label lblLikes;
        private Label lblComments;

        private void InitializeComponent()
        {
            this.lblAuthor = new Label();
            this.lblContent = new Label();
            this.lblTimestamp = new Label();
            this.lblLikes = new Label();
            this.lblComments = new Label();

            this.SuspendLayout();

            // Set properties for labels...
            // Ví dụ:
            this.lblAuthor.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.lblAuthor.Location = new System.Drawing.Point(10, 10);
            this.lblAuthor.AutoSize = true;

            this.lblContent.Location = new System.Drawing.Point(10, 35);
            this.lblContent.MaximumSize = new System.Drawing.Size(400, 0);
            this.lblContent.AutoSize = true;

            this.lblTimestamp.Location = new System.Drawing.Point(10, 80);
            this.lblTimestamp.AutoSize = true;

            this.lblLikes.Location = new System.Drawing.Point(300, 110);
            this.lblLikes.AutoSize = true;

            this.lblComments.Location = new System.Drawing.Point(380, 110);
            this.lblComments.AutoSize = true;

            // Add controls
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.lblTimestamp);
            this.Controls.Add(this.lblLikes);
            this.Controls.Add(this.lblComments);

            // Set this UserControl's properties
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Size = new System.Drawing.Size(450, 140);
            this.Margin = new Padding(10);
            this.Padding = new Padding(10);
            this.BorderStyle = BorderStyle.FixedSingle;

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}