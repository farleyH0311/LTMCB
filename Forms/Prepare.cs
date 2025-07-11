using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordUp.Helpers;
using WordUp.Models;
using Google.Cloud.Firestore;
using System.Threading;
using System.Drawing.Drawing2D;
using WordUp.Services;
using System.Net.NetworkInformation;

namespace WordUp.Forms
{
    public partial class Prepare : BaseForm
    {
        private readonly User currentUser;
        private readonly string roomId;
        private FirestoreChangeListener listener;
        private FirestoreChangeListener roomListener;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private string hostUid = "";
        private bool gameStarted = false;
        private Form1 mainform;

        public Prepare(Form1 MainForm, User user, string roomId)
        {
            InitializeComponent();
            this.mainform = MainForm;
            this.currentUser = user;
            this.roomId = roomId;
            lblRoomId.Text = "Room ID: " + roomId;
            this.FormClosing += Prepare_FormClosing;
        }

        private Panel CreatePlayerPanel(string name, bool isHost)
        {
            Panel userPanel = new Panel
            {
                Width = 140,
                Height = 45,
                BackColor = Color.FromArgb(204, 229, 255),
                Margin = new Padding(8),
                Padding = new Padding(2),
                Cursor = Cursors.Hand
            };

            userPanel.Paint += (s, e) =>
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 10;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(userPanel.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(userPanel.Width - radius, userPanel.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, userPanel.Height - radius, radius, radius, 90, 90);
                    path.CloseFigure();
                    userPanel.Region = new Region(path);
                }
            };

            Label nameLabel = new Label
            {
                Text = isHost ? "👑 " + name : name,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 112),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            userPanel.Controls.Add(nameLabel);
            return userPanel;
        }

        public async Task LoadPlayersAsync(string roomId, FlowLayoutPanel panel, string hostUid)
        {
            var db = FirestoreHelper.GetDb();
            var playersRef = db.Collection("Rooms").Document(roomId).Collection("players");
            var snapshot = await playersRef.GetSnapshotAsync();

            panel.Controls.Clear();
            foreach (var doc in snapshot.Documents)
            {
                string uid = doc.Id;
                string name = doc.GetValue<string>("name");
                bool isHost = uid == hostUid;
                var playerPanel = CreatePlayerPanel(name, isHost);
                panel.Controls.Add(playerPanel);
            }
        }

        public void ListenToPlayers(string roomId, FlowLayoutPanel panel, string hostUid)
        {
            var db = FirestoreHelper.GetDb();
            listener = db.Collection("Rooms").Document(roomId).Collection("players")
                .Listen(snapshot =>
                {
                    panel.Invoke(new Action(() =>
                    {
                        panel.Controls.Clear();
                        foreach (var doc in snapshot.Documents)
                        {
                            string uid = doc.Id;
                            string name = doc.GetValue<string>("name");
                            bool isHost = uid == hostUid;
                            var playerPanel = CreatePlayerPanel(name, isHost);
                            panel.Controls.Add(playerPanel);
                        }
                    }));
                });
        }

        private void ListenToRoomStart()
        {
            var db = FirestoreHelper.GetDb();
            var roomRef = db.Collection("Rooms").Document(roomId);
            roomListener = roomRef.Listen(snapshot =>
            {
                if (snapshot.Exists &&
                    snapshot.TryGetValue<bool>("started", out bool started) &&
                    started && !gameStarted)
                {
                    gameStarted = true; // Ngăn lặp lại nhiều lần
                    this.Invoke(new Action(async () =>
                    {
                        this.Hide();
                        if (mainform.game != null)
                            try { mainform.game.Close(); } catch { }
                        mainform.game = new Game(mainform, currentUser, roomId);
                        mainform.game.Show();

                        if (listener != null)
                            await listener.StopAsync(cts.Token);
                        if (roomListener != null)
                            await roomListener.StopAsync(cts.Token);
                        
                    }));
                }
            });
        }

        private async void Prepare_Load(object sender, EventArgs e)
        {
            var db = FirestoreHelper.GetDb();
            var roomRef = db.Collection("Rooms").Document(roomId);
            var roomSnap = await roomRef.GetSnapshotAsync();

            if (roomSnap.Exists && roomSnap.ContainsField("hostUid"))
            {
                hostUid = roomSnap.GetValue<string>("hostUid");
                Batdau.Enabled = (hostUid == currentUser.Uid);
            }

            await LoadPlayersAsync(roomId, PlayerPanel, hostUid);
            ListenToPlayers(roomId, PlayerPanel, hostUid);
            ListenToRoomStart(); // 🔔 Bắt đầu nghe khi room được bắt đầu
        }

        private async void Batdau_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentUser.Uid != hostUid)
                {
                    MessageBox.Show("Chỉ chủ phòng mới được phép bắt đầu trò chơi.");
                    return;
                }

                var db = FirestoreHelper.GetDb();
                var roomRef = db.Collection("Rooms").Document(roomId);

                await roomRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "started", true }
                });

                // Không mở game tại đây nữa — sẽ được xử lý bởi roomListener
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi bắt đầu trò chơi: " + ex.Message);
            }
        }

        private async void Exit_Click(object sender, EventArgs e)
        {
            try
            {
                var db = FirestoreHelper.GetDb();
                var roomRef = db.Collection("Rooms").Document(roomId);
                var roomSnap = await roomRef.GetSnapshotAsync();

                if (!roomSnap.Exists)
                {
                    MessageBox.Show("Phòng không tồn tại.");
                    return;
                }

                var roomData = roomSnap.ToDictionary();
                bool isHost = roomData.ContainsKey("hostUid") && roomData["hostUid"].ToString() == currentUser.Uid;

                if (isHost)
                {
                    DialogResult result = MessageBox.Show(
                        "Bạn là chủ phòng. Thoát sẽ giải tán phòng. Bạn có chắc chắn muốn thoát?",
                        "Xác nhận thoát",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.Yes)
                    {
                        await RoomService.DeleteRoomImmediatelyAsync(db, roomId);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    var playerRef = roomRef.Collection("players").Document(currentUser.Uid);
                    await playerRef.DeleteAsync();
                }

                if (listener != null)
                    await listener.StopAsync(cts.Token);
                if (roomListener != null)
                    await roomListener.StopAsync(cts.Token);

                this.Hide();
                if (mainform.room != null)
                    try { mainform.room.Close(); } catch { }
                mainform.room = new Room(mainform, currentUser);
                mainform.room.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thoát phòng: " + ex.Message);
            }
        }

        private async void Prepare_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (listener != null)
            {
                try { await listener.StopAsync(cts.Token); } catch { }
            }

            if (roomListener != null)
            {
                try { await roomListener.StopAsync(cts.Token); } catch { }
            }
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

        private void Quiz_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.selecttopicform != null)
                try { mainform.selecttopicform.Close(); } catch { }
            mainform.selecttopicform = new SelectTopicForm(mainform, currentUser);
            mainform.selecttopicform.Show();
            this.Close();
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
            if (mainform.leaderboard == null || mainform.home.IsDisposed)
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
            if (mainform.room != null)
                try { mainform.room.Close(); } catch { }
            mainform.room = new Room(mainform, currentUser);
            mainform.room.Show();
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
    }
}
