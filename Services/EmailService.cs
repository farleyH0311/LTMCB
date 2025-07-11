using System;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace WordUp.Services
{
    internal class EmailService
    {
        private readonly string senderEmail = "lttlefrog31@gmail.com";
        private readonly string appPassword = "pukc mwlm afwt pfoq";                    

        public string LastSentOtp { get; private set; }

        public bool SendOtp(string recipientEmail)
        {
            try
            {
                Random rand = new Random();
                LastSentOtp = rand.Next(100000, 999999).ToString();

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, appPassword),
                    EnableSsl = true,   
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Mã OTP xác nhận đăng ký",
                    Body = $"Mã OTP của bạn là: {LastSentOtp}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(recipientEmail);

                smtpClient.Send(mailMessage);

                MessageBox.Show("Mã OTP đã được gửi tới email của bạn.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi gửi OTP: {ex.Message}");
                return false;
            }
        }

        public bool VerifyOtp(string inputOtp)
        {
            return inputOtp == LastSentOtp;
        }
    }
}
