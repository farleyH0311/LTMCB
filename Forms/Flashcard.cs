using Google.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordUp.Models;
using WordUp.Services;
using NAudio.Wave;
using Microsoft.CognitiveServices.Speech;
using System.IO;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Guna.UI2.WinForms;


namespace WordUp.Forms
{
    public partial class FlashcardForm : BaseForm
    {
        private readonly User currentUser;
        private readonly string currentUid;
        private FirebaseService firebaseService;
        private FirebaseLessonService service;
        private readonly string recordedFilePath = Path.Combine(Path.GetTempPath(), "user_audio.wav");
        private WaveInEvent waveSource = null;
        private WaveFileWriter waveFile = null;
        private System.Windows.Forms.Timer scoreCardTimer;

        private List<Flashcard> currentFlashcards;
        private int currentFlashcardIndex = 0;
        private FirebaseProgressService progressService = new FirebaseProgressService();
        private FirestoreFlashcardService firestoreService;
        private bool isFront = true;
        private HashSet<int> flippedIndexes = new HashSet<int>();
        private Form1 mainform;

        public FlashcardForm(Form1 MainForm, User user, string uid)
        {
            InitializeComponent();
            this.mainform = MainForm;
            currentUser = user;
            currentUid = uid;
            firestoreService = new FirestoreFlashcardService();
            firebaseService = new FirebaseService();
            scoreCardTimer = new System.Windows.Forms.Timer();
            scoreCardTimer.Interval = 2500;
            scoreCardTimer.Tick += (s, e) =>
            {
                scoreCardPanel.Visible = false;
                scoreCardTimer.Stop();
            };

        }

        private void PopulateLessonDropdown(string[] lessons)
        {
            panelLessonDropdown.Controls.Clear();  
            int y = 10;
            foreach (string lesson in lessons)
            {
                Guna2Button btn = new Guna2Button();
                btn.Text = lesson;
                btn.Size = new Size(panelLessonDropdown.Width - 50, 50);
                btn.Location = new Point(10, y);
                btn.BorderRadius = 5;
                btn.FillColor = Color.White;
                btn.ForeColor = Color.Black;
                btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                btn.Padding = new Padding(10, 5, 10, 5);
                btn.Margin = new Padding(10);
                btn.Click += (s, e) =>
                {
                    btnSelectLesson.Text = lesson;
                    panelLessonDropdown.Visible = false;
                    LoadFlashcardsByLesson(lesson);
                };
                panelLessonDropdown.Controls.Add(btn);
                y += 42;
            }
        }

        private async void FlashcardForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<string> lessons = await firestoreService.GetAllLessonsAsync();

                if (lessons.Count > 0)
                {
                    btnSelectLesson.Text = lessons[0];
                    PopulateLessonDropdown(lessons.ToArray());
                }

                Point screenPoint = btnSelectLesson.PointToScreen(Point.Empty);
                Point relativePoint = mainPanel.PointToClient(screenPoint);
                panelLessonDropdown.Location = new Point(relativePoint.X, relativePoint.Y + btnSelectLesson.Height + 3);

