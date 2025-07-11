using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordUp.Models; // Gồm Lesson, Question, RoomModel, AnswerModel, PlayerModel

namespace WordUp.Services
{
    public static class RoomService
    {
        // === A. ROOM MANAGEMENT ===

        public static async Task<string> FindOrCreateAvailableRoomAsync(FirestoreDb db, string uid, string name)
        {
            CollectionReference roomsRef = db.Collection("Rooms");
            QuerySnapshot snapshot = await roomsRef
                .WhereEqualTo("Started", false)
                .WhereEqualTo("Ended", false)
                .GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                var playersRef = doc.Reference.Collection("players");
                var playersSnap = await playersRef.GetSnapshotAsync();
                if (playersSnap.Count < 10)
                {
                    await AddPlayerToRoomAsync(db, doc.Id, uid, name);
                    return doc.Id;
                }
            }

            return await CreateRoomWithRandomLessonAsync(db, uid, name);
        }

        public static async Task<string> CreateRoomWithRandomLessonAsync(FirestoreDb db, string hostUid, string hostName)
        {
            var lessonsRef = db.Collection("Lessons");
            var lessonsSnapshot = await lessonsRef.GetSnapshotAsync();
            if (lessonsSnapshot.Count == 0)
                throw new Exception("Không có lesson nào trong hệ thống.");

            var random = new Random();
            var lessonDoc = lessonsSnapshot.Documents[random.Next(lessonsSnapshot.Count)];
            var lessonData = lessonDoc.ToDictionary();

            var title = lessonData.ContainsKey("title") ? lessonData["title"].ToString() : "Untitled";
            var questionsData = lessonData.ContainsKey("questions") ? lessonData["questions"] as List<object> : new List<object>();

            var allQuestions = questionsData.Select(q =>
            {
                var qDict = q as Dictionary<string, object>;
                if (qDict == null) return null;

                return new Question
                {
                    QuestionText = qDict["questionText"]?.ToString(),
                    Options = (qDict["options"] as IEnumerable<object>)?.Select(o => o.ToString()).ToList() ?? new List<string>(),
                    CorrectAnswer = qDict["correctAnswer"]?.ToString()
                };
            }).Where(q => q != null).ToList();

            var selectedQuestions = allQuestions.OrderBy(x => random.Next()).Take(10).ToList();

            string roomId;
            do
            {
                roomId = random.Next(1000, 10000).ToString();
            }
            while ((await db.Collection("Rooms").Document(roomId).GetSnapshotAsync()).Exists);

            var roomDict = new Dictionary<string, object>
            {
                { "hostUid", hostUid },
                { "title", title },
                { "questions", selectedQuestions.Select(q => new Dictionary<string, object>
                    {
                        { "questionText", q.QuestionText },
                        { "options", q.Options },
                        { "correctAnswer", q.CorrectAnswer }
                    }).ToList()
                },
                { "started", false },
                { "ended", false },
                { "currentQuestion", 0 },
                { "createdAt", Timestamp.GetCurrentTimestamp() }
            };

            var roomRef = db.Collection("Rooms").Document(roomId);
            await roomRef.SetAsync(roomDict);

            await AddPlayerToRoomAsync(db, roomId, hostUid, hostName);

            return roomId;
        }

        public static async Task<bool> JoinRoomAsync(FirestoreDb db, string roomId, string uid, string name)
        {
            var roomRef = db.Collection("Rooms").Document(roomId);
            var roomSnap = await roomRef.GetSnapshotAsync();

            if (!roomSnap.Exists)
                return false;

            var roomData = roomSnap.ToDictionary();
            bool started = roomData.ContainsKey("started") && Convert.ToBoolean(roomData["started"]);
            bool ended = roomData.ContainsKey("ended") && Convert.ToBoolean(roomData["ended"]);

            if (started || ended)
                return false;

            var playersRef = roomRef.Collection("players");
            var playersSnap = await playersRef.GetSnapshotAsync();

            if (playersSnap.Count >= 10)
                return false;

            if (playersSnap.Any(p => p.Id == uid))
                return true;

            await AddPlayerToRoomAsync(db, roomId, uid, name);
            return true;
        }

        public static async Task AddPlayerToRoomAsync(FirestoreDb db, string roomId, string uid, string name)
        {
            var playerRef = db.Collection("Rooms").Document(roomId).Collection("players").Document(uid);

            var playerData = new Dictionary<string, object>
            {
                { "name", name },
                { "score", 0 },
                { "ready", true }
            };

            await playerRef.SetAsync(playerData);
        }



        public static async Task<List<Player>> GetPlayersInRoomAsync(FirestoreDb db, string roomId)
        {
            var playersRef = db.Collection("Rooms").Document(roomId).Collection("players");
            var snapshot = await playersRef.GetSnapshotAsync();

            var players = new List<Player>();

            foreach (var doc in snapshot.Documents)
            {
                var data = doc.ToDictionary();
                players.Add(new Player
                {
                    Name = data.ContainsKey("name") ? data["name"].ToString() : "",
                    Score = data.ContainsKey("score") ? Convert.ToInt32(data["score"]) : 0,
                    Ready = data.ContainsKey("ready") ? Convert.ToBoolean(data["ready"]) : false
                });
            }

            return players;
        }



        // === C. QUESTION & ANSWERS ===

