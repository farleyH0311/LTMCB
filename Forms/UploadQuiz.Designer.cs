namespace WordUp.Forms
{
    partial class PractiseQuizForm
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
            btnUploadLessons = new RoundedButton();
            SuspendLayout();
            // 
            // btnUploadLessons
            // 
            btnUploadLessons.Location = new Point(330, 224);
            btnUploadLessons.Name = "btnUploadLessons";
            btnUploadLessons.Size = new Size(282, 84);
            btnUploadLessons.TabIndex = 0;
            btnUploadLessons.Text = "Upload bài học";
            btnUploadLessons.UseVisualStyleBackColor = true;
            btnUploadLessons.Click += roundedButton1_Click;
            // 
            // PractiseQuizForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(951, 580);
            Controls.Add(btnUploadLessons);
            Name = "PractiseQuizForm";
            Text = "PractiseQuizForm";
            ResumeLayout(false);
        }

        #endregion

        private RoundedButton btnUploadLessons;
    }
}