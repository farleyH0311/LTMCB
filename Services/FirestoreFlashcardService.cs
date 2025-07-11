using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WordUp.Models;

namespace WordUp.Services
{
    public class FirestoreFlashcardService
    {
        private readonly FirestoreDb _db;


        public FirestoreFlashcardService()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var credentialPath = Path.Combine(basePath, "Resources", "ltm-wu-firebase-adminsdk-fbsvc-91be4e23e4.json");

            Environment.SetEnvironmentVariable(
                "GOOGLE_APPLICATION_CREDENTIALS",
                credentialPath
            );
            _db = FirestoreDb.Create("ltm-wu");
        }


        public async Task<List<string>> GetAllLessonsAsync()
        {
            QuerySnapshot snapshot = await _db.Collection("flashcard").GetSnapshotAsync();
            List<string> lessonNames = new List<string>();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                lessonNames.Add(doc.Id);
            }

            return lessonNames;
        }

        public async Task<List<Flashcard>> GetFlashcardsByLessonAsync(string lessonId)
        {
            DocumentReference docRef = _db.Collection("flashcard").Document(lessonId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return new List<Flashcard>();

            Dictionary<string, object> docData = snapshot.ToDictionary();

            if (!docData.TryGetValue("cards", out object cardsObj) || cardsObj is not List<object> cardsData)
                return new List<Flashcard>();

            List<Flashcard> flashcards = new List<Flashcard>();

            foreach (var cardObj in cardsData)
            {
                if (cardObj is not Dictionary<string, object> cardDict) continue;
                if (!cardDict.TryGetValue("front", out var frontObj) || frontObj is not Dictionary<string, object> frontDict)
                    continue;
                if (!cardDict.TryGetValue("back", out var backObj) || backObj is not Dictionary<string, object> backDict)
                    continue;

                Flashcard vocab = new Flashcard
                {
                    front = new Front
                    {
                        Word = frontDict["Word"]?.ToString(),
                        Phonetic = frontDict["Phonetic"]?.ToString(),
                        Definition = frontDict["Definition"]?.ToString()
                    },
                    back = new Back
                    {
                        Meaning = backDict["Meaning"]?.ToString()
                    }
                };

                flashcards.Add(vocab);
            }

            return flashcards;
        }

        public async Task AddFlashcardToLessonAsync(string lessonId, Flashcard newCard)
        {
            DocumentReference docRef = _db.Collection("flashcard").Document(lessonId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            List<Dictionary<string, object>> cardsList = new();

            if (snapshot.Exists)
            {
                var data = snapshot.ToDictionary();
                if (data.TryGetValue("cards", out object cardsObj) && cardsObj is List<object> existingCards)
                {
                    foreach (var obj in existingCards)
                    {
                        if (obj is Dictionary<string, object> d)
                            cardsList.Add(d);
                    }
                }
            }

            var newCardDict = new Dictionary<string, object>
            {
                ["front"] = new Dictionary<string, object>
                {
                    ["Word"] = newCard.front.Word,
                    ["Phonetic"] = newCard.front.Phonetic,
                    ["Definition"] = newCard.front.Definition
                },
                ["back"] = new Dictionary<string, object>
                {
                    ["Meaning"] = newCard.back.Meaning
                }
            };

            cardsList.Add(newCardDict);

            await docRef.SetAsync(new Dictionary<string, object>
            {
                { "cards", cardsList }
            });
        }

        public async Task UpdateLessonFlashcardsAsync(string lessonId, List<Flashcard> flashcards)
        {
            DocumentReference docRef = _db.Collection("flashcard").Document(lessonId);

            var cardList = new List<Dictionary<string, object>>();

            foreach (var card in flashcards)
            {
                var cardDict = new Dictionary<string, object>
                {
                    ["front"] = new Dictionary<string, object>
                    {
                        ["Word"] = card.front.Word,
                        ["Phonetic"] = card.front.Phonetic,
                        ["Definition"] = card.front.Definition
                    },
                    ["back"] = new Dictionary<string, object>
                    {
                        ["Meaning"] = card.back.Meaning
                    }
                };

                cardList.Add(cardDict);
            }

            await docRef.SetAsync(new Dictionary<string, object>
            {
                { "cards", cardList }
            });
        }

        public async Task<bool> RemoveFlashcardFromLessonAsync(string lessonId, int index)
        {
            DocumentReference docRef = _db.Collection("flashcard").Document(lessonId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return false;

            Dictionary<string, object> data = snapshot.ToDictionary();

            if (!data.TryGetValue("cards", out object cardsObj) || cardsObj is not List<object> cardsList)
                return false;

            var updatedCards = new List<Dictionary<string, object>>();

            for (int i = 0; i < cardsList.Count; i++)
            {
                if (i == index) continue;

                if (cardsList[i] is Dictionary<string, object> cardDict)
                    updatedCards.Add(cardDict);
            }

            await docRef.SetAsync(new Dictionary<string, object>
            {
                { "cards", updatedCards }
            });

            return true;
        }
    }
}
