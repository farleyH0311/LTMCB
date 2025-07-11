using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordUp.Helpers;
using WordUp.Models;
using Google.Cloud.Firestore;


namespace WordUp.Forms
{
    public partial class Game : BaseForm
    {
        private readonly User currentUser;
        private readonly string roomId;
        private List<Question> questions = new List<Question>();
        private int currentQuestionIndex = 0;
        private int totalQuestions = 0;
        private int score = 0;
        private System.Windows.Forms.Timer countdownTimer;
        private Form1 mainform;

        private TimeSpan timeLeft = TimeSpan.FromMinutes(3);


        public Game(Form1 MainForm, User user, string roomId)
        {
            InitializeComponent();
            this.currentUser = user;
            this.mainform = MainForm;
            this.roomId = roomId;
            this.Load += Game_Load;
        }

        private async void Game_Load(object sender, EventArgs e)
        {
            labelQuestion.Text = "Đang tải câu hỏi...";
            DisableAnswerButtons();

            await LoadQuestionsFromFirestore();

            if (questions.Count > 0)
            {
                totalQuestions = questions.Count;
                Console.WriteLine($"Bắt đầu hiển thị câu hỏi đầu tiên.");
                DisplayQuestion(currentQuestionIndex);
                StartCountdownTimer();
            }
            else
            {
                MessageBox.Show("Không có câu hỏi hợp lệ.");
                this.Close();
            }
        }


