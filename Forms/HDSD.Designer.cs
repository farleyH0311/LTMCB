namespace WordUp.Forms
{
    partial class HDSD
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panelHuongDan = new Guna.UI2.WinForms.Guna2Panel();
            picHuongDan = new Guna.UI2.WinForms.Guna2PictureBox();
            panelHuongDan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picHuongDan).BeginInit();
            SuspendLayout();
            // 
            // panelHuongDan
            // 
            panelHuongDan.AutoScroll = true;
            panelHuongDan.Controls.Add(picHuongDan);
            panelHuongDan.CustomizableEdges = customizableEdges3;
            panelHuongDan.Dock = DockStyle.Fill;
            panelHuongDan.Location = new Point(0, 0);
            panelHuongDan.Name = "panelHuongDan";
            panelHuongDan.ShadowDecoration.CustomizableEdges = customizableEdges4;
            panelHuongDan.Size = new Size(1271, 792);
            panelHuongDan.TabIndex = 0;
            // 
            // picHuongDan
            // 
            picHuongDan.CustomizableEdges = customizableEdges1;
            picHuongDan.ImageRotate = 0F;
            picHuongDan.Location = new Point(0, 0);
            picHuongDan.Name = "picHuongDan";
            picHuongDan.ShadowDecoration.CustomizableEdges = customizableEdges2;
            picHuongDan.Size = new Size(1271, 789);
            picHuongDan.SizeMode = PictureBoxSizeMode.AutoSize;
            picHuongDan.TabIndex = 0;
            picHuongDan.TabStop = false;
            // 
            // HDSD
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1271, 792);
            Controls.Add(panelHuongDan);
            Name = "HDSD";
            Text = "HDSD";
            panelHuongDan.ResumeLayout(false);
            panelHuongDan.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picHuongDan).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel panelHuongDan;
        private Guna.UI2.WinForms.Guna2PictureBox picHuongDan;
    }
}