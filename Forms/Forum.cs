using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordUp.Models;
using WordUp.Services;
using WordUp.Controllers;

namespace WordUp.Forms
{
    public partial class Forum : BaseForm
    {
        private readonly FirebaseService _firebaseService;
        private readonly User currentUser;
        private List<Post> allPosts = new List<Post>();
        private Post currentlyViewingPost;
        private readonly Form backForm;
        private Form1 mainform;

        public Forum(Form1 MainForm, User user, Form backForm, string callerName)
        {
            InitializeComponent();
            _firebaseService = new FirebaseService();
            currentUser = user;
            this.mainform = MainForm;
            this.backForm = backForm;
            ApplyRoundedStyleToAllButtons(this);

            panel1.Visible = false;
            flowPanelPosts.Visible = true;

            if (callerName != "no")
            {
                guna2CircleButton2.Visible = true;
                button_exit.Visible = false;
            }
            else
            {
                guna2CircleButton2.Visible = false;
                button_exit.Visible = true;
            }
            this.Load += Forum_Load;
        }

        private async void Forum_Load(object sender, EventArgs e)
        {
            await LoadPosts();
        }

        private async Task LoadPosts()
        {
            if (!string.IsNullOrEmpty(currentUser.AvatarPath))
            {
                try
                {
                    pictureBox1.ImageLocation = currentUser.AvatarPath;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    MakePictureBoxCircle(pictureBox1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message);
                }
            }

            try
            {
                allPosts = await _firebaseService.GetPostsAsync();
                allPosts.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));

                flowPanelPosts.Controls.Clear();
                foreach (var post in allPosts)
                {
                    var bubble = new PostBubble(post);
                    bubble.Click += (s, e) => OpenPostDetail(post.Id);
                    flowPanelPosts.Controls.Add(bubble);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải bài viết: " + ex.Message);
            }
        }

        private void ShowMyPosts()
        {
            flowPanelPosts.Controls.Clear();

            var myPosts = allPosts
                .Where(post => post.Author == currentUser.Username)
                .OrderByDescending(post => post.Timestamp)
                .ToList();

            foreach (var post in myPosts)
            {
                var bubble = new PostBubble(post);
                bubble.Click += (s, e) => OpenPostDetail(post.Id);
                flowPanelPosts.Controls.Add(bubble);
            }

            if (myPosts.Count == 0)
            {
                MessageBox.Show("Không có bài viết nào của bạn.");
            }
        }

        private void ShowMyLikes()
        {
            flowPanelPosts.Controls.Clear();

            var likedPosts = allPosts
                .Where(post => post.LikedByUsers.Contains(currentUser.Username))
                .OrderByDescending(post => post.Timestamp)
                .ToList();

            foreach (var post in likedPosts)
            {
                var bubble = new PostBubble(post);
                bubble.Click += (s, e) => OpenPostDetail(post.Id);
                flowPanelPosts.Controls.Add(bubble);
            }

            if (likedPosts.Count == 0)
            {
                MessageBox.Show("Bạn chưa thích bài viết nào.");
            }
        }

        private bool showingMyPosts = false;
        private bool showingMyLikes = false;

        private async void btnToggleMyPosts_Click(object sender, EventArgs e)
        {
            showingMyPosts = !showingMyPosts;

            if (showingMyPosts)
            {
                await LoadPosts();
                panel1.Visible = false;
                btn_mypost.Text = "⬅️ Quay lại";
                ShowMyPosts();
            }
            else
            {
                panel1.Visible = false;
                btn_mypost.Text = "📄 My Posts";
                await LoadPosts();
            }
        }

        private async void button_mylike_Click(object sender, EventArgs e)
        {
            showingMyLikes = !showingMyLikes;

            if (showingMyLikes)
            {
                await LoadPosts();
                panel1.Visible = false;
                button_mylike.Text = "⬅️ Quay lại";
                ShowMyLikes();
            }
            else
            {
                panel1.Visible = false;
                button_mylike.Text = "❤️ Liked Posts";
                await LoadPosts();
            }
        }

        private async void btnPost_Click(object sender, EventArgs e)
        {
            string content = up.Text.Trim();

            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("Vui lòng nhập nội dung bài viết.");
                return;
            }

            var newPost = new Post
            {
                Author = currentUser.Username,
                Content = content,
                Likes = 0,
                Comments = new Dictionary<string, Comment>(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await _firebaseService.AddPostAsync(newPost);
                MessageBox.Show("Bài viết đã được đăng thành công!");
                up.Clear();
                await LoadPosts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể đăng bài viết: " + ex.Message);
            }
        }

        public async void OpenPostDetail(string postId)
        {
            Post postDetail = allPosts.FirstOrDefault(post => post.Id == postId);

            if (postDetail == null)
            {
                postDetail = await _firebaseService.GetPostByIdAsync(postId);
            }

            if (postDetail == null)
            {
                MessageBox.Show("Không tìm thấy bài viết.");
                return;
            }

            panel1.Visible = true;
            currentlyViewingPost = postDetail;
            await ShowPostWithComments(postDetail);
        }
        private async void listBox2_MouseDoubleClick(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem == null) return;

            string selectedText = listBox2.SelectedItem.ToString();

