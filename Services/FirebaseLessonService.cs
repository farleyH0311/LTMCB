using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordUp.Models;

namespace WordUp.Services
{
    public class FirebaseLessonService
    {
        private readonly FirestoreDb _db;

        public FirebaseLessonService(string projectId, string credentialsPath)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
            _db = FirestoreDb.Create(projectId);
        }

        // Lấy danh sách ID của các document (tức các bài học)
        public async Task<List<string>> GetAllLessonIdsAsync()
        {
            var snapshot = await _db.Collection("Lessons").GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.Id).ToList();
        }

        // Lấy nội dung bài học cụ thể (bao gồm title và questions)
        public async Task<Lesson> GetLessonAsync(string lessonId)
        {
            DocumentReference docRef = _db.Collection("Lessons").Document(lessonId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            var data = snapshot.ToDictionary();
            var json = JsonConvert.SerializeObject(data);
            Lesson lesson = JsonConvert.DeserializeObject<Lesson>(json)!;
            return lesson;
        }
    }
}
