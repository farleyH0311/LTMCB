using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WordUp.Services
{
    public class FirebaseProgressService
    {
        // 🔧 Đổi URL này thành URL Realtime Database thực tế của bạn nếu khác
        private const string DatabaseUrl = "https://ltm-wu-default-rtdb.asia-southeast1.firebasedatabase.app";

        /// <summary>
        /// Lưu tiến độ học (phần trăm) cho một bài học của người dùng
        /// </summary>
        public async Task SaveProgressAsync(string userId, string lessonId, int percent)
        {
            try
            {
                using var client = new HttpClient();
                string path = $"{DatabaseUrl}/progress/{userId}/{lessonId}.json";

                var json = JsonSerializer.Serialize(percent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(path, content);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Lỗi khi lưu tiến độ: {response.StatusCode} - {error}");
                }
                else
                {
                    Console.WriteLine("Tiến độ đã được lưu thành công.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi kết nối Firebase: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy số lượng bài học đã hoàn thành của người dùng
        /// </summary>
        public async Task<int> LoadCompletedLessonsAsync(string userId)
        {
            try
            {
                using var client = new HttpClient();
                string path = $"{DatabaseUrl}/progress/{userId}.json";

                var response = await client.GetAsync(path);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Lỗi khi tải số bài đã học: {response.StatusCode}");
                    return 0;
                }

                string json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(json))
                {
                    var progressData = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                    return progressData?.Count ?? 0;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi kết nối Firebase: {ex.Message}");
                return 0;
            }
        }

    }
}
