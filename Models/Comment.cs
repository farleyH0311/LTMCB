using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordUp.Models
{
    public class Comment
    {
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public Comment() { }

        public Comment(string author, string text)
        {
            Author = author;
            Text = text;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"@{Author}: {Text} ({Timestamp.ToLocalTime():g})";
        }
    }
}