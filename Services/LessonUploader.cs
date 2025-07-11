using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WordUp.Services
{
    public class LessonUploadModel
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
    }

    public static class LessonUploader
    {
        public static async Task UploadLessonsAsync(string jsonPath, string credentialsPath, string projectId)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
            FirestoreDb db = FirestoreDb.Create(projectId);

            string json = await File.ReadAllTextAsync(jsonPath);
            var lessons = JsonConvert.DeserializeObject<Dictionary<string, LessonUploadModel>>(json);

            foreach (var entry in lessons)
            {
                string docId = entry.Key;
                LessonUploadModel lesson = entry.Value;

                var lessonDict = new Dictionary<string, object>
                {
                    { "title", lesson.Title ?? string.Empty },
                    { "questions", (lesson.Questions ?? new List<Question>())
                        .Select(q => new Dictionary<string, object>
                        {
                            { "questionText", q.QuestionText ?? string.Empty },
                            { "options", q.Options ?? new List<string>() },
                            { "correctAnswer", q.CorrectAnswer ?? string.Empty }
                        }).ToList()
                    }
                };


                DocumentReference docRef = db.Collection("Lessons").Document(docId);
                await docRef.SetAsync(lessonDict);
            }
        }
    }
}
