using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    private static string apiKey;
    private static string rssFeedUrl = "https://acikgazete.com/feed/";

    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        apiKey = config["OpenAI:ApiKey"];

        Console.WriteLine("Fetching News...");
        List<string> articles = await FetchLatestNews(10);

        foreach(var article in articles)
        {
            Console.WriteLine("Summarizing News Content...");
            string summary = await SummarizeNewsArticle(article);
            Console.WriteLine("AI Summary:");
            Console.WriteLine(summary);
            Console.WriteLine("=========================================");
        }


        // Methods
        static async Task<List<string>> FetchLatestNews(int count)
        {
            var client = new HttpClient();
            string rssContent = await client.GetStringAsync(rssFeedUrl);

            XDocument doc = XDocument.Parse(rssContent);
            var items = doc.Descendants("item").Take(count);

            List<string> articles = items.Select(item =>
            {
                string title = item.Element("title")?.Value ?? "";
                string description = item.Element("description")?.Value ?? "";
                return $"{title} - {description}";
            }).ToList();

            return articles;
        }

        static async Task<string> SummarizeNewsArticle(string newsContent)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4-turbo",
                messages = new[]
                    {
                    new { role = "system", content = "You're specialized in news content summarization." },
                    new { role = "user", content = "Summarize this news content in 3 sentences: " + newsContent }
                },
                max_tokens = 500
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);
            
            string responseContent = await response.Content.ReadAsStringAsync();

            JsonDocument doc = JsonDocument.Parse(responseContent);

            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}