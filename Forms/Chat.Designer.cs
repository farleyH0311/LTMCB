namespace WordUp.Forms
{
    partial class Chat
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            splitContainer1 = new SplitContainer();
            flowChatList = new FlowLayoutPanel();
            panel1 = new Panel();
            txtSearchUser = new Guna.UI2.WinForms.Guna2TextBox();
            label1 = new Label();
            flowMessages = new FlowLayoutPanel();
            panelBottom = new Panel();
            btnSend = new Guna.UI2.WinForms.Guna2Button();
            txtMessage = new TextBox();
            panelHeader = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            lblUserName = new Label();
            picAvatar = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            panelBottom.SuspendLayout();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picAvatar).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(flowChatList);
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(flowMessages);
            splitContainer1.Panel2.Controls.Add(panelBottom);
            splitContainer1.Panel2.Controls.Add(panelHeader);
            splitContainer1.Size = new Size(978, 619);
            splitContainer1.SplitterDistance = 250;
            splitContainer1.TabIndex = 0;
            // 
            // flowChatList
            // 
            flowChatList.BackColor = Color.FromArgb(255, 241, 229);
            flowChatList.Dock = DockStyle.Bottom;
            flowChatList.Location = new Point(0, 50);
            flowChatList.Name = "flowChatList";
            flowChatList.Size = new Size(250, 569);
            flowChatList.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.BackColor = Color.RosyBrown;
            panel1.Controls.Add(txtSearchUser);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 50);
            panel1.TabIndex = 1;
            // 
            // txtSearchUser
            // 
            txtSearchUser.BackColor = Color.RosyBrown;
            txtSearchUser.BorderRadius = 23;
            txtSearchUser.CustomizableEdges = customizableEdges1;
            txtSearchUser.DefaultText = "";
            txtSearchUser.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtSearchUser.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtSearchUser.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtSearchUser.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtSearchUser.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearchUser.Font = new Font("Segoe UI", 9F);
            txtSearchUser.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearchUser.Location = new Point(43, 5);
            txtSearchUser.Margin = new Padding(10);
            txtSearchUser.Name = "txtSearchUser";
            txtSearchUser.PlaceholderText = "";
            txtSearchUser.SelectedText = "";
            txtSearchUser.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtSearchUser.Size = new Size(197, 40);
            txtSearchUser.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 9);
            label1.Name = "label1";
            label1.Size = new Size(37, 25);
            label1.TabIndex = 1;
            label1.Text = "🔙";
            label1.Click += btnHome_Click;
            // 
            // flowMessages
            // 
            flowMessages.AutoScroll = true;
            flowMessages.BackColor = SystemColors.Info;
            flowMessages.Dock = DockStyle.Fill;
            flowMessages.FlowDirection = FlowDirection.TopDown;
            flowMessages.Location = new Point(0, 80);
            flowMessages.Name = "flowMessages";
            flowMessages.Size = new Size(724, 449);
            flowMessages.TabIndex = 2;
            flowMessages.WrapContents = false;
            // 
            // panelBottom
            // 
            panelBottom.BackColor = SystemColors.GradientInactiveCaption;
            panelBottom.Controls.Add(btnSend);
            panelBottom.Controls.Add(txtMessage);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 529);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(724, 90);
            panelBottom.TabIndex = 1;
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.Transparent;
            btnSend.BorderRadius = 20;
            btnSend.CustomizableEdges = customizableEdges3;
            btnSend.DisabledState.BorderColor = Color.DarkGray;
            btnSend.DisabledState.CustomBorderColor = Color.DarkGray;
            btnSend.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSend.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnSend.Font = new Font("Segoe UI", 9F);
            btnSend.ForeColor = Color.PeachPuff;
            btnSend.Location = new Point(634, 26);
            btnSend.Name = "btnSend";
            btnSend.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnSend.Size = new Size(77, 44);
            btnSend.TabIndex = 2;
            btnSend.Text = "send";
            btnSend.Click += btnSend_Click;
            // 
            // txtMessage
            // 
            txtMessage.Font = new Font("Segoe UI", 10F);
            txtMessage.Location = new Point(62, 31);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(566, 34);
            txtMessage.TabIndex = 2;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(253, 243, 155);
            panelHeader.Controls.Add(lblUserName);
            panelHeader.Controls.Add(picAvatar);
            panelHeader.CustomizableEdges = customizableEdges6;
            panelHeader.Dock = DockStyle.Top;
            panelHeader.FillColor = Color.FromArgb(253, 243, 155);
            panelHeader.FillColor2 = Color.FromArgb(253, 243, 155);
            panelHeader.FillColor3 = Color.FromArgb(253, 243, 155);
            panelHeader.FillColor4 = Color.FromArgb(253, 243, 155);
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.ShadowDecoration.CustomizableEdges = customizableEdges7;
            panelHeader.Size = new Size(724, 80);
            panelHeader.TabIndex = 0;
            panelHeader.Paint += panelHeader_Paint;
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.BackColor = Color.Transparent;
            lblUserName.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblUserName.Location = new Point(123, 22);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(96, 38);
            lblUserName.TabIndex = 1;
            lblUserName.Text = "label1";
            // 
            // picAvatar
            // 
            picAvatar.BackColor = Color.Transparent;
            picAvatar.ImageRotate = 0F;
            picAvatar.Location = new Point(18, 1);
            picAvatar.Name = "picAvatar";
            picAvatar.ShadowDecoration.CustomizableEdges = customizableEdges5;
            picAvatar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            picAvatar.Size = new Size(82, 77);
            picAvatar.TabIndex = 1;
            picAvatar.TabStop = false;
            // 
            // Chat
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(978, 619);
            Controls.Add(splitContainer1);
            Name = "Chat";
            Text = "Chat";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picAvatar).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel panelHeader;
        private Guna.UI2.WinForms.Guna2CirclePictureBox picAvatar;
        private Label lblUserName;
        private Panel panelBottom;
        private TextBox txtMessage;
        private Guna.UI2.WinForms.Guna2Button btnSend;
        private FlowLayoutPanel flowMessages;
        private FlowLayoutPanel flowChatList;
        private Guna.UI2.WinForms.Guna2TextBox txtSearchUser;
        private Panel panel1;
        private Label label1;
    }
}