        private async Task LoadQuestionsFromFirestore()
        {
            try
            {
                var db = FirestoreHelper.GetDb();
                var roomDoc = await db.Collection("Rooms").Document(roomId).GetSnapshotAsync();

                if (!roomDoc.Exists)
                    throw new Exception("Không tìm thấy phòng.");

                if (!roomDoc.ContainsField("questions"))
                    throw new Exception("Không có trường 'questions' trong document.");

                var rawList = roomDoc.GetValue<IEnumerable<object>>("questions");

                questions = new List<Question>();
                int index = 1;

                foreach (var qObj in rawList)
                {
                    if (qObj is Dictionary<string, object> qDict)
                    {
                        string questionText = qDict.ContainsKey("questionText") ? qDict["questionText"]?.ToString() ?? "" : "";
                        string correctAnswer = qDict.ContainsKey("correctAnswer") ? qDict["correctAnswer"]?.ToString() ?? "" : "";

                        if (qDict.TryGetValue("options", out var optsObj) && optsObj is IEnumerable<object> optionsList)
                        {
                            var options = optionsList.Select(o => o.ToString()).ToList();
                            if (options.Count == 4)
                            {
                                questions.Add(new Question
                                {
                                    QuestionText = questionText,
                                    CorrectAnswer = correctAnswer,
                                    Options = options
                                });

                                Console.WriteLine($"[Câu {index++}] {questionText}");
                            }
                            else
                            {
                                Console.WriteLine("Câu hỏi có số lượng đáp án khác 4.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Không có danh sách đáp án.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Không thể ép kiểu câu hỏi.");
                    }
                }

                Console.WriteLine($"Đã load tổng cộng {questions.Count} câu hỏi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load câu hỏi: " + ex.Message);
                questions = new List<Question>(); // Reset để tránh lỗi sau
            }
        }


        private void DisplayQuestion(int index)
        {
            try
            {
                if (questions == null || questions.Count == 0)
                {
                    MessageBox.Show("Danh sách câu hỏi rỗng!");
                    this.Close();
                    return;
                }

                if (index < 0 || index >= questions.Count)
                {
                    MessageBox.Show($"Index không hợp lệ: {index}. Số câu hỏi: {questions.Count}");
                    this.Close();
                    return;
                }

                var q = questions[index];

                if (q.Options == null || q.Options.Count < 4)
                {
                    MessageBox.Show($"Câu hỏi không hợp lệ (có {q.Options?.Count ?? 0} đáp án, cần 4).");
                    this.Close();
                    return;
                }

                labelQuestion.Text = $"Câu {index + 1}/{totalQuestions}: {q.QuestionText}";
                Random rnd = new Random();
                btnA.Text = q.Options[rnd.Next(0, 3)];
                int value = rnd.Next(0, 3);
                while (q.Options[value] == btnA.Text)
                    value = rnd.Next(0, 3);
                btnB.Text = q.Options[value];
                while (q.Options[value] == btnB.Text || q.Options[value] == btnA.Text)
                    value = rnd.Next(0, 3);
                btnC.Text = q.Options[value];
                for (int i = 0; i < 4; i++)
                {
                    string option = q.Options[i];
                    if (option == btnA.Text)
                        continue;
                    if (option == btnB.Text)
                        continue;
                    if (option == btnC.Text)
                        continue;
                    btnD.Text = q.Options[3];
                }
                btnA.Enabled = true;
                btnB.Enabled = true;
                btnC.Enabled = true;
                btnD.Enabled = true;

                this.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị câu hỏi: {ex.Message}");
                this.Close();
            }
        }

        private async Task HandleAnswer(string selectedAnswer)
        {
            try
            {
                DisableAnswerButtons();

                if (questions == null || questions.Count == 0)
                {
                    MessageBox.Show(" Danh sách câu hỏi rỗng hoặc chưa tải xong.");
                    return;
                }

                if (currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count)
                {
                    MessageBox.Show($"Câu hỏi số {currentQuestionIndex + 1} không tồn tại. Tổng số câu: {questions.Count}");
                    return;
                }

                var q = questions[currentQuestionIndex];

                if (selectedAnswer == q.CorrectAnswer)
                {
                    score++;
                }

                var db = FirestoreHelper.GetDb();
                var answerRef = db.Collection("Rooms").Document(roomId).Collection("answers").Document(currentUser.Username);

                var answerData = new Dictionary<string, object>
                {
                    { $"q{currentQuestionIndex}", selectedAnswer },
                    { "score", score },
                    { "done", currentQuestionIndex == totalQuestions - 1 },
                    { "timestamp", FieldValue.ServerTimestamp }
                };

                await answerRef.SetAsync(answerData, SetOptions.MergeAll);

                currentQuestionIndex++;

                if (currentQuestionIndex < totalQuestions)
                {
                    DisplayQuestion(currentQuestionIndex);
                }
                else
                {
                    labelQuestion.Text = "Đang chờ người chơi khác...";
                    DisableAnswerButtons();
                    await WaitForEveryoneToFinish();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý câu trả lời: {ex.Message}");
                EnableAnswerButtons();
            }
        }

        private void DisableAnswerButtons()
        {
            btnA.Enabled = false;
            btnB.Enabled = false;
            btnC.Enabled = false;
            btnD.Enabled = false;
        }

        private void EnableAnswerButtons()
        {
            btnA.Enabled = true;
            btnB.Enabled = true;
            btnC.Enabled = true;
            btnD.Enabled = true;
        }

        private async Task WaitForEveryoneToFinish()
        {
            try
            {
                var db = FirestoreHelper.GetDb();
                var playersRef = db.Collection("Rooms").Document(roomId).Collection("players");
                var playersSnapshot = await playersRef.GetSnapshotAsync();
                int totalPlayers = playersSnapshot.Count;

                while (true)
                {
                    var answersSnapshot = await db.Collection("Rooms").Document(roomId).Collection("answers").GetSnapshotAsync();
                    int doneCount = answersSnapshot.Documents.Count(doc => doc.ContainsField("done") && doc.GetValue<bool>("done"));

                    if (doneCount >= totalPlayers)
                        break;

                    await Task.Delay(1000);
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        this.Hide();
                        if (mainform.resultform != null)
                            try { mainform.resultform.Close(); } catch { }
                        mainform.resultform = new ResultForm(mainform, currentUser, roomId);
                        mainform.resultform.Show();
                        this.Close();
                    }));
                }
                else
                {
                    this.Hide();
                    if (mainform.resultform != null)
                        try { mainform.resultform.Close(); } catch { }
                    mainform.resultform = new ResultForm(mainform, currentUser, roomId);
                    mainform.resultform.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chờ người chơi khác: {ex.Message}");
            }
        }

        private async void btnA_Click(object sender, EventArgs e) => await HandleAnswer(btnA.Text);
        private async void btnB_Click(object sender, EventArgs e) => await HandleAnswer(btnB.Text);
        private async void btnC_Click(object sender, EventArgs e) => await HandleAnswer(btnC.Text);
        private async void btnD_Click(object sender, EventArgs e) => await HandleAnswer(btnD.Text);


        private void StartCountdownTimer()
        {
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000; // 1 giây
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private async void CountdownTimer_Tick(object sender, EventArgs e)
        {
            timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
            lblTime.Text = $"⏰ {timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";

            if (timeLeft.TotalSeconds <= 0)
            {
                countdownTimer.Stop();
                DisableAnswerButtons();
                labelQuestion.Text = "⏰ Hết giờ!";

                var db = FirestoreHelper.GetDb();
                var answerRef = db.Collection("Rooms").Document(roomId).Collection("answers").Document(currentUser.Username);

                var answerData = new Dictionary<string, object>
        {
            { "score", score },
            { "done", true },
            { "timestamp", FieldValue.ServerTimestamp }
        };

                await answerRef.SetAsync(answerData, SetOptions.MergeAll);

                await WaitForEveryoneToFinish();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            countdownTimer?.Stop();
            countdownTimer?.Dispose();
            base.OnFormClosing(e);
        }


        private void btnA_Click_1(object sender, EventArgs e)
        {

        }

        private void btnB_Click_1(object sender, EventArgs e)
        {

        }

        private void btnC_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void lblTime_Click(object sender, EventArgs e)
        {

        }
    }
}
