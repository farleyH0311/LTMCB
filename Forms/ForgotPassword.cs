using System;
using System.Windows.Forms;
using WordUp.Services;
using WordUp.Models;

namespace WordUp.Forms
{
    public partial class ForgotPassword : BaseForm
    {
        private FirebaseService firebaseService = new FirebaseService();
        private EmailService emailService = new EmailService();
        private string currentUsername;
        private string currentEmail;
        private Form1 mainform;
        public ForgotPassword(Form1 MainForm)
        {
            InitializeComponent();
            panel_rspw.Visible = false;
            rspw.PasswordChar = '*';
            cfrspw.PasswordChar = '*';
            this.mainform = MainForm;
        }
        private void showpw_MouseDoubleClick(object sender, EventArgs e)
        {
            if (rspw.PasswordChar == '*' || cfrspw.PasswordChar == '*')
            {
                rspw.PasswordChar = '\0';
                cfrspw.PasswordChar = '\0';

            }
            else
            {
                rspw.PasswordChar = '*';
                cfrspw.PasswordChar = '*';
            }
        }
        private async void btnSendOTP_Click(object sender, EventArgs e)
        {
            string Username = textBox2.Text.Trim();
            string Email = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Email))
            {
                MessageBox.Show("Vui lòng nhập Username và Email.");
                return;
            }


            var user = await firebaseService.GetUserByUsernameAndEmailAsync(Username, Email);
            if (user != null)
            {
                currentUsername = Username;
                currentEmail = Email;

                emailService.SendOtp(Email);

            }
            else
            {
                MessageBox.Show("Username và Email không khớp.");
            }
        }

        private void btnVerifyOtp_Click(object sender, EventArgs e)
        {
            string inputOtp = guna2otp.Text.Trim();
            if (string.IsNullOrEmpty(inputOtp))
            {
                MessageBox.Show("Vui lòng nhập mã OTP.");
                return;
            }

            if (emailService.VerifyOtp(inputOtp))
            {
                MessageBox.Show("Xác minh thành công. Vui lòng đặt lại mật khẩu mới.");
                panel_rspw.Visible = true;
            }
            else
            {
                MessageBox.Show("Mã OTP không đúng.");
            }
        }

        private async void btnResetPassword_Click(object sender, EventArgs e)
        {
            string newPassword = rspw.Text.Trim();
            string confirmPassword = cfrspw.Text.Trim();

            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới.");
                return;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu để xác nhận.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp. Vui lòng thử lại.");
                return;
            }

            string hashedPassword = PasswordHelper.HashPassword(newPassword);

            bool success = await firebaseService.UpdatePassword(currentUsername, hashedPassword);
            if (success)
            {
                MessageBox.Show("Đặt lại mật khẩu thành công.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi đặt lại mật khẩu.");
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel_rspw_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ForgotPassword_Load(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            if (mainform.loginform == null || mainform.loginform.IsDisposed)
            {
                if (mainform.loginform != null)
                    try { mainform.loginform.Close(); } catch { }
                mainform.loginform = new LoginForm(mainform);
            }
            mainform.loginform.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.registerui == null || mainform.registerui.IsDisposed)
            {
                if (mainform.registerui != null)
                    try { mainform.registerui.Close(); } catch { }
                mainform.registerui = new RegisterUI(mainform);
            }
            mainform.registerui.Show();
            this.Close();

        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (mainform.hdsd == null || mainform.hdsd.IsDisposed)
            {
                if (mainform.hdsd != null)
                    try { mainform.hdsd.Close(); } catch { }
                mainform.hdsd = new HDSD(mainform);
            }
            mainform.hdsd.ShowDialog();
        }
        private void cfrspw_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
    }
}
