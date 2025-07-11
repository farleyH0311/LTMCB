using Newtonsoft.Json;

namespace WordUp.Models
{
    public class Lesson
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        [JsonProperty("questionText")]
        public string QuestionText { get; set; }

        [JsonProperty("options")]
        public List<string> Options { get; set; }

        [JsonProperty("correctAnswer")]
        public string CorrectAnswer { get; set; }
    }
}
