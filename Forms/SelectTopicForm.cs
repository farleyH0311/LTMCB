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
using System.IO;

namespace WordUp.Forms
{
    public partial class SelectTopicForm : BaseForm
    {
        private FirebaseLessonService service;
        private readonly User currentUser;
        private Form1 mainform;
        public SelectTopicForm(Form1 MainForm, User user)
        {
            InitializeComponent();
            this.mainform = MainForm;
            currentUser = user;
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void SelectTopicForm_Load(object sender, EventArgs e)
        {
            string jsonPath = Path.Combine(Application.StartupPath, @"..\..\..\Resources\serviceAccountKey.json");
            service = new FirebaseLessonService("ltm-wu", Path.GetFullPath(jsonPath));
            service = new FirebaseLessonService("ltm-wu", jsonPath); List<string> topics = await service.GetAllLessonIdsAsync();

            foreach (var topic in topics)
            {
                var btn = new Guna.UI2.WinForms.Guna2Button
                {
                    Text = topic,
                    Width = 321,
                    Height = 80,
                    Font = new Font("Comic Sans MS", 10, FontStyle.Bold),
                    FillColor = Color.MidnightBlue,
                    ForeColor = Color.White,
                    BorderRadius = 15,
                    Margin = new Padding(10),
                    HoverState =
                    {
                        FillColor = Color.White,
                        ForeColor = Color.MidnightBlue,
                    }
                };

                btn.Click += async (s, ev) =>
                {
                    var lesson = await service.GetLessonAsync(topic);
                    if (lesson != null)
                    {
                        this.Hide();
                        if (mainform.practisequiz != null)
                            try { mainform.practisequiz.Close(); } catch { }
                        mainform.practisequiz = new PractiseQuiz(mainform, lesson, currentUser);
                        mainform.practisequiz.Show();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu cho chủ đề này.");
                    }
                };

                flowTopics.Controls.Add(btn);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            foreach (Control ctrl in flowTopics.Controls)
            {
                if (ctrl is Guna.UI2.WinForms.Guna2Button btn)
                {
                    btn.Visible = btn.Text.ToLower().Contains(keyword);
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.home == null || mainform.home.IsDisposed)
                mainform.home =  new Home(mainform, currentUser);
            mainform.home.Show();
            this.Close();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
