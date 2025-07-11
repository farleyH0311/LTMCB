using System;
using System.Windows.Forms;
using WordUp.Models;
using WordUp.Services;
using System.Drawing.Drawing2D;

namespace WordUp.Forms
{


    public partial class LoginForm : BaseForm
    {
        public User currentUser;
        private Form1 mainform;

        public LoginForm(Form1 MainForm)
        {
            InitializeComponent();
            this.mainform = MainForm;
            GraphicsPath path = new GraphicsPath();
            int radius = 30; // Bo tròn góc 30px
            path.AddArc(0, 0, radius, radius, 180, 90); // Góc trên bên trái
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90); // Góc trên bên phải
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90); // Góc dưới bên phải
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90); // Góc dưới bên trái
            path.CloseAllFigures();
            this.Region = new Region(path);

            guna2fgp.PasswordChar = '*';
        }



        
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

        private void linkLabelfgp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            if (mainform.forgotpassword == null || mainform.forgotpassword.IsDisposed)
            {
                if (mainform.forgotpassword != null)
                    try { mainform.forgotpassword.Close(); } catch { }
                mainform.forgotpassword = new ForgotPassword(mainform);
            }
            mainform.forgotpassword.Show();
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void label6_Click_1(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void LoginForm_Load(object sender, EventArgs e) { }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2CustomRadioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private async void guna2Button1_Click1(object sender, EventArgs e)
        {
            string email = guna2Email.Text.Trim();
            string username = guna2Username.Text.Trim();
            string password = guna2fgp.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin đăng nhập.");
                return;
            }

            string hashedPassword = PasswordHelper.HashPassword(password);
            FirebaseService firebaseService = new FirebaseService();

            var user = await firebaseService.LoginAsync(email, username, hashedPassword);

            if (user != null)
            {
                currentUser = user;
                MessageBox.Show("Đăng nhập thành công!");


                if (user.Bio == null)
                {
                    string callerName = "me";
                    this.Hide();
                    if (mainform.profile != null)
                        try { mainform.profile.Close(); } catch { }
                    mainform.profile = new Profile(mainform, user, this, callerName);
                    mainform.profile.Show();
                    
                    
                }
                else
                {
                    this.Hide();
                    if (mainform.home != null)
                        try { mainform.home.Close(); } catch { }
                    mainform.home = new Home(mainform, user);
                    mainform.home.Show();
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show("Sai thông tin đăng nhập. Vui lòng kiểm tra lại.");
            }
        }

        private void guna2Username_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Email_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2fgp_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint_2(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
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

        private void showpw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            guna2fgp.PasswordChar = guna2fgp.PasswordChar == '*' ? '\0' : '*';
        }
    }
}
