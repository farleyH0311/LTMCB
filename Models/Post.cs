using System;
using System.Collections.Generic;

namespace WordUp.Models
{
    public class Post
    {
        public string Id { get; set; }                
        public string Author { get; set; }             
        public string Content { get; set; }            
        public int Likes { get; set; } = 0;            
        public Dictionary<string, Comment> Comments { get; set; } = new(); 
        public HashSet<string> LikedByUsers { get; set; } = new HashSet<string>(); 
        public DateTime Timestamp { get; set; }        

        public Post() { }

        public Post(string author, string content)
        {
            Author = author;
            Content = content;
            Likes = 0;
            Comments = new Dictionary<string, Comment>();
            Timestamp = DateTime.UtcNow; 
        }


       
        public override string ToString()
        {
            string contentPreview = Content.Length > 100 ? Content.Substring(0, 100) + "..." : Content;

            return $"👤 {Author}: 📝 {contentPreview} | ❤️ {Likes} likes";
        }



    }

}
