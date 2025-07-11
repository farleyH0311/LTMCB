// LeaderboardService.cs
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordUp.Services
{
    public class UserProfile
    {
        public string Username { get; set; }
        public int Score { get; set; }
    }

    public class LeaderboardService
    {
        private readonly FirebaseClient firebaseClient;

        public LeaderboardService()
        {
            firebaseClient = new FirebaseClient("https://ltm-wu-default-rtdb.firebaseio.com/");
        }

        public async Task<List<UserProfile>> GetLeaderboardAsync()
        {
            var users = await firebaseClient
                .Child("Users")
                .OnceAsync<UserProfile>();

            return users
                .Select(u => u.Object)
                .Where(u => u.Username != null)
                .ToList();
        }
    }
}