            if (selectedText.StartsWith("📌"))
            {
                string authorUsername = selectedText.Substring(2).Trim();

                User authorUser = await _firebaseService.GetUserByUsernameAsync(authorUsername);
                string callerName = "author";
                if (authorUser != null)
                {
                    this.Hide();
                    if (mainform.profile != null)
                        try { mainform.profile.Close(); } catch { }
                    mainform.profile = new Profile(mainform, currentUser, this, callerName, authorUser);
                    mainform.profile.Show();
                }
            }
            if (selectedText.StartsWith("   ↳ @"))
            {
                int start = "   ↳ @".Length;
                int end = selectedText.IndexOf(":", start);
                if (end > start)
                {
                    string authorUsername = selectedText.Substring(start, end - start).Trim();

                    User authorUser = await _firebaseService.GetUserByUsernameAsync(authorUsername);
                    string callerName = "author";
                    if (authorUser != null)
                    {
                        this.Hide();
                        if (mainform.profile != null)
                            try { mainform.profile.Close(); } catch { }
                        mainform.profile = new Profile(mainform, authorUser, this, callerName);
                        mainform.profile.Show();
                        
                    }
                }
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
        private void btnForum_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "no";
            if (mainform.forum != null)
                try { mainform.forum.Close(); } catch { }
            mainform.forum = new Forum(mainform, currentUser, this, callerName);
            mainform.forum.Show();
            
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
        private void btnFlashcard_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.flashcardform == null || mainform.flashcardform.IsDisposed)
            {
                if (mainform.flashcardform != null)
                    try { mainform.flashcardform.Close(); } catch { }
                mainform.flashcardform = new FlashcardForm(mainform, currentUser, currentUser.Uid);
            }
            mainform.flashcardform.Show();
            this.Close();
        }

        private void Forum_Load_1(object sender, EventArgs e)
        {

        }
        private void ApplyRoundedStyleToAllButtons(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = Color.FromArgb(255, 250, 230);
                    btn.ForeColor = Color.MidnightBlue;
                    btn.TextAlign = ContentAlignment.MiddleCenter;
                    btn.Size = new Size(150, 40);

                    btn.Paint += (s, e) =>
                    {
                        GraphicsPath path = new GraphicsPath();
                        int radius = 20;
                        path.AddArc(0, 0, radius, radius, 180, 90);
                        path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
                        path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
                        path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
                        path.CloseFigure();
                        btn.Region = new Region(path);
                    };
                }

                if (ctrl.HasChildren)
                {
                    ApplyRoundedStyleToAllButtons(ctrl);
                }
            }
        }
        public async Task ShowPostWithComments(Post post)
        {
            listBox2.Items.Clear();

            listBox2.Items.Add("📌 " + post.Author);
            listBox2.Items.Add("🕒 " + post.Timestamp.ToString("dd/MM/yyyy HH:mm"));
            listBox2.Items.Add("📝 " + post.Content);
            listBox2.Items.Add("❤️ " + post.Likes + " lượt thích");

            if (post.LikedByUsers.Contains(currentUser.Username))
            {
                btn_like.Text = "💔";
            }
            else
            {
                btn_like.Text = "💖";
            }

            if (post.Comments.Count > 0)
            {
                listBox2.Items.Add("💬 Bình luận:");
                foreach (var comment in post.Comments.Values)
                {
                    listBox2.Items.Add("   ↳ " + comment.ToString());
                }
            }
            else
            {
                listBox2.Items.Add("   Chưa có bình luận nào.");
            }

            currentlyViewingPost = post;
        }
        private async Task SendNotification(string toUser, string type)
        {
            if (toUser == currentUser.Username) return;

            var notification = new Notification(
                toUser,
                currentUser.Username,
                currentlyViewingPost?.Id,
                type

            );

            await _firebaseService.SendNotificationAsync(toUser, notification);
        }

        private async void btnPostComment_Click(object sender, EventArgs e)
        {
            if (currentlyViewingPost == null)
            {
                MessageBox.Show("Không tìm thấy bài viết để bình luận.");
                return;
            }

            string commentText = up_cmt.Text.Trim();
            if (string.IsNullOrEmpty(commentText))
            {
                MessageBox.Show("Vui lòng nhập bình luận.");
                return;
            }

            var newComment = new Comment(currentUser.Username, commentText);
            currentlyViewingPost.Comments[Guid.NewGuid().ToString()] = newComment;

            try
            {
                await _firebaseService.AddCommentAsync(currentlyViewingPost.Id, newComment);
                up_cmt.Clear();
                await ShowPostWithComments(currentlyViewingPost);
                await SendNotification(currentlyViewingPost.Author, $"comment");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm bình luận: " + ex.Message);
            }
        }

        private void MakePictureBoxCircle(PictureBox pic)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, pic.Width - 1, pic.Height - 1);
            pic.Region = new Region(gp);
        }


        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

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
        private void btnBack_Click(object sender, EventArgs e)
        {
            backForm.Show();
            this.Close();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

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
        private async void btnLike_Click(object sender, EventArgs e)
        {
            if (currentlyViewingPost == null)
            {
                MessageBox.Show("Không tìm thấy bài viết để thích.");
                return;
            }

            bool alreadyLiked = currentlyViewingPost.LikedByUsers.Contains(currentUser.Username);

            try
            {
                if (alreadyLiked)
                {
                    currentlyViewingPost.Likes--;
                    currentlyViewingPost.LikedByUsers.Remove(currentUser.Username);
                }
                else
                {
                    currentlyViewingPost.Likes++;
                    currentlyViewingPost.LikedByUsers.Add(currentUser.Username);
                    await SendNotification(currentlyViewingPost.Author, $"like");
                }

                await _firebaseService.LikePostAsync(currentlyViewingPost.Id, currentlyViewingPost.Likes, currentlyViewingPost.LikedByUsers);
                await ShowPostWithComments(currentlyViewingPost);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật lượt thích: " + ex.Message);
            }
        }




    }
}
