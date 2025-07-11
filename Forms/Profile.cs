using Guna.UI2.WinForms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using WordUp.Models;
using WordUp.Services;
using static Google.Api.ResourceDescriptor.Types;

namespace WordUp.Forms
{
    public partial class Profile : BaseForm
    {
        private User currentUser;
        private User viewedUser;
        private FirebaseService firebaseService;
        private readonly Form backForm;
        private CloudinaryService cloudinaryService;
        private Form1 mainform;
        public Profile(Form1 MainForm, User loggedInUser, Form backForm, string callerName, User viewedUser = null)
        {
            InitializeComponent();
            this.mainform = MainForm;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            MakePictureBoxCircle(pictureBox2);
            this.currentUser = loggedInUser;
            this.backForm = backForm;
            firebaseService = new FirebaseService();
            cloudinaryService = new CloudinaryService();

            ApplyRoundedStyleToAllButtons(this);
            panel2.Visible = false;
            panel1.Visible = false;
            guna2TextBox2.ReadOnly = true;
            guna2TextBox3.ReadOnly = true;
            guna2TextBox4.ReadOnly = true;

            textBox_pw.PasswordChar = '*';
            guna2TextBox1.PasswordChar = '*';
            textBox_npw.PasswordChar = '*';
            if (callerName == "author")
            {
                button1.Visible = false;
                this.viewedUser = viewedUser;
                LoadUserData(viewedUser);

            }
            if (callerName == "me")
            {
                btn_return.Visible = false;
                LoadUserData(currentUser);
            }
        }
        private void LoadUserData(User currentUser)
        {
            guna2TextBox2.Text = currentUser.Bio;
            label_username.Text = currentUser.Username;
            textBox_bio.Text = currentUser.Bio;
            guna2TextBox3.Text = currentUser.Score.ToString();
            guna2TextBox4.Text = currentUser.Lessons.ToString();
            guna2TextBox2.Font = new Font("Segoe UI", 10.875F, FontStyle.Bold);
            guna2TextBox3.Font = new Font("Trebuchet MS", 20.875F, FontStyle.Bold);
            guna2TextBox4.Font = new Font("Trebuchet MS", 20.875F, FontStyle.Bold);


            if (!string.IsNullOrEmpty(currentUser.AvatarPath))
            {
                try
                {
                    pictureBox1.ImageLocation = currentUser.AvatarPath;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    MakePictureBoxCircle(pictureBox1);

                    pictureBox2.ImageLocation = currentUser.AvatarPath;
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    MakePictureBoxCircle(pictureBox2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message);
                }
            }
        }

        private void showpw_Click(object sender, EventArgs e)
        {
            if (textBox_pw.PasswordChar == '*' || textBox_npw.PasswordChar == '*' || guna2TextBox1.PasswordChar == '*')
            {
                textBox_pw.PasswordChar = '\0';
                textBox_npw.PasswordChar = '\0';
                guna2TextBox1.PasswordChar = '\0';

            }
            else
            {
                textBox_pw.PasswordChar = '*';
                textBox_npw.PasswordChar = '*';
                guna2TextBox1.PasswordChar = '*';
            }
        }
        private void MakePictureBoxCircle(PictureBox pic)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, pic.Width - 1, pic.Height - 1);
            pic.Region = new Region(gp);
        }
        private void btnUploadAvatar_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = Image.FromFile(ofd.FileName);
                pictureBox2.Tag = ofd.FileName;
            }
        }

        private async void btnConfirmAvatar_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Tag != null)
            {
                string localImagePath = pictureBox2.Tag.ToString();
                // MessageBox.Show("Uploading image from path: " + localImagePath);  

                try
                {
                    var cloudinaryService = new CloudinaryService();

                    string cloudinaryImageUrl = await cloudinaryService.UploadImageToCloudinary(localImagePath);

                    if (!string.IsNullOrEmpty(cloudinaryImageUrl))
                    {
                        currentUser.AvatarPath = cloudinaryImageUrl;

                        bool updated = await firebaseService.UpdateAvatarAsync(currentUser.Username, cloudinaryImageUrl);
                        if (updated)
                        {
                            MessageBox.Show("Avatar đã được cập nhật!");
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật avatar thất bại!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Upload ảnh lên Cloudinary thất bại!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Không có hình ảnh để upload.");
            }
        }

        private async void btnChangeBio_Click(object sender, EventArgs e)
        {
            string newBio = textBox_bio.Text.Trim();
            currentUser.Bio = newBio;

            bool success = await firebaseService.UpdateBioAsync(currentUser.Username, newBio);
            if (success)
                MessageBox.Show("Bio updated!");
            else
                MessageBox.Show("Failed to update bio.");
        }
        private async void btnVerifyPassword_Click(object sender, EventArgs e)
        {
            string enteredPassword = textBox_pw.Text.Trim();
            string hashedPassword = PasswordHelper.HashPassword(enteredPassword);
            if (hashedPassword == currentUser.Password)
            {
                MessageBox.Show("Xác thực thành công. Mời bạn nhập mật khẩu mới.");
                panel2.Visible = true;
                panel2.BringToFront();
                guna2TextBox1.PasswordChar = '*';
                textBox_npw.PasswordChar = '*';
            }
            else
            {
                MessageBox.Show("Mật khẩu hiện tại không đúng!");
            }
        }

        private async void btnChangePassword_Click(object sender, EventArgs e)
        {
            string newPassword = textBox_npw.Text.Trim();
            string confirmPassword = guna2TextBox1.Text.Trim();

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Mật khẩu mới không được để trống!");
                return;
            }

            string hashedNewPassword = PasswordHelper.HashPassword(newPassword);

            currentUser.Password = hashedNewPassword;

            bool success = await firebaseService.UpdatePassword(currentUser.Username, hashedNewPassword);

            if (success)
            {
                panel2.Visible = false;
                MessageBox.Show("Đổi mật khẩu thành công!");
            }
            else
            {
                MessageBox.Show("Đổi mật khẩu thất bại. Vui lòng thử lại.");
            }
        }
        private async void btnExit_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == true)
            {
                panel2.Visible = false;
            }
            else
            {
                panel1.Visible = false;
            }
            LoadUserData(currentUser);
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
            backForm.Show();
        }
        private async void btnEditProfile_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label_username_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
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
                    btn.BackColor = Color.Gold;
                    btn.ForeColor = Color.MidnightBlue;
                    btn.TextAlign = ContentAlignment.MiddleCenter;
                    btn.Font = new Font("Segoe UI", 10.875F, FontStyle.Bold);
                    btn.Size = new Size(120, 40);

                    btn.Paint += (s, e) =>
                    {
                        GraphicsPath path = new GraphicsPath();
                        int radius = 30;
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

        private void Profile_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

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

        private void btnFlashcard_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.flashcardform != null)
                try { mainform.flashcardform.Close(); } catch { }
            mainform.flashcardform = new FlashcardForm(mainform, currentUser, currentUser.Uid);
            mainform.flashcardform.Show();
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

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == true)
            {
                panel2.Visible = false;
            }
            else
            {
                panel1.Visible = false;
            }
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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

        private void textBox_bio_TextChanged(object sender, EventArgs e)
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
        private void btnProfile_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "me";
            if (mainform.profile != null)
                try { mainform.profile.Close(); } catch { }
            mainform.profile = new Profile(mainform, currentUser, this, callerName);
            mainform.profile.Show();
            
        }
        private void message_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.chat != null)
                try { mainform.chat.Close(); } catch { }
            mainform.chat = new Chat(mainform, currentUser, viewedUser);
            mainform.chat.Show();
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
      }
    }
