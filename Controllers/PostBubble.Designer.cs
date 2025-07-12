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
            lblAuthor = new Label();
            lblContent = new Label();
            lblTimestamp = new Label();
            lblLikes = new Label();
            lblComments = new Label();
            SuspendLayout();
            // 
            // lblAuthor
            // 
            lblAuthor.AutoSize = true;
            lblAuthor.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAuthor.Location = new Point(10, 10);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Size = new Size(0, 28);
            lblAuthor.TabIndex = 0;
            // 
            // lblContent
            // 
            lblContent.AutoSize = true;
            lblContent.Location = new Point(10, 35);
            lblContent.MaximumSize = new Size(400, 0);
            lblContent.Name = "lblContent";
            lblContent.Size = new Size(0, 25);
            lblContent.TabIndex = 1;
            // 
            // lblTimestamp
            // 
            lblTimestamp.AutoSize = true;
            lblTimestamp.Location = new Point(10, 80);
            lblTimestamp.Name = "lblTimestamp";
            lblTimestamp.Size = new Size(0, 25);
            lblTimestamp.TabIndex = 2;
            // 
            // lblLikes
            // 
            lblLikes.AutoSize = true;
            lblLikes.Location = new Point(300, 110);
            lblLikes.Name = "lblLikes";
            lblLikes.Size = new Size(0, 25);
            lblLikes.TabIndex = 3;
            // 
            // lblComments
            // 
            lblComments.AutoSize = true;
            lblComments.Location = new Point(380, 110);
            lblComments.Name = "lblComments";
            lblComments.Size = new Size(0, 25);
            lblComments.TabIndex = 4;
            // 
            // PostBubble
            // 
            BackColor = SystemColors.Info;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(lblAuthor);
            Controls.Add(lblContent);
            Controls.Add(lblTimestamp);
            Controls.Add(lblLikes);
            Controls.Add(lblComments);
            Margin = new Padding(10);
            Name = "PostBubble";
            Padding = new Padding(10);
            Size = new Size(448, 138);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}