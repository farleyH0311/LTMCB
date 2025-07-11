using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordUp.Models;

namespace WordUp.Services
{
    public class FirebaseService
    {
        private IFirebaseClient client;

        public FirebaseService()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                BasePath = "https://ltm-wu-default-rtdb.firebaseio.com/",
            };

            client = new FireSharp.FirebaseClient(config);
        }
        public async Task<bool> UpdateBioAsync(string username, string newBio)
        {
            var update = new { Bio = newBio };
            FirebaseResponse response = await client.UpdateAsync("Users/" + username, update);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        public async Task<bool> UpdateScoreAsync(string username, int newscore)
        {
            
            var update = new { Score = newscore };
            FirebaseResponse response = await client.UpdateAsync("Users/" + username, update);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        
        }
        public async Task<bool> UpdateLessonAsync(string username, int newslesson)
        {

            var update = new { Lessons = newslesson };
            FirebaseResponse response = await client.UpdateAsync("Users/" + username, update);
            return response.StatusCode == System.Net.HttpStatusCode.OK;

        }

        public async Task<bool> UpdateAvatarAsync(string username, string avatarPath)
        {
            var update = new { AvatarPath = avatarPath };
            FirebaseResponse response = await client.UpdateAsync("Users/" + username, update);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> RegisterUser(User user)
        {
            FirebaseResponse response = await client.GetAsync("Users/" + user.Username);
            User existingUser = response.ResultAs<User>();

            if (existingUser != null)
                return false;

            bool emailExists = await CheckEmailExists(user.Email);
            if (emailExists)
                return false;
            user.Uid = user.Username;

            SetResponse set = await client.SetAsync("Users/" + user.Username, user);
            return set.StatusCode == System.Net.HttpStatusCode.OK;
        }



        public async Task<bool> CheckEmailExists(string email)
        {
            FirebaseResponse response = await client.GetAsync("Users/");
            var users = response.ResultAs<Dictionary<string, User>>();

            if (users == null) return false;

            foreach (var user in users.Values)
            {
                if (user.Email == email)
                    return true;
            }

            return false;
        }
        public async Task<User> LoginAsync(string email, string username, string password)
        {
            FirebaseResponse response = await client.GetAsync("Users/");
            var users = response.ResultAs<Dictionary<string, User>>();

            if (users == null) return null;

            foreach (var user in users.Values)
            {
                if (user.Email == email && user.Username == username && user.Password == password)
                {
                    // Gán UID là username
                    user.Uid = user.Username;

                    return user;
                }
            }

            return null;
        }
        public async Task<User> GetUserByUsernameAndEmailAsync(string username, string email)
        {
            FirebaseResponse response = await client.GetAsync("Users/" + username);

            if (response.Body == "null" || string.IsNullOrEmpty(response.Body))
            {
                MessageBox.Show($"Không tìm thấy người dùng với username: {username}");
                return null;
            }

            var user = response.ResultAs<User>();

            if (user.Email.Trim().ToLower() == email.Trim().ToLower())
            {

                return user;
            }

            return null;
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            FirebaseResponse response = await client.GetAsync("Users/" + username);

            if (response.Body == "null" || string.IsNullOrEmpty(response.Body))
            {
                MessageBox.Show($"Không tìm thấy người dùng với username: {username}");
                return null;
            }

            return response.ResultAs<User>();
        }


        public async Task<bool> UpdatePassword(string username, string newPassword)
        {
            FirebaseResponse response = await client.GetAsync("Users/" + username);
            if (response.Body == "null") return false;

            User user = response.ResultAs<User>();
            user.Password = newPassword;

            SetResponse setResponse = await client.SetAsync("Users/" + username, user);
            return setResponse.StatusCode == System.Net.HttpStatusCode.OK;
        }
        public async Task<List<Post>> GetPostsAsync()
        {
            FirebaseResponse response = await client.GetAsync("posts");
            var data = response.ResultAs<Dictionary<string, Post>>();

            List<Post> posts = new();
            if (data != null)
            {
                foreach (var item in data)
                {
                    item.Value.Id = item.Key;
                    posts.Add(item.Value);
                }
            }

            return posts;
        }

        public async Task AddPostAsync(Post post)
        {
            PushResponse response = await client.PushAsync("posts", post);
            post.Id = response.Result.name;
        }


        public async Task SendNotificationAsync(string toUser, Notification notification)
        {
            string notificationId = Guid.NewGuid().ToString();
            await client.SetAsync($"notifications/{toUser}/{notificationId}", notification);
        }
        public List<(string NotificationId, Notification Noti)> GetNotificationsWithId(string username)
        {
            var response = client.GetAsync($"notifications/{username}").Result;
            var data = response.ResultAs<Dictionary<string, Notification>>();

            var result = new List<(string, Notification)>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    result.Add((item.Key, item.Value));
                }
            }
            return result;
        }

        public void SendNotification(string toUser, Notification notification)
        {
            string notificationId = Guid.NewGuid().ToString();
            notification.Id = notificationId;
            client.Set($"notifications/{toUser}/{notificationId}", notification);
        }
        public async Task UpdateNotificationReadStatus(string notificationId, string username)
        {
            var response = await client.GetAsync($"notifications/{username}/{notificationId}");

            if (response.Body == "null") return;

            var notification = response.ResultAs<Notification>();

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await client.UpdateAsync($"notifications/{username}/{notificationId}", notification);
            }
        }

        public async Task<Post> GetPostByIdAsync(string postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync($"posts/{postId}");
                if (response.Body != "null")
                {
                    Post post = response.ResultAs<Post>();
                    return post;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy bài viết: " + ex.Message);
                return null;
            }
        }

        public async Task LikePostAsync(string postId, int updatedLikes, HashSet<string> likedByUsers)
        {
            await client.UpdateAsync($"posts/{postId}", new
            {
                Likes = updatedLikes,
                LikedByUsers = likedByUsers.ToList()
            });
        }

        public async Task AddCommentAsync(string postId, Comment comment)
        {
            await client.PushAsync($"posts/{postId}/Comments", comment);
        }
        public async Task<List<Comment>> GetCommentsAsync(string postId)
        {
            FirebaseResponse response = await client.GetAsync($"posts/{postId}/Comments");
            var data = response.ResultAs<Dictionary<string, Comment>>();

            List<Comment> comments = new();
            if (data != null)
            {
                foreach (var item in data)
                {
                    comments.Add(item.Value);
                }
            }

            return comments;
        }
        public async Task<bool> UpdatePostAsync(string postId, Post updatedPost)
        {
            FirebaseResponse response = await client.GetAsync("posts/" + postId);
            if (response.Body == "null" || string.IsNullOrEmpty(response.Body))
            {
                return false;
            }

            SetResponse setResponse = await client.SetAsync("posts/" + postId, updatedPost);

            return setResponse.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> AddBonusScoreAsync(string username, int bonus)
        {
            FirebaseResponse response = await client.GetAsync("Users/" + username);
            if (response.Body == "null" || string.IsNullOrEmpty(response.Body))
            {
                return false;
            }

            var user = response.ResultAs<User>();
            if (user == null)
                return false;

            user.Score += bonus;

            SetResponse set = await client.SetAsync("Users/" + username, user);
            return set.StatusCode == System.Net.HttpStatusCode.OK;
        }


    }

}

