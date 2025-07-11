namespace WordUp.Controllers
{
    partial class ChatItemControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            picAvatar = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            lblName = new Label();
            ((System.ComponentModel.ISupportInitialize)picAvatar).BeginInit();
            SuspendLayout();
            // 
            // picAvatar
            // 
            picAvatar.ImageRotate = 0F;
            picAvatar.Location = new Point(13, 15);
            picAvatar.Name = "picAvatar";
            picAvatar.ShadowDecoration.CustomizableEdges = customizableEdges1;
            picAvatar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            picAvatar.Size = new Size(68, 68);
            picAvatar.TabIndex = 0;
            picAvatar.TabStop = false;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 11F);
            lblName.Location = new Point(98, 36);
            lblName.Name = "lblName";
            lblName.Size = new Size(71, 30);
            lblName.TabIndex = 1;
            lblName.Text = "label1";
            // 
            // ChatItemControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(lblName);
            Controls.Add(picAvatar);
            Name = "ChatItemControl";
            Size = new Size(250, 100);
            ((System.ComponentModel.ISupportInitialize)picAvatar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2CirclePictureBox picAvatar;
        private Label lblName;
    }
}
