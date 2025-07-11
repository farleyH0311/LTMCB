using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using WordUp.Models;
using WordUp.Services;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace WordUp.Forms
{
    public partial class Home : BaseForm
    {
        private WaveInEvent waveSource = null;
        private WaveFileWriter waveFile = null;
        private readonly string recordedFilePath = Path.Combine(Path.GetTempPath(), "voicechat.wav");
        private string currentPracticeQuestion = "";
        private readonly System.Speech.Synthesis.SpeechSynthesizer synthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
        private readonly User currentUser;
        private readonly SpeechService speechService = new SpeechService();
        private string currentEnglishWord = "";
        private static readonly HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        private Form1 mainform;

        public Home(Form1 MainForm, User user)
        {
            InitializeComponent();
            currentUser = user;
            this.mainform = MainForm;
        }

        private void Home_Load(object sender, EventArgs e)
        {           
        }
        private void btnVoiceChat_Click(object sender, EventArgs e)
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

                    txtKetQua.Text = "⏳ Đang nhận dạng giọng nói...";

                    string userText = await TranscribeAudioWithAzure(recordedFilePath);

                    if (string.IsNullOrWhiteSpace(userText))
                    {
                        txtKetQua.Text = "⚠ Không nhận được giọng nói.";
                        btnVoiceChat.Text = "🎙 Voice Chat AI";
                        return;
                    }

                    txtTuKhoa.Text = userText;

                    if (!string.IsNullOrEmpty(currentPracticeQuestion))
                    {
                        txtKetQua.Text = "⏳ Đang gửi tới AI để đánh giá phản xạ...";

                        string prompt = $"Tôi đã hỏi: \"{currentPracticeQuestion}\". Học sinh trả lời: \"{userText}\". " +
                                        "Hãy thực hiện các yêu cầu sau:\n" +
                                        "1️⃣ Đánh giá độ chính xác nội dung, ngữ pháp, từ vựng (cho điểm 1-10).\n" +
                                        "2️⃣ ĐÁNH GIÁ PHÁT ÂM (cho điểm 1-10), chỉ ra các từ hoặc âm học sinh phát âm chưa rõ hoặc sai, cách khắc phục cụ thể.\n" +
                                        "3️⃣ Gợi ý cách diễn đạt lại câu này cho tự nhiên hơn nếu cần.\n" +
                                        "Trả lời rõ ràng, dễ hiểu, liệt kê cụ thể lỗi phát âm nếu có.";

                        string aiReply = await AI.AskGeminiAsync(prompt);
                        txtKetQua.Text = $"📘 AI (Đánh giá phản xạ):\r\n{aiReply.Replace("\\n", "\r\n").Replace("\n", "\r\n")}";

                        currentPracticeQuestion = "";
                    }
                    else
                    {
                        txtKetQua.Text = "✅ Đã nhận giọng nói. Hãy bấm [Hỏi AI] để hỏi hoặc [Phản xạ AI] để tạo câu hỏi.";
                    }

                    btnVoiceChat.Text = "🎙 Voice Chat AI";
                };

                waveFile = new WaveFileWriter(recordedFilePath, waveSource.WaveFormat);
                waveSource.StartRecording();
                btnVoiceChat.Text = "⏹ Dừng";
            }
            else
            {
                waveSource.StopRecording();
                btnVoiceChat.Text = "🎙 Voice Chat AI";
            }
        }


        private async void btnPracticeByTopic_Click(object sender, EventArgs e)
        {
            string topic = txtTuKhoa.Text.Trim();
            if (string.IsNullOrWhiteSpace(topic))
            {
                MessageBox.Show("Vui lòng nhập chủ đề vào ô tìm kiếm trước khi tạo câu hỏi.");
                return;
            }

            txtKetQua.Text = "⏳ Đang tạo câu hỏi phản xạ...";

            string prompt = $"Hãy tạo một câu hỏi tiếng Anh đơn giản, phù hợp cho người học phản xạ nói, theo chủ đề: \"{topic}\". " +
                            $"Chỉ trả về câu hỏi tiếng Anh duy nhất, không cần giải thích.";

            string generatedQuestion = await AI.AskGeminiAsync(prompt);

            if (!string.IsNullOrWhiteSpace(generatedQuestion))
            {
                currentPracticeQuestion = generatedQuestion.Trim();
                txtKetQua.Text = "🤖 Câu hỏi: " + currentPracticeQuestion;
                synthesizer.SpeakAsync(currentPracticeQuestion);
            }
            else
            {
                txtKetQua.Text = "⚠ Không tạo được câu hỏi. Vui lòng thử lại.";
            }
        }
        
        private async void btnDoiCauHoi_Click(object sender, EventArgs e)
        {
            string topic = txtTuKhoa.Text.Trim();

            if (string.IsNullOrWhiteSpace(topic))
            {
                topic = "daily life"; 
            }

            txtKetQua.Text = "⏳ Đang tạo câu hỏi phản xạ mới...";

            string prompt = $"Hãy gợi ý một câu hỏi phản xạ tiếng Anh mới, đơn giản, dễ luyện nói, phù hợp cho người học tiếng Anh, theo chủ đề: \"{topic}\". Chỉ trả về câu hỏi, không cần giải thích.";

            string newQuestion = await AI.AskGeminiAsync(prompt);

            if (!string.IsNullOrWhiteSpace(newQuestion))
            {
                currentPracticeQuestion = newQuestion.Trim();
                txtKetQua.Text = "🤖 Câu hỏi mới: " + currentPracticeQuestion;
                synthesizer.SpeakAsync(currentPracticeQuestion);
            }
            else
            {
                txtKetQua.Text = "⚠ Không tạo được câu hỏi mới, vui lòng thử lại.";
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                if (synthesizer != null)
                {
                    synthesizer.SpeakAsyncCancelAll();
                }
                txtKetQua.Clear();
                txtTuKhoa.Clear();
                currentPracticeQuestion = "";

                if (waveSource != null)
                {
                    try
                    {
                        waveSource.StopRecording();
                    }
                    catch {  }

                    waveSource.Dispose();
                    waveSource = null;
                }

                if (waveFile != null)
                {
                    waveFile.Dispose();
                    waveFile = null;
                }
                if (File.Exists(recordedFilePath))
                {
                    try
                    {
                        File.Delete(recordedFilePath);
                    }
                    catch { }
                }
                btnVoiceChat.Text = "🎙 Voice Chat AI";

                txtKetQua.Text = "✅ Đã reset, sẵn sàng bắt đầu mới.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi reset: {ex.Message}");
            }
        }



        private async Task<string> TranscribeAudioWithAzure(string audioPath)
        {
            var config = SpeechConfig.FromSubscription("9zdx2mlCJObizWuILwufeQH5JeWe548TceXwXSx0dgMKRrhthwGbJQQJ99BGACqBBLyXJ3w3AAAYACOGR21v", "southeastasia");
            config.SpeechRecognitionLanguage = "en-US";

            using var audioInput = AudioConfig.FromWavFileInput(audioPath);
            using var recognizer = new SpeechRecognizer(config, audioInput);

            var result = await recognizer.RecognizeOnceAsync();
            return result.Reason == ResultReason.RecognizedSpeech ? result.Text : "";
        }
        
        private void btnProfile_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "me";
            if (mainform.profile != null)
                try { mainform.profile.Close(); } catch { }
            mainform.profile = new Profile(mainform, currentUser, this, callerName);
            mainform.profile.Show();
        }

        private void btnForum_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "no";
            if (mainform.forum != null)
                try { mainform.forum.Close(); } catch { }
            mainform.forum = new Forum(mainform, currentUser, this, callerName);
            mainform.forum.Show();
        }

        private void btnNoti_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.notifications != null)
                try { mainform.notifications.Close(); } catch { }
            mainform.notifications = new Notifications(mainform, currentUser);
            mainform.notifications.Show();
            this.Close();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.selecttopicform != null)
                try { mainform.selecttopicform.Close(); } catch { }
            mainform.selecttopicform = new SelectTopicForm(mainform, currentUser);
            mainform.selecttopicform.Show();
            this.Close();
        }

        private void message_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.chat != null)
                try { mainform.chat.Close(); } catch { }
            mainform.chat = new Chat(mainform, currentUser);
            mainform.chat.Show();
            this.Close();
        }

        private void Bangxephang_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.leaderboard != null)
                try { mainform.leaderboard.Close(); } catch { }
            mainform.leaderboard = new LeaderBoard(mainform, currentUser);
            mainform.leaderboard.Show();
            this.Close();
        }

        private void btnFlashcard_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.flashcardform != null)
                try { mainform.flashcardform.Close(); } catch { }
            mainform.flashcardform = new FlashcardForm(mainform, currentUser, currentUser.Uid);
            mainform.flashcardform.Show();
            this.Close();
        }
        private void Thachdau_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.room != null)
                try { mainform.room.Close(); } catch { }
            mainform.room = new Room(mainform, currentUser);
            mainform.room.Show();
            this.Close();
        }
        private void btnOut_Click(object sender, EventArgs e)
        {
            mainform.Close();
            this.Close();
            mainform = new Form1();
            mainform.Show();

        }

        private async void btnTraTu_Click(object sender, EventArgs e)
        {
            string input = txtTuKhoa.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Vui lòng nhập từ cần tra.");
                return;
            }

            txtKetQua.Text = "⏳ Đang dịch...";

            bool isEnglish = IsEnglish(input);

            if (isEnglish)
            {
                string vi = await Translate(input, "en", "vi");
                currentEnglishWord = input;
                txtKetQua.Text = $"📘 Nghĩa tiếng Việt: {vi}";
            }
            else
            {
                string en = await Translate(input, "vi", "en");
                currentEnglishWord = en;
                txtKetQua.Text = $"📘 Dịch sang tiếng Anh: {en}";
            }
        }

        private void btnPhatAm_Click(object sender, EventArgs e)
        {
            string textToSpeak = txtKetQua.SelectedText.Trim();

            if (string.IsNullOrWhiteSpace(textToSpeak))
            {
                textToSpeak = currentEnglishWord;
                if (string.IsNullOrWhiteSpace(textToSpeak))
                {
                    MessageBox.Show("Vui lòng nhập từ cần phát âm hoặc tô đen kết quả cần đọc.");
                    return;
                }
            }
            textToSpeak = textToSpeak.Replace("🔊", "").Trim();
            if (Regex.IsMatch(textToSpeak, "[a-zA-Z/]"))
            {
                synthesizer.SpeakAsync(textToSpeak);
            }
            else
            {
                MessageBox.Show("Không có nội dung tiếng Anh hoặc âm tiết IPA để đọc.");
            }
        }

        private async Task<string> Translate(string input, string from, string to)
        {
            try
            {
                string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={Uri.EscapeDataString(input)}";
                var response = await httpClient.GetStringAsync(url);
                JArray arr = JArray.Parse(response);
                return arr[0][0][0]?.ToString() ?? "Không thể dịch.";
            }
            catch
            {
                return "❌ Lỗi khi dịch. Hãy thử lại.";
            }
        }

        private bool IsEnglish(string input)
        {
            foreach (char c in input)
            {
                if (c >= 128) return false;
            }
            return true;
        }
        private async void btnHoiAI_Click(object sender, EventArgs e)
        {
            string question = txtTuKhoa.Text;
            string answer = await AI.AskGeminiAsync(question);
            txtKetQua.Text = answer.Replace("\n", "\r\n");
        }

    }
}
