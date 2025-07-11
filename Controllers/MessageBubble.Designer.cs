namespace WordUp.Controls
{
    partial class MessageBubble
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblMessage = new Label();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblMessage.AutoSize = true;
            lblMessage.BackColor = Color.FromArgb(253, 243, 155);
            lblMessage.Font = new Font("Segoe UI", 10F);
            lblMessage.ForeColor = Color.Black;
            lblMessage.Location = new Point(0, 5);
            lblMessage.MaximumSize = new Size(250, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Padding = new Padding(10);
            lblMessage.Size = new Size(85, 48);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "label1";
            // 
            // MessageBubble
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.Transparent;
            Controls.Add(lblMessage);
            Margin = new Padding(4, 5, 1, 5);
            Name = "MessageBubble";
            Padding = new Padding(10);
            Size = new Size(694, 63);
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Label lblMessage;
    }
}
