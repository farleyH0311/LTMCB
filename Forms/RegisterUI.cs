using Guna.UI2.WinForms;
using System;
using System.Windows.Forms;
using WordUp.Models;
using WordUp.Services;

namespace WordUp.Forms
{
    public partial class RegisterUI : BaseForm
    {
        private FirebaseService firebaseService;
        private EmailService emailService;
        private Form1 mainform;

        public RegisterUI(Form1 MainForm)
        {
            InitializeComponent();
            firebaseService = new FirebaseService();
            emailService = new EmailService();
            guna2fgp.PasswordChar = '*';
            guna2TextBox1.PasswordChar = '*';
            this.mainform = MainForm;
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string username = guna2Username.Text.Trim();
            string email = guna2Email.Text.Trim();
            string password = guna2fgp.Text.Trim();
            string confirmPassword = guna2TextBox1.Text.Trim();
            string Otp = guna2otp.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu không khớp.");
                return;
            }
            if (!emailService.VerifyOtp(Otp))
            {
                MessageBox.Show("OTP không khớp.");
                return;
            }
            string hashedPassword = PasswordHelper.HashPassword(password);

            bool isRegistered = await firebaseService.RegisterUser(new User { Username = username, Email = email, Password = hashedPassword });

            if (isRegistered)
            {
                MessageBox.Show("Chúc mừng!Bạn đã đăng ký thành công!");
                var user = await firebaseService.LoginAsync(email, username, hashedPassword);

                string callerName = "me";
                this.Hide();
                if (mainform.profile != null)
                    try { mainform.profile.Close(); } catch { }
                mainform.profile = new Profile(mainform, user, this, callerName);
                mainform.profile.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc email đã tồn tại.");
            }
        }

        private void btnOtp_Click(object sender, EventArgs e)
        {
            string email = guna2Email.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email!");
                return;
            }
            emailService.SendOtp(email);
        }

        private void showpw_MouseDoubleClick(object sender, EventArgs e)
        {
            if (guna2fgp.PasswordChar == '*' || guna2TextBox1.PasswordChar == '*')
            {
                guna2fgp.PasswordChar = '\0';
                guna2TextBox1.PasswordChar = '\0';
            }
            else
            {
                guna2fgp.PasswordChar = '*';
                guna2TextBox1.PasswordChar = '*';
            }
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.loginform == null || mainform.loginform.IsDisposed)
            {
                if (mainform.loginform != null)
                    try { mainform.loginform.Close(); } catch { }
                mainform.loginform = new LoginForm(mainform);
            }
            mainform.loginform.Show();
            this.Close();
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (mainform.hdsd != null)
                try { mainform.hdsd.Close(); } catch { }
            mainform.hdsd = new HDSD(mainform);
            mainform.hdsd.ShowDialog();
        }

    }
}
