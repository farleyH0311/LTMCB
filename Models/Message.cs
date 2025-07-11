using System;

namespace WordUp.Models
{
    public class Message
    {
        public string SenderId { get; set; }
        public string Text { get; set; }
        public long Timestamp { get; set; } 
    }
}