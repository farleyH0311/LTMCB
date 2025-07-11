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
    public partial class LeaderBoard : BaseForm
    {
        private List<UserProfile> allProfiles = new List<UserProfile>();
        private readonly User currentUser;
        private Form1 mainform;
        public LeaderBoard(Form1 MainForm, User user)
        {
            InitializeComponent();
            currentUser = user;
            this.mainform = MainForm;
            this.Load += new EventHandler(LeaderBoard_Load);
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private Panel CreateLeaderboardHeader()
        {
            var header = new Panel
            {
                Width = 450,
                Height = 50,
                BackColor = Color.MidnightBlue,
                Margin = new Padding(5),
                Padding = new Padding(5),
                ForeColor = Color.White,
            };

            var lblRank = new Label
            {
                Text = "🏅",
                Font = new Font("Segoe UI Emoji", 12, FontStyle.Bold),
                Size = new Size(50, 40),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 10),
                BackColor = Color.Transparent
            };

            var lblName = new Label
            {
                Text = "👤 Tên",
                Font = new Font("Comic Sans MS", 10, FontStyle.Bold),
                Size = new Size(250, 45),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(80, 5),
                BackColor = Color.Transparent
            };

            var lblScore = new Label
            {
                Text = "⭐",
                Font = new Font("Segoe UI Emoji", 12, FontStyle.Bold),
                Size = new Size(50, 40),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(header.Width - 80, 2),
                BackColor = Color.Transparent
            };

            header.Controls.Add(lblRank);
            header.Controls.Add(lblName);
            header.Controls.Add(lblScore);

            return header;
        }


        private Guna.UI2.WinForms.Guna2Panel CreateLeaderboardItem(int rank, string username, int score)
        {
            Color fillColor = rank == 1 ? Color.Gold :
                              rank == 2 ? Color.Silver :
                              rank == 3 ? Color.Peru :
                              Color.WhiteSmoke;

            var panel = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = 450,
                Height = 50,
                BorderRadius = 20,
                FillColor = fillColor,
                Margin = new Padding(5),
                Padding = new Padding(5),
                BorderThickness = 0
            };

            string medalEmoji = rank == 1 ? "🥇" :
                                rank == 2 ? "🥈" :
                                rank == 3 ? "🥉" : $"#{rank}";

            var lblRank = new Label
            {
                Text = medalEmoji,
                Font = new Font("Segoe UI Emoji", 12, FontStyle.Bold),

                Size = new Size(50, 40),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 10),
                BackColor = Color.Transparent
            };

            var lblName = new Label
            {
                Text = username,
                Font = new Font("Comic Sans MS", 12),
                Size = new Size(240, 35),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(80, 10),
                BackColor = Color.Transparent
            };

            var lblScore = new Label
            {
                Text = score.ToString(),
                Font = new Font("Comic Sans MS", 12, FontStyle.Bold),
                Size = new Size(70, 40),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(panel.Width - 100, 10),
                BackColor = Color.Transparent
            };

            panel.Controls.Add(lblRank);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblScore);

            return panel;
        }

        private async Task LoadLeaderboardToFlowPanel()
        {
            var service = new LeaderboardService();
            var profiles = await service.GetLeaderboardAsync();
            allProfiles = await service.GetLeaderboardAsync();
            DisplayProfiles(allProfiles);

            var sorted = profiles
                .OrderByDescending(p => p.Score)
                .Select((p, index) => new { Rank = index + 1, p.Username, p.Score })
                .ToList();

            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.Add(CreateLeaderboardHeader());

            foreach (var user in sorted)
            {
                var item = CreateLeaderboardItem(user.Rank, user.Username, user.Score);
                flowLayoutPanel1.Controls.Add(item);
            }
        }
        private async void LeaderBoard_Load(object sender, EventArgs e)
        {
            await LoadLeaderboardToFlowPanel();
        }
        private void DisplayProfiles(List<UserProfile> profiles)
        {
            var sorted = profiles
                .OrderByDescending(p => p.Score)
                .Select((p, index) => new { Rank = index + 1, p.Username, p.Score })
                .ToList();

            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.Add(CreateLeaderboardHeader());

            foreach (var user in sorted)
            {
                var item = CreateLeaderboardItem(user.Rank, user.Username, user.Score);
                flowLayoutPanel1.Controls.Add(item);
            }
        }


        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var filtered = allProfiles
                .Where(p => p.Username != null && p.Username.ToLower().Contains(keyword))
                .ToList();

            DisplayProfiles(filtered);
        }

        private void Trangcanhan_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "me";
            if (mainform.profile != null)
                try { mainform.profile.Close(); } catch { }
            mainform.profile = new Profile(mainform, currentUser, this, callerName);
            mainform.profile.Show();

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.selecttopicform != null)
                try { mainform.selecttopicform.Close(); } catch { }
            mainform.selecttopicform = new SelectTopicForm(mainform, currentUser);
            mainform.selecttopicform.Show();
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
        private void Bangxephang_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.leaderboard != null)
                try { mainform.leaderboard.Close(); } catch { }
            mainform.leaderboard = new LeaderBoard(mainform, currentUser);
            mainform.leaderboard.Show();
            this.Close();
        }
        private void Diendan_Click(object sender, EventArgs e)
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
            if (mainform.home != null)
                try { mainform.home.Close(); } catch { }
            mainform.home = new Home(mainform, currentUser);
            mainform.home.Show();
            this.Close();
        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
