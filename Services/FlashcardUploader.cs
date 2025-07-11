using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using WordUp.Models;

namespace WordUp.Services
{
    

    public static class FlashcardUploader
    {
        public static async Task UploadFlashcardsAsync(string jsonPath, string credentialsPath, string projectId)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
            FirestoreDb db = FirestoreDb.Create(projectId);

            string json = await File.ReadAllTextAsync(jsonPath);
            var flashcards = JsonConvert.DeserializeObject<Dictionary<string, List<Flashcard>>>(json);

            foreach (var entry in flashcards)
            {
                string docId = entry.Key;
                var cardList = entry.Value;

                // Mỗi document là 1 chủ đề (vd: "Contracts") chứa danh sách flashcard
                var docData = new Dictionary<string, object>
            {
                { "cards", cardList.Select(card => new Dictionary<string, object>
                    {
                        { "front", new Dictionary<string, object>
                            {
                                { "Word", card.front.Word },
                                { "Phonetic", card.front.Phonetic },
                                { "Definition", card.front.Definition }
                            }
                        },
                        { "back", new Dictionary<string, object>
                            {
                                { "Meaning", card.back.Meaning }
                            }
                        }
                    }).ToList()
                }
            };

                DocumentReference docRef = db.Collection("flashcard").Document(docId); // 🔄 Tên collection
                await docRef.SetAsync(docData);
            }
        }
    }

}
