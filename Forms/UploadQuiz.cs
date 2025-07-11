using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordUp.Services;
using WordUp.Models;

namespace WordUp.Forms
{
    public partial class PractiseQuizForm : BaseForm
    {
        private Form1 mainform;
        public PractiseQuizForm(Form1 MainForm)
        {
            InitializeComponent();
            this.mainform = MainForm;
        }

        private async void roundedButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string jsonPath = @"G:\-n-LTMCB-_-NT106.P22.ANTT-2025\Resources\Lessons.json";

                string credentialsPath = @"G:\-n-LTMCB-_-NT106.P22.ANTT-2025\Resources\serviceAccountKey.json";

                string projectId = "ltm -wu";

                await LessonUploader.UploadLessonsAsync(jsonPath, credentialsPath, projectId);
                MessageBox.Show("Upload thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi upload: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
