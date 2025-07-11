namespace WordUp.Forms
{
    partial class ResultForm
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultForm));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            labelYourScore = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2ShadowPanel2 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            flowLayoutPanel = new FlowLayoutPanel();
            btnExit = new Guna.UI2.WinForms.Guna2Button();
            btnReplay = new Guna.UI2.WinForms.Guna2Button();
            labelLeaderboardTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            guna2ShadowPanel1.SuspendLayout();
            guna2ShadowPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // guna2ShadowPanel1
            // 
            guna2ShadowPanel1.BackColor = Color.Transparent;
            guna2ShadowPanel1.Controls.Add(labelYourScore);
            guna2ShadowPanel1.FillColor = Color.White;
            guna2ShadowPanel1.Location = new Point(321, 31);
            guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            guna2ShadowPanel1.Radius = 15;
            guna2ShadowPanel1.ShadowColor = Color.Black;
            guna2ShadowPanel1.Size = new Size(610, 84);
            guna2ShadowPanel1.TabIndex = 0;
            // 
            // labelYourScore
            // 
            labelYourScore.BackColor = Color.Transparent;
            labelYourScore.Font = new Font("Comic Sans MS", 16.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelYourScore.ForeColor = Color.IndianRed;
            labelYourScore.Location = new Point(166, 3);
            labelYourScore.Name = "labelYourScore";
            labelYourScore.Size = new Size(314, 62);
            labelYourScore.TabIndex = 0;
            labelYourScore.Text = "labelYourScore";
            // 
            // guna2ShadowPanel2
            // 
            guna2ShadowPanel2.BackColor = Color.Transparent;
            guna2ShadowPanel2.Controls.Add(flowLayoutPanel);
            guna2ShadowPanel2.Controls.Add(btnExit);
            guna2ShadowPanel2.Controls.Add(btnReplay);
            guna2ShadowPanel2.Controls.Add(labelLeaderboardTitle);
            guna2ShadowPanel2.FillColor = Color.White;
            guna2ShadowPanel2.Location = new Point(487, 147);
            guna2ShadowPanel2.Name = "guna2ShadowPanel2";
            guna2ShadowPanel2.Radius = 20;
            guna2ShadowPanel2.ShadowColor = Color.Black;
            guna2ShadowPanel2.Size = new Size(746, 610);
            guna2ShadowPanel2.TabIndex = 1;
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.Location = new Point(110, 106);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(528, 384);
            flowLayoutPanel.TabIndex = 3;
            // 
            // btnExit
            // 
            btnExit.BorderRadius = 20;
            btnExit.CustomizableEdges = customizableEdges1;
            btnExit.DisabledState.BorderColor = Color.DarkGray;
            btnExit.DisabledState.CustomBorderColor = Color.DarkGray;
            btnExit.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnExit.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnExit.FillColor = Color.SandyBrown;
            btnExit.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExit.ForeColor = Color.MidnightBlue;
            btnExit.Location = new Point(424, 518);
            btnExit.Name = "btnExit";
            btnExit.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnExit.Size = new Size(258, 76);
            btnExit.TabIndex = 2;
            btnExit.Text = "🔙 Thoát";
            // 
            // btnReplay
            // 
            btnReplay.BorderRadius = 20;
            btnReplay.CustomizableEdges = customizableEdges3;
            btnReplay.DisabledState.BorderColor = Color.DarkGray;
            btnReplay.DisabledState.CustomBorderColor = Color.DarkGray;
            btnReplay.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnReplay.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnReplay.FillColor = Color.SandyBrown;
            btnReplay.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnReplay.ForeColor = Color.MidnightBlue;
            btnReplay.Location = new Point(77, 518);
            btnReplay.Name = "btnReplay";
            btnReplay.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnReplay.Size = new Size(258, 76);
            btnReplay.TabIndex = 1;
            btnReplay.Text = " ⇄ Chơi lại";
            // 
            // labelLeaderboardTitle
            // 
            labelLeaderboardTitle.BackColor = Color.Transparent;
            labelLeaderboardTitle.Font = new Font("Cooper Black", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelLeaderboardTitle.ForeColor = Color.DarkSlateBlue;
            labelLeaderboardTitle.Location = new Point(230, 17);
            labelLeaderboardTitle.Name = "labelLeaderboardTitle";
            labelLeaderboardTitle.Size = new Size(288, 57);
            labelLeaderboardTitle.TabIndex = 0;
            labelLeaderboardTitle.Text = "Final result";
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackgroundImage = (Image)resources.GetObject("guna2Panel1.BackgroundImage");
            guna2Panel1.BackgroundImageLayout = ImageLayout.Stretch;
            guna2Panel1.CustomizableEdges = customizableEdges5;
            guna2Panel1.Location = new Point(29, 207);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2Panel1.Size = new Size(428, 498);
            guna2Panel1.TabIndex = 2;
            // 
            // ResultForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 255, 192);
            ClientSize = new Size(1271, 792);
            Controls.Add(guna2Panel1);
            Controls.Add(guna2ShadowPanel2);
            Controls.Add(guna2ShadowPanel1);
            Name = "ResultForm";
            Text = "ResultForm";
            guna2ShadowPanel1.ResumeLayout(false);
            guna2ShadowPanel1.PerformLayout();
            guna2ShadowPanel2.ResumeLayout(false);
            guna2ShadowPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel labelYourScore;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel labelLeaderboardTitle;
        private Guna.UI2.WinForms.Guna2Button btnReplay;
        private Guna.UI2.WinForms.Guna2Button btnExit;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private FlowLayoutPanel flowLayoutPanel;
    }
}