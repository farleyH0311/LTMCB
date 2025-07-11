using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
//using FirestoreUploader.Models;
using WordUp.Models;
using WordUp.Services;

namespace WordUp.Forms
{
    public partial class PractiseQuiz : BaseForm
    {
        private Lesson currentLesson;
        private User currentUser;
        private FirebaseService firebaseService;
        private int currentIndex = 0;
        private int score = 0;
        private string correctAnswer = "";
        private List<string> shuffledOptions = new();
        private Form1 mainform;

        public PractiseQuiz(Form1 MainForm, Lesson lesson, User currentUser)
        {
            InitializeComponent();
            this.mainform = MainForm;
            firebaseService = new FirebaseService();
            labelTitle.Left = (guna2ShadowPanel3.Width - labelTitle.Width) / 2;
            this.currentLesson = lesson;
            this.currentUser = currentUser;

            currentIndex = 0;
            score = 0;

            guna2Button1.Click += Answer_Click;
            guna2Button2.Click += Answer_Click;
            guna2Button3.Click += Answer_Click;
            guna2Button4.Click += Answer_Click;
            labelTitle.Text = lesson.Title;
            LoadQuestion();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            currentIndex++;
            guna2Button1.Enabled = guna2Button2.Enabled = guna2Button3.Enabled = guna2Button4.Enabled = true;

            guna2Button1.FillColor = guna2Button2.FillColor =
            guna2Button3.FillColor = guna2Button4.FillColor = Color.RoyalBlue;

            LoadQuestion();
        }

        private async void LoadQuestion()
        {
            if (currentIndex >= currentLesson.Questions.Count)
            {
                MessageBox.Show($"Hoàn thành! Điểm của bạn là {score}/{currentLesson.Questions.Count}");

                currentUser.Score += score;

                bool success = await firebaseService.UpdateScoreAsync(currentUser.Username, currentUser.Score);

                if (success)
                {
                    MessageBox.Show("Điểm của bạn đã được cập nhật!");
                }
                else
                {
                    MessageBox.Show("Cập nhật điểm thất bại. Vui lòng thử lại.");
                }

                // Tự động chuyển về SelectTopicForm
                this.Hide();
                if (mainform.selecttopicform != null)
                    try { mainform.selecttopicform.Close(); } catch { }
                mainform.selecttopicform = new SelectTopicForm(mainform, currentUser);
                mainform.selecttopicform.Show();
                this.Close();
                return;
            }


            var q = currentLesson.Questions[currentIndex];
            labelQuestion.Text = q.QuestionText;

            shuffledOptions = q.Options.OrderBy(x => Guid.NewGuid()).ToList();

            guna2Button1.Text = shuffledOptions[0];
            guna2Button2.Text = shuffledOptions[1];
            guna2Button3.Text = shuffledOptions[2];
            guna2Button4.Text = shuffledOptions[3];

            correctAnswer = q.CorrectAnswer;

            guna2ProgressBar1.Maximum = currentLesson.Questions.Count;
            guna2ProgressBar1.Value = currentIndex + 1;
            Next.Enabled = false;
        }

        private async void Answer_Click(object sender, EventArgs e)
        {
            var btn = sender as Guna.UI2.WinForms.Guna2Button;

            if (btn.Text == correctAnswer)
            {
                score++;
                lblScore.Text = "Điểm: " + score.ToString();
                btn.FillColor = Color.LightGreen;
            }
            else
            {
                btn.FillColor = Color.IndianRed;


                foreach (var b in new[] { guna2Button1, guna2Button2, guna2Button3, guna2Button4 })
                {
                    if (b.Text == correctAnswer)
                    {
                        b.FillColor = Color.LightGreen;
                        break;
                    }
                }
            }

            await Task.Delay(1000);

            guna2Button1.Enabled = guna2Button2.Enabled = guna2Button3.Enabled = guna2Button4.Enabled = false;

            // Kiểm tra nếu đây là câu hỏi cuối cùng
            if (currentIndex == currentLesson.Questions.Count - 1)
            {

                await FinishQuiz();
            }
            else
            {

                Next.Enabled = true;
            }
        }

        private async Task FinishQuiz()
        {
            // Hiển thị kết quả
            MessageBox.Show($"Hoàn thành! Điểm của bạn là {score}/{currentLesson.Questions.Count}");

            // Cập nhật điểm
            currentUser.Score += score;

            bool success = await firebaseService.UpdateScoreAsync(currentUser.Username, currentUser.Score);

            if (success)
            {
                MessageBox.Show("Điểm của bạn đã được cập nhật!");
            }
            else
            {
                MessageBox.Show("Cập nhật điểm thất bại. Vui lòng thử lại.");
            }

            // Tự động chuyển về SelectTopicForm
            this.Hide();
            if (mainform.selecttopicform != null)
                try { mainform.selecttopicform.Close(); } catch { }
            mainform.selecttopicform = new SelectTopicForm(mainform, currentUser);
            mainform.selecttopicform.Show();
            this.Close();
        }

        private void Quiz_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.selecttopicform != null)
                try { mainform.selecttopicform.Close(); } catch { }
            mainform.selecttopicform = new SelectTopicForm(mainform, currentUser);
            mainform.selecttopicform.Show();
            this.Close();
        }
    }
}