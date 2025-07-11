using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WordUp.Services
{
    internal class AI
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string apiKey = "AIzaSyA5Tkm14u7AerdvkBUm5kV68rfJH_CkncA";
        private const string model = "gemini-2.0-flash";
        private static readonly string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        public static async Task<string> AskGeminiAsync(string userInput)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = userInput }
                        }
                    }
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WordUpApp/1.0");

                var response = await httpClient.PostAsync(apiUrl, content);
                string responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"Lỗi API: {response.StatusCode}\n{responseText}";
                }

                dynamic result = JsonConvert.DeserializeObject(responseText);
                return result?.candidates?[0]?.content?.parts?[0]?.text ?? "⚠️ Không có phản hồi từ AI.";
            }
            catch (Exception ex)
            {
                return "Lỗi gọi API: " + ex.Message;
            }
        }
    }
}