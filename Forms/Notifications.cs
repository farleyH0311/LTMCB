using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WordUp.Models;
using WordUp.Services;
using WordUp.Controllers;

namespace WordUp.Forms
{
    public partial class Notifications : BaseForm
    {
        private readonly User currentUser;
        private readonly FirebaseService _firebaseService;
        private bool isLoading = false;
        private bool isOpeningForum = false;
        private Form1 mainform;

        public Notifications(Form1 MainForm, User user)
        {
            InitializeComponent();
            _firebaseService = new FirebaseService();
            currentUser = user;
            this.mainform = MainForm;
            InitFlowPanel();
            LoadNotifications();
        }

        private void InitFlowPanel()
        {
            flowPanel.Location = new Point(340, 164);
            flowPanel.Size = new Size(600, 400);
            flowPanel.AutoScroll = true;
            flowPanel.WrapContents = false;
            flowPanel.FlowDirection = FlowDirection.TopDown;
            flowPanel.Padding = new Padding(10);
            flowPanel.BackColor = Color.Transparent;
        }

        private async void LoadNotifications()
        {
            isLoading = true;
            var notifications = _firebaseService.GetNotificationsWithId(currentUser.Username);
            flowPanel.Controls.Clear();

            if (notifications != null && notifications.Count > 0)
            {
                notifications.Sort((x, y) => y.Noti.Timestamp.CompareTo(x.Noti.Timestamp));

                foreach (var (id, notification) in notifications)
                {

                    string time = notification.Timestamp.ToString("HH:mm dd/MM");
                    string message = notification.Type switch
                    {
                        "like" => $"{notification.SenderId} đã thích bài viết của bạn\n                                                          🕒 {time}",
                        "comment" => $"{notification.SenderId} đã bình luận bài viết của bạn\n                                                          🕒 {time}",
                        _ => "Loại thông báo không xác định"
                    };

                    var bubble = new NotificationBubble(message, notification.IsRead, notification.PostId, id);

                    bubble.BubbleClicked += async (s, e) =>
                    {
                        if (isOpeningForum) return;
                        isOpeningForum = true;

                        await _firebaseService.UpdateNotificationReadStatus(id, currentUser.Username);
                        this.Hide();
                        if (mainform.forum != null)
                            try { mainform.forum.Close(); } catch { }
                        mainform.forum = new Forum(mainform, currentUser, this, "noti");
                        mainform.forum.OpenPostDetail(notification.PostId);
                        mainform.forum.Show();
                        

                        isOpeningForum = false;
                    };

                    flowPanel.Controls.Add(bubble);
                }
            }
            else
            {
                MessageBox.Show("Không có thông báo nào.");
            }

            isLoading = false;
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
            if (mainform.home == null || mainform.IsDisposed)
            {
                if (mainform.home != null)
                    try { mainform.home.Close(); } catch { }
                mainform.home = new Home(mainform, currentUser);
            }
            mainform.home.Show();
            this.Close();
        }

        private void btnNoti_Click(object sender, EventArgs e)
        {
            LoadNotifications();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
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
        public void SetRoundedRegion(Control control, int radius)
        {
            Rectangle bounds = control.ClientRectangle;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            control.Region = new Region(path);
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
