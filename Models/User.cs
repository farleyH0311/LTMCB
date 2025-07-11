namespace WordUp.Models
{
    public class User
    {
        public string Uid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
        public string Email { get; set; }
        public string OTP { get; set; }
        public string AvatarPath { get; set; }
        public string Bio { get; set; }
        public int Score { get; set; } = 0;
        public int Posts { get; set; } = 0;
        public int Lessons { get; set; } = 0;

    }
}