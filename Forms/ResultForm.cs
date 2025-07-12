using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Firestore;
using WordUp.Helpers;
using WordUp.Models;
using WordUp.Services;

namespace WordUp.Forms
{
    public partial class ResultForm : BaseForm
    {
        private readonly User currentUser;
        private readonly string roomId;
        private Form1 mainform;

        public ResultForm(Form1 MainForm, User user, string roomId)
        {
            InitializeComponent();
            this.mainform = MainForm;
            this.currentUser = user;
            this.roomId = roomId;
            this.Load += ResultForm_Load;
            btnReplay.Click += btnReplay_Click;
            btnExit.Click += btnExit_Click;
        }

        private async void ResultForm_Load(object sender, EventArgs e)
        {
            try
            {
                var db = FirestoreHelper.GetDb();

                // Lấy danh sách người chơi và username
                var playersRef = db.Collection("Rooms").Document(roomId).Collection("players");
                var playersSnapshot = await playersRef.GetSnapshotAsync();
                var usernameMap = playersSnapshot.Documents.ToDictionary(
                    doc => doc.Id,
                    doc => doc.ContainsField("username") ? doc.GetValue<string>("username") : doc.Id
                );

                // Lấy điểm từ answers
                var answersRef = db.Collection("Rooms").Document(roomId).Collection("answers");
                var answersSnapshot = await answersRef.GetSnapshotAsync();

                var ranking = new List<(string username, int score)>();

                foreach (var doc in answersSnapshot.Documents)
                {
                    string uid = doc.Id;
                    int score = doc.ContainsField("score") ? doc.GetValue<int>("score") : 0;
                    string username = usernameMap.ContainsKey(uid) ? usernameMap[uid] : uid;

                    ranking.Add((username, score));
                }
                //Sort rank
                var enrichedRanking = answersSnapshot.Documents.Select(doc => new
                {
                    username = usernameMap.ContainsKey(doc.Id) ? usernameMap[doc.Id] : doc.Id,
                    score = doc.ContainsField("score") ? doc.GetValue<int>("score") : 0,
                    timestamp = doc.ContainsField("timestamp") ? doc.GetValue<Timestamp>("timestamp").ToDateTime() : DateTime.MaxValue
                }).OrderByDescending(x => x.score)
                .ThenBy(x => x.timestamp) // ai hoàn thành sớm hơn xếp cao hơn nếu cùng điểm
                .ToList();


                // Hiển thị điểm người dùng hiện tại
                var current = ranking.FirstOrDefault(x => x.username == currentUser.Username);
                labelYourScore.Text = $"Your score: {current.score}";

                // Hiển thị bảng xếp hạng
                flowLayoutPanel.Controls.Clear();
                int rank = 1;

                foreach (var player in ranking)
                {
                    var rowPanel = new TableLayoutPanel
                    {
                        ColumnCount = 2,
                        Width = flowLayoutPanel.Width - 25,
                        Height = 40,
                        Margin = new Padding(5),
                        BackColor = player.username == currentUser.Username ? Color.LightGoldenrodYellow : Color.PeachPuff
                    };

                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Tên chiếm 60%
                    rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Điểm chiếm 40%

                    Label nameLabel = new Label
                    {
                        Text = $"{rank++}. {player.username}",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Font = new Font("Segoe UI", 11, FontStyle.Bold),
                        ForeColor = player.username == currentUser.Username ? Color.DarkGreen : Color.Firebrick,
                        AutoSize = false
                    };

                    Label scoreLabel = new Label
                    {
                        Text = $"{player.score} điểm",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleRight,
                        Font = new Font("Segoe UI", 11, FontStyle.Bold),
                        ForeColor = player.username == currentUser.Username ? Color.DarkGreen : Color.Firebrick,
                        AutoSize = false
                    };

                    rowPanel.Controls.Add(nameLabel, 0, 0);
                    rowPanel.Controls.Add(scoreLabel, 1, 0);

                    flowLayoutPanel.Controls.Add(rowPanel);
                }

                // Cộng điểm thưởng theo số lượng người chơi
                var firebaseService = new FirebaseService();
                int totalPlayers = ranking.Count;

                for (int i = 0; i < totalPlayers; i++)
                {
                    string username = ranking[i].username;
                    int bonus = 0;

                    if (totalPlayers == 1) bonus = 5;
                    
                    else if (totalPlayers == 2) bonus = (i == 0) ? 10 : 5;
                    else
                    {
                        if (i == 0) bonus = 20;
                        else if (i == 1) bonus = 15;
                        else if (i == 2) bonus = 10;
                        else bonus = 5;
                    }

                    if (username == currentUser.Username)
                    {
                        currentUser.Score += bonus;
                        await firebaseService.UpdateScoreAsync(username, currentUser.Score);
                        MessageBox.Show($"Bạn được thưởng thêm {bonus} điểm! Tổng điểm hiện tại: {currentUser.Score}");
                    }
                    else
                    {
                        await firebaseService.AddBonusScoreAsync(username, bonus);
                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải kết quả: " + ex.Message);
            }


        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.room != null)
                try { mainform.room.Close(); } catch { }
            mainform.room = new Room(mainform, currentUser); // Hoặc Prepare(currentUser)
            mainform.room.Show();
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.home != null)
                try { mainform.home.Close(); } catch { }
            mainform.home = new Home(mainform, currentUser);
            mainform.home.Show();
            this.Close();
        }
    }
}