                panelLessonDropdown.BringToFront();
                panelLessonDropdown.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách bài học: " + ex.Message);
            }
        }


        private void BtnSelectLesson_Click(object sender, EventArgs e)
        {
            Point screenPoint = btnSelectLesson.PointToScreen(Point.Empty);
            Point relativePoint = mainPanel.PointToClient(screenPoint);
            panelLessonDropdown.Location = new Point(relativePoint.X, relativePoint.Y + btnSelectLesson.Height + 3);
            panelLessonDropdown.BringToFront();
            panelLessonDropdown.Visible = !panelLessonDropdown.Visible;
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (waveSource == null)
            {
                waveSource = new WaveInEvent();
                waveSource.WaveFormat = new WaveFormat(44100, 1);

                waveSource.DataAvailable += (s, a) =>
                {
                    waveFile?.Write(a.Buffer, 0, a.BytesRecorded);
                };

                waveSource.RecordingStopped += async (s, a) =>
                {
                    waveFile?.Dispose();
                    waveFile = null;
                    waveSource.Dispose();
                    waveSource = null;

                    string correctWord = lblWord.Text.Trim().ToLower();
                    var (recognizedText, accuracy, completeness, fluency) = await EvaluatePronunciation(correctWord, recordedFilePath);

                    if (string.IsNullOrWhiteSpace(recognizedText))
                    {
                        lblScoreCard.Text = "⚠ Không nhận diện";
                        scoreCardPanel.FillColor = Color.Orange;
                        lblScoreCard.ForeColor = Color.White;
                        scoreCardPanel.Visible = true;
                        return;
                    }

                    string spoken = recognizedText.Trim().ToLower().Replace(".", "").Replace(",", "");
                    int totalScore = (accuracy + completeness + fluency) / 3;

                    lblScoreCard.Text = $"🎯 {totalScore}%";

                    if (totalScore >= 90)
                    {
                        scoreCardPanel.FillColor = Color.White;
                        scoreCardPanel.BorderColor = Color.FromArgb(0, 200, 120);
                        lblScoreCard.ForeColor = Color.FromArgb(0, 150, 80);
                    }
                    else
                    {
                        scoreCardPanel.FillColor = Color.White;
                        scoreCardPanel.BorderColor = Color.Crimson;
                        lblScoreCard.ForeColor = Color.Crimson;

                        Task.Run(async () => {
                            await Task.Delay(1500);
                            var speech = new SpeechService();
                            speech.Speak(correctWord);
                        });
                    }

                    scoreCardPanel.Visible = true;
                    scoreCardPanel.BringToFront();
                    scoreCardTimer.Start();
                    DisplayFlashcard(currentFlashcardIndex);


                };

                waveFile = new WaveFileWriter(recordedFilePath, waveSource.WaveFormat);
                waveSource.StartRecording();
                btnRecord.Text = "⏹ Dừng";
            }
            else
            {
                waveSource.StopRecording();
                btnRecord.Text = "🎙";
            }
        }

        private async Task<(string recognizedText, int accuracyScore, int completenessScore, int fluencyScore)> EvaluatePronunciation(string expectedText, string audioPath)
        {
            var config = SpeechConfig.FromSubscription("9zdx2mlCJObizWuILwufeQH5JeWe548TceXwXSx0dgMKRrhthwGbJQQJ99BGACqBBLyXJ3w3AAAYACOGR21v", "southeastasia");
            config.SpeechRecognitionLanguage = "en-US";

            var pronunciationConfig = new PronunciationAssessmentConfig(
                expectedText,
                GradingSystem.HundredMark,
                Granularity.Word,
                enableMiscue: true
            );

            using var audioInput = AudioConfig.FromWavFileInput(audioPath);
            using var recognizer = new SpeechRecognizer(config, audioInput);
            pronunciationConfig.ApplyTo(recognizer);

            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                var assessmentResult = PronunciationAssessmentResult.FromResult(result);
                return (
                    result.Text,
                    (int)assessmentResult.AccuracyScore,
                    (int)assessmentResult.CompletenessScore,
                    (int)assessmentResult.FluencyScore
                );
            }
            else
            {
                MessageBox.Show("Không thể đánh giá phát âm. Lý do: " + result.Reason.ToString(), "Lỗi");
                return ("", 0, 0, 0);
            }
        }

        private int CalculateSimilarity(string s1, string s2)
        {
            int distance = LevenshteinDistance(s1, s2);
            int maxLength = Math.Max(s1.Length, s2.Length);
            return maxLength == 0 ? 100 : (int)((1.0 - (double)distance / maxLength) * 100);
        }

        private int LevenshteinDistance(string s, string t)
        {
            int[,] d = new int[s.Length + 1, t.Length + 1];
            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }

        private async void LoadFlashcardsByLesson(string selectedLesson)
        {
            if (string.IsNullOrEmpty(selectedLesson)) return;

            try
            {
                currentFlashcards = await firestoreService.GetFlashcardsByLessonAsync(selectedLesson);
                ShuffleFlashcards(currentFlashcards);

                currentFlashcardIndex = 0;
                flippedIndexes.Clear();
                DisplayFlashcard(currentFlashcardIndex);
                await UpdateProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải flashcard: " + ex.Message);
            }
        }
        private void ShuffleFlashcards(List<Flashcard> flashcards)
        {
            Random rng = new Random();
            int n = flashcards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var temp = flashcards[k];
                flashcards[k] = flashcards[n];
                flashcards[n] = temp;
            }
        }

        private async void btnShuffle_Click(object sender, EventArgs e)
        {
            if (currentFlashcards == null || currentFlashcards.Count == 0) return;

            ShuffleFlashcards(currentFlashcards);
            currentFlashcardIndex = 0;
            flippedIndexes.Clear();

            DisplayFlashcard(currentFlashcardIndex);
            await UpdateProgress();
        }
        private void DisplayFlashcard(int index)
        {
            if (currentFlashcards == null || currentFlashcards.Count == 0 || index >= currentFlashcards.Count)
            {
                lblWord.Text = "(Không có từ nào)";
                lblPhonetic.Text = "";
                lblDefinition.Text = "";
                lblMeaning.Text = "";
                progressBar.Value = 0;
                progressBar.Text = "0%";
                return;
            }

            Flashcard flashcard = currentFlashcards[index];
            lblWord.Text = flashcard.front.Word;
            lblPhonetic.Text = flashcard.front.Phonetic;
            lblDefinition.Text = flashcard.front.Definition;
            lblMeaning.Text = flashcard.back.Meaning;

            ShowFront();
        }

        private async void btnFlip_Click(object sender, EventArgs e)
        {
            if (isFront)
            {
                ShowBack();

                if (!flippedIndexes.Contains(currentFlashcardIndex))
                {
                    flippedIndexes.Add(currentFlashcardIndex);
                    await UpdateProgress();
                }
            }
            else
            {
                ShowFront();
            }
        }
        private void ShowFront()
        {
            lblWord.Visible = true;
            lblPhonetic.Visible = true;
            lblDefinition.Visible = true;
            lblMeaning.Visible = false;
            isFront = true;
        }

        private void ShowBack()
        {
            lblWord.Visible = false;
            lblPhonetic.Visible = false;
            lblDefinition.Visible = false;
            lblMeaning.Visible = true;
            isFront = false;
        }

        private async Task UpdateProgress()
        {
            if (currentFlashcards == null || currentFlashcards.Count == 0)
            {
                progressBar.Value = 0;
                progressBar.Text = "0%";
                return;
            }

            int flippedCount = flippedIndexes.Count;
            int total = currentFlashcards.Count;

            int progress = (int)((double)flippedCount / total * 100);
            progressBar.Value = Math.Min(progress, 100);
            progressBar.Text = $"{progress}%";

            string selectedLesson = btnSelectLesson.Text;
            if (!string.IsNullOrEmpty(selectedLesson))
            {
                try
                {
                    await progressService.SaveProgressAsync(currentUser.Uid, selectedLesson, progress);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi lưu tiến độ: " + ex.Message);
                }
            }

            if (flippedCount == total)
            {
                MessageBox.Show("🎉 Bạn đã hoàn thành bài học!", "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Nếu muốn lưu trạng thái hoàn thành:
                // await firestoreService.MarkLessonAsCompletedAsync(currentUser.Uid, selectedLesson);
                currentUser.Lessons += 1;

                bool success = await firebaseService.UpdateLessonAsync(currentUser.Username, currentUser.Lessons);

                return;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentFlashcards == null || currentFlashcards.Count == 0)
                return;

            List<int> unflippedIndexes = new List<int>();
            for (int i = 0; i < currentFlashcards.Count; i++)
            {
                if (!flippedIndexes.Contains(i))
                    unflippedIndexes.Add(i);
            }

            if (unflippedIndexes.Count > 0)
            {
                int nextUnflipped = unflippedIndexes.Find(i => i > currentFlashcardIndex);

                if (nextUnflipped != default)
                {
                    currentFlashcardIndex = nextUnflipped;
                }
                else
                {
                    Random rng = new Random();
                    currentFlashcardIndex = unflippedIndexes[rng.Next(unflippedIndexes.Count)];
                }

                DisplayFlashcard(currentFlashcardIndex);
            }
            else
            {
                MessageBox.Show("🎉 Bạn đã hoàn thành tất cả flashcard!", "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentFlashcards != null && currentFlashcardIndex > 0)
            {
                currentFlashcardIndex--;
                DisplayFlashcard(currentFlashcardIndex);
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "me";
            if (mainform.profile == null || mainform.profile.IsDisposed)
            {
                if (mainform.profile != null)
                    try { mainform.profile.Close(); } catch { }
                mainform.profile = new Profile(mainform, currentUser, this, callerName);
            }
            mainform.profile.Show();
        }

        private void Quiz_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.selecttopicform == null || mainform.selecttopicform.IsDisposed)
            {
                if (mainform.selecttopicform != null)
                    try { mainform.selecttopicform.Close(); } catch { }
                mainform.selecttopicform = new SelectTopicForm(mainform, currentUser);
            }
            mainform.selecttopicform.Show();
            this.Close();
        }

        private void btnForum_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "no";
            if (mainform.forum == null || mainform.forum.IsDisposed)
            {
                if (mainform.forum != null)
                    try { mainform.forum.Close(); } catch { }
                mainform.forum = new Forum(mainform, currentUser, this, callerName);
            }
            mainform.forum.Show();
        }
        
        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.home == null || mainform.home.IsDisposed)
            {
                if (mainform.home != null)
                    try { mainform.home.Close(); } catch { }
                mainform.home = new Home(mainform, currentUser);
            }
            mainform.home.Show();
            this.Close();
        }

        private void Bangxephang_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.leaderboard == null || mainform.leaderboard.IsDisposed)
            {
                if (mainform.leaderboard != null)
                    try { mainform.leaderboard.Close(); } catch { }
                mainform.leaderboard = new LeaderBoard(mainform, currentUser);
            }
            mainform.leaderboard.Show();
            this.Close();
        }

        private void Thachdau_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.room == null || mainform.room.IsDisposed)
            {
                if (mainform.room != null)
                    try { mainform.room.Close(); } catch { }
                mainform.room = new Room(mainform, currentUser);
            }
            mainform.room.Show();
            this.Close();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            var speech = new SpeechService();
            speech.Speak(lblWord.Text);
        }
        private void labelTitle_Click(object sender, EventArgs e)
        {
        }

        private void progressBar_ValueChanged(object sender, EventArgs e)
        {
        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e)
        {
        }

    }
}