        public static async Task<Question> GetCurrentQuestionAsync(FirestoreDb db, string roomId)
        {
            // 1. Lấy document của phòng
            DocumentReference roomRef = db.Collection("Rooms").Document(roomId);
            DocumentSnapshot roomSnapshot = await roomRef.GetSnapshotAsync();

            if (!roomSnapshot.Exists)
                throw new Exception("Room không tồn tại.");

            var roomData = roomSnapshot.ToDictionary();

            // 2. Lấy chỉ số câu hỏi hiện tại
            int currentQuestion = roomData.ContainsKey("currentQuestion") ? Convert.ToInt32(roomData["currentQuestion"]) : 0;

            // 3. Lấy danh sách câu hỏi từ trường "questions"
            if (!roomData.ContainsKey("questions"))
                throw new Exception("Không có danh sách câu hỏi trong phòng.");

            var questionsData = roomData["questions"] as IEnumerable<object>;
            var questions = questionsData.Select(q =>
            {
                var qDict = q as Dictionary<string, object>;
                return new Question
                {
                    QuestionText = qDict["questionText"].ToString(),
                    Options = (qDict["options"] as IEnumerable<object>).Select(o => o.ToString()).ToList(),
                    CorrectAnswer = qDict["correctAnswer"].ToString()
                };
            }).ToList();

            // 4. Trả về câu hỏi tương ứng với chỉ số hiện tại
            if (currentQuestion >= 0 && currentQuestion < questions.Count)
                return questions[currentQuestion];
            else
                throw new Exception("Chỉ số câu hỏi không hợp lệ.");
        }


        public static async Task SubmitAnswerAsync(FirestoreDb db, string roomId, string uid, string answer)
        {
            // 1. Tạo tham chiếu đến subcollection answers trong room
            var answerRef = db.Collection("Rooms").Document(roomId).Collection("answers").Document(uid);

            // 2. Ghi dữ liệu câu trả lời vào Firestore
            var answerData = new Dictionary<string, object>
    {
            { "uid", uid },
            { "answer", answer },
            { "timestamp", Timestamp.GetCurrentTimestamp() }
    };

            await answerRef.SetAsync(answerData);
        }


        public static async Task CheckAnswerAndUpdateScoreAsync(FirestoreDb db, string roomId, string uid)
        {
            var roomRef = db.Collection("Rooms").Document(roomId);

            // 1. Lấy câu trả lời của người dùng
            var answerRef = roomRef.Collection("answers").Document(uid);
            var answerSnap = await answerRef.GetSnapshotAsync();
            if (!answerSnap.Exists) return;

            var answerData = answerSnap.ToDictionary();
            var submittedAnswer = answerData.ContainsKey("answer") ? answerData["answer"].ToString() : null;
            if (submittedAnswer == null) return;

            // 2. Lấy thông tin phòng để xác định câu hỏi hiện tại
            var roomSnap = await roomRef.GetSnapshotAsync();
            if (!roomSnap.Exists) return;

            var roomData = roomSnap.ToDictionary();
            var currentQuestionIndex = roomData.ContainsKey("currentQuestion") ? Convert.ToInt32(roomData["currentQuestion"]) : 0;
            var questionsRaw = roomData.ContainsKey("questions") ? roomData["questions"] as IEnumerable<object> : null;
            if (questionsRaw == null) return;

            var questionsList = questionsRaw.ToList();
            if (currentQuestionIndex >= questionsList.Count) return;

            var currentQuestionData = questionsList[currentQuestionIndex] as Dictionary<string, object>;
            var correctAnswer = currentQuestionData["correctAnswer"].ToString();

            // 3. So sánh
            if (submittedAnswer.Trim().Equals(correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                // 4. Nếu đúng → cộng điểm
                var playerRef = roomRef.Collection("players").Document(uid);
                var playerSnap = await playerRef.GetSnapshotAsync();

                if (playerSnap.Exists)
                {
                    var playerData = playerSnap.ToDictionary();
                    int currentScore = playerData.ContainsKey("score") ? Convert.ToInt32(playerData["score"]) : 0;

                    await playerRef.UpdateAsync("score", currentScore + 10); // ví dụ mỗi câu đúng được 10 điểm
                }
            }
        }


        public static async Task<List<PlayerScore>> GetFinalScoresAsync(FirestoreDb db, string roomId)
        {
            var playersRef = db.Collection("Rooms").Document(roomId).Collection("players");
            var snapshot = await playersRef.GetSnapshotAsync();

            var scores = new List<PlayerScore>();

            foreach (var doc in snapshot.Documents)
            {
                var data = doc.ToDictionary();

                scores.Add(new PlayerScore
                {
                    Name = data.ContainsKey("name") ? data["name"].ToString() : "",
                    Score = data.ContainsKey("score") ? Convert.ToInt32(data["score"]) : 0
                });
            }

            // Sắp xếp giảm dần theo điểm
            return scores.OrderByDescending(p => p.Score).ToList();
        }


        public static async Task DeleteRoomImmediatelyAsync(FirestoreDb db, string roomId)
        {
            var roomRef = db.Collection("Rooms").Document(roomId);
            var roomSnap = await roomRef.GetSnapshotAsync();

            if (!roomSnap.Exists)
                return;

            // 1. Xoá subcollections
            var playersSnap = await roomRef.Collection("players").GetSnapshotAsync();
            foreach (var doc in playersSnap.Documents)
            {
                await doc.Reference.DeleteAsync();
            }

            var answersSnap = await roomRef.Collection("answers").GetSnapshotAsync();
            foreach (var doc in answersSnap.Documents)
            {
                await doc.Reference.DeleteAsync();
            }

            // 2. Xoá chính document room
            await roomRef.DeleteAsync();
        }

        public static async Task ScheduleRoomDeletionAsync(FirestoreDb db, string roomId, int delaySeconds = 180)
        {
            // Chờ 3 phút (180 giây) rồi xoá phòng
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            await DeleteRoomImmediatelyAsync(db, roomId);
        }



        // === Utility ===

        private static string GenerateRoomId()
        {
            var random = new Random();
            return random.Next(1000, 10000).ToString(); // Tạo số có 4 chữ số
        }
    }
}