using WordUp.Services;
using WordUp.Forms;
using System.Drawing.Drawing2D;
using WordUp.Models;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace WordUp
{
    public partial class Form1 : BaseForm
    {
        public Chat chat;
        public FlashcardForm flashcardform;
        public ForgotPassword forgotpassword;
        public Forum forum;
        public Game game;
        public HDSD hdsd;
        public History history;
        public Home home;
        public LeaderBoard leaderboard;
        public LoginForm loginform;
        public Notifications notifications;
        public PractiseQuiz practisequiz;
        public Prepare prepare;
        public Profile profile;
        public RegisterUI registerui;
        public ResultForm resultform;
        public Room room;
        public SelectTopicForm selecttopicform;
        public PractiseQuizForm practisequizform;

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            Task.Run(() => checking());
        }

        void checking()
        {
            while (true)
            {
                int ended = 0;
                for (int i = 0; i < 1000; i++)
                {
                    Thread.Sleep(100);
                    if (this.Visible)
                        continue;
                    if (chat != null && chat.Visible)
                        continue;
                    if (flashcardform != null && flashcardform.Visible)
                        continue;
                    if (forgotpassword != null && forgotpassword.Visible)
                        continue;
                    if (forum != null && forum.Visible)
                        continue;
                    if (game != null && game.Visible)
                        continue;
                    if (hdsd != null && hdsd.Visible)
                        continue;
                    if (history != null && history.Visible)
                        continue;
                    if (home != null && home.Visible)
                        continue;
                    if (leaderboard != null && leaderboard.Visible)
                        continue;
                    if (loginform != null && loginform.Visible)
                        continue;
                    if (notifications != null && notifications.Visible)
                        continue;
                    if (practisequiz != null && practisequiz.Visible)
                        continue;
                    if (prepare != null && prepare.Visible)
                        continue;
                    if (profile != null && profile.Visible)
                        continue;
                    if (registerui != null && registerui.Visible)
                        continue;
                    if (resultform != null && resultform.Visible)
                        continue;
                    ended++;
                }
                if(ended == 1000)
                    Application.Exit();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            GraphicsPath formPath = new GraphicsPath();
            formPath.AddArc(0, 0, 30, 30, 180, 90);
            formPath.AddArc(Width - 30, 0, 30, 30, 270, 90);
            formPath.AddArc(Width - 30, Height - 30, 30, 30, 0, 90);
            formPath.AddArc(0, Height - 30, 30, 30, 90, 90);
            formPath.CloseAllFigures();
            this.Region = new Region(formPath);

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            if (this.BackgroundImage != null)
            {
                e.Graphics.DrawImage(this.BackgroundImage, this.ClientRectangle);
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(80, 255, 245, 180),
                Color.FromArgb(80, 255, 230, 100),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (this.registerui == null || this.registerui.IsDisposed)
            {
                if (this.registerui != null)
                    try { this.registerui.Close(); } catch { }
                this.registerui = new RegisterUI(this);
            }
            this.registerui.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (this.loginform == null || this.loginform.IsDisposed)
            {
                if (this.loginform != null)
                    try { this.loginform.Close(); } catch { }
                this.loginform = new LoginForm(this);
            }
            this.loginform.Show();
        }
    }

    public class RoundedButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, 20, 20, 180, 90);
            path.AddArc(rect.Right - 20, rect.Y, 20, 20, 270, 90);
            path.AddArc(rect.Right - 20, rect.Bottom - 20, 20, 20, 0, 90);
            path.AddArc(rect.X, rect.Bottom - 20, 20, 20, 90, 90);
            path.CloseAllFigures();

            this.Region = new Region(path);

            // Vẽ gradient
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(255, 190, 90), Color.FromArgb(255, 140, 30), LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }

            TextRenderer.DrawText(g, this.Text, this.Font, rect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }



}
