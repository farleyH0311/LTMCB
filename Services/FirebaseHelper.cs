using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Threading.Tasks;
using WordUp.Models;

public class FirebaseHelper
{
    private FirebaseClient firebaseClient;

    public FirebaseHelper(string firebaseUrl)
    {
        firebaseClient = new FirebaseClient(firebaseUrl);
    }

    public FirebaseClient Client => firebaseClient;
    public async Task<List<(string otherUser, long lastTimestamp)>> GetRecentChats(string currentUserId)
    {
        var result = new List<(string otherUser, long lastTimestamp)>();

        var rooms = await Client.Child("chatRooms").OnceAsync<object>();

        foreach (var room in rooms)
        {
            var roomId = room.Key;

            // roomId luôn là {user1}_{user2}
            var parts = roomId.Split('_');
            if (parts.Length != 2) continue;

            if (parts[0] != currentUserId && parts[1] != currentUserId) continue;

            string otherUser = (parts[0] == currentUserId) ? parts[1] : parts[0];

            // lấy tin nhắn cuối cùng
            var lastMessage = await Client
                .Child("chatRooms")
                .Child(roomId)
                .Child("messages")
                .OrderBy("Timestamp")
                .LimitToLast(1)
                .OnceAsync<WordUp.Models.Message>();

            long timestamp = 0;
            if (lastMessage.Any())
            {
                timestamp = lastMessage.First().Object.Timestamp;
            }

            result.Add((otherUser, timestamp));
        }

        // sắp xếp giảm dần theo thời gian
        result.Sort((a, b) => b.lastTimestamp.CompareTo(a.lastTimestamp));
        return result;
    }
}