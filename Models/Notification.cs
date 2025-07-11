using System;

namespace WordUp.Models
{
    public class Notification
    {
        public string Id { get; set; }           
        public string ReceiverId { get; set; }    
        public string SenderId { get; set; }     
        public string PostId { get; set; }         
        public string Type { get; set; }       
        public bool IsRead { get; set; }          
        public DateTime Timestamp { get; set; }    

        public Notification() { }

        public Notification(string receiverId, string senderId, string postId, string type)
        {
            ReceiverId = receiverId;
            SenderId = senderId;
            PostId = postId;
            Type = type;
            IsRead = false;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{SenderId} -> {ReceiverId}:  ({Timestamp.ToLocalTime():g})";
        }
    }
}
