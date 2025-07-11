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
    public partial class History : BaseForm
    {
        //private string currentUserId;
        private readonly User currentUser;

        private FirestoreDb db;
        private Form1 mainform;

        public History(Form1 MainForm, User user)
        {
            InitializeComponent();
            currentUser = user;
            this.mainform = MainForm;
            this.Load += HistoryForm_Load;
        }

        private void SetupHistoryGrid()
        {
            dgvHistory.Columns.Clear();

            // Cột thời gian
            var colTime = new DataGridViewTextBoxColumn
            {
                Name = "colTime",
                HeaderText = "🕒 Thời gian",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 60F,
                MinimumWidth = 120
            };

            // Cột điểm
            var colScore = new DataGridViewTextBoxColumn
            {
                Name = "colScore",
                HeaderText = "🎯 Điểm",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 20F,
                MinimumWidth = 50,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            // Cột chi tiết (nút "Xem")
            var colDetail = new DataGridViewButtonColumn
            {
                Name = "colDetail",
                HeaderText = "🔍 Chi tiết",
                Text = "Xem",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 20F,
                MinimumWidth = 60,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            dgvHistory.Columns.AddRange(new DataGridViewColumn[] { colTime, colScore, colDetail });

            dgvHistory.ReadOnly = true;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.MultiSelect = false;
        }



        private void SetupDetailGrid()
        {
            dgvDetail.Columns.Clear();

            var colQuestion = new DataGridViewTextBoxColumn
            {
                Name = "colQuestion",
                HeaderText = "📝 Câu hỏi",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 40F,
                MinimumWidth = 100
            };

            var colChoice = new DataGridViewTextBoxColumn
            {
                Name = "colChoice",
                HeaderText = "✅ Đã chọn",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 30F,
                MinimumWidth = 80
            };

            var colAnswer = new DataGridViewTextBoxColumn
            {
                Name = "colAnswer",
                HeaderText = "📌 Đáp án",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 30F,
                MinimumWidth = 80
            };

            dgvDetail.Columns.AddRange(new DataGridViewColumn[] { colQuestion, colChoice, colAnswer });
            dgvDetail.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvDetail.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgvDetail.ReadOnly = true;
            dgvDetail.AllowUserToAddRows = false;
            dgvDetail.RowHeadersVisible = false;
        }


        private async Task LoadHistoryAsync()
        {
            dgvHistory.Rows.Clear();
            var db = FirestoreHelper.GetDb();
            var roomsRef = db.Collection("Rooms");
            var roomSnaps = await roomsRef.GetSnapshotAsync();

            foreach (var roomDoc in roomSnaps.Documents)
            {
                var answerRef = roomDoc.Reference.Collection("answers").Document(currentUser.Uid);
                var answerSnap = await answerRef.GetSnapshotAsync();
                if (!answerSnap.Exists) continue;

                var data = answerSnap.ToDictionary();
                if (!data.ContainsKey("score") || !data.ContainsKey("timestamp")) continue;

                int score = Convert.ToInt32(data["score"]);
                Timestamp timestamp = (Timestamp)data["timestamp"];
                string timeStr = timestamp.ToDateTime().ToString("g");

                int rowIndex = dgvHistory.Rows.Add(timeStr, score, "Xem");
                dgvHistory.Rows[rowIndex].Tag = roomDoc.Id; // lưu roomId để dùng khi bấm "Xem"
            }
        }

        private async void dgvHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Bấm vào cột thứ 2 (index 2) = cột "Chi tiết"
            if (e.RowIndex < 0 || e.ColumnIndex != 2) return;

            string roomId = dgvHistory.Rows[e.RowIndex].Tag?.ToString();
            if (string.IsNullOrEmpty(roomId)) return;

            dgvHistory.Visible = false;         // Ẩn lịch sử
            panelDetail.Visible = true;         // Hiện chi tiết

            await ShowDetailAsync(roomId);
        }

        private async Task ShowDetailAsync(string roomId)
        {
            dgvDetail.Rows.Clear();

            var db = FirestoreHelper.GetDb();
            var roomDoc = db.Collection("Rooms").Document(roomId);
            var roomSnap = await roomDoc.GetSnapshotAsync();
            var answerSnap = await roomDoc.Collection("answers").Document(currentUser.Uid).GetSnapshotAsync();

            if (!roomSnap.Exists || !answerSnap.Exists) return;

            var questionList = roomSnap.GetValue<List<object>>("questions");
            var answers = answerSnap.ToDictionary();

            int correctCount = 0;

            foreach (var entry in questionList.Select((q, index) => new { q, index }))
            {
                var qData = entry.q as Dictionary<string, object>;
                if (qData == null) continue;

                string questionText = qData["questionText"].ToString();
                string correctAnswer = qData["correctAnswer"].ToString();
                string chosen = answers.ContainsKey($"q{entry.index}") ? answers[$"q{entry.index}"].ToString() : "(chưa chọn)";

                int rowIndex = dgvDetail.Rows.Add(questionText, chosen, correctAnswer);

                if (chosen != correctAnswer)
                {
                    dgvDetail.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 228, 232); // Hồng nhạt
                }
                else
                {
                    correctCount++;
                }
            }

            panelDetail.Visible = true;

            int total = questionList.Count;
            double rate = total > 0 ? (double)correctCount / total : 0;
            labelRate.Text = $"Tỉ lệ đúng: {(rate * 100):0.#}%";
            panelRate.Visible = true;
            panelHappy.Visible = false;
            panelSad.Visible = false;
            Gioi.Visible = false;
            Do.Visible = false;

            if (rate >= 0.5)
            {
                panelHappy.Visible = true;
                Gioi.Visible = true;
            }
            else
            {
                panelSad.Visible = true;
                Do.Visible = true;
            }

        }





        private void btnCloseDetail_Click(object sender, EventArgs e)
        {
            panelDetail.Visible = false;
            dgvHistory.Visible = true;
            panelHappy.Visible = false;
            panelSad.Visible = false;
            Gioi.Visible = false;
            Do.Visible = false;
            panelRate.Visible = false;

        }


        private async void HistoryForm_Load(object sender, EventArgs e)
        {
            SetupHistoryGrid();
            SetupDetailGrid();
            panelDetail.Visible = false;
            await LoadHistoryAsync();
            dgvHistory.CellClick += dgvHistory_CellClick;
            btnCloseDetail.Click += btnCloseDetail_Click;
            panelHappy.Visible = false;
            panelSad.Visible = false;
            Gioi.Visible = false;
            Do.Visible = false;
            panelRate.Visible = false;

        }
        private void Room_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.room != null)
                try { mainform.room.Close(); } catch { }
            mainform.room = new Room(mainform, currentUser);
            mainform.room.Show();
            this.Close();
        }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }
    }
}
