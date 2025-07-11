using Google.Cloud.Firestore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WordUp.Models;

namespace FirestoreUploader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Đường dẫn tới file JSON credentials
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string credentialsPath = Path.Combine(basePath, @"Resources\ltm-wu-firebase-adminsdk-fbsvc-91be4e23e4.json");
            string jsonPath = Path.Combine(basePath, @"Resources\Lessons.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

            FirestoreDb db = FirestoreDb.Create("ltm-wu");

            if (!File.Exists(jsonPath))
            {
                MessageBox.Show("File không tồn tại: " + jsonPath);
            }
            string json = File.ReadAllText(jsonPath);

            var data = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);

            foreach (var entry in data)
            {
                string docPath = entry.Key;
                JObject lessonData = entry.Value;

                string[] pathParts = docPath.Split('/');
                string collectionName = pathParts[0];
                string documentId = pathParts[1];

                DocumentReference docRef = db.Collection(collectionName).Document(documentId);
                Dictionary<string, object> docData = lessonData.ToObject<Dictionary<string, object>>();

                await docRef.SetAsync(docData);
                Console.WriteLine($"Uploaded: {documentId} to {collectionName}");
            }

            Console.WriteLine("Done.");
        }
    }
}
