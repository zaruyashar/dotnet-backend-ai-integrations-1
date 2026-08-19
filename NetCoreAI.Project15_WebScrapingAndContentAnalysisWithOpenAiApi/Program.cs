using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

class Program
{
    private static string apiKey;

    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        apiKey = config["OpenAI:ApiKey"];

        Console.WriteLine("Enter website URL for analysis: ");
        string inputUrl = Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("========== Website Content ==========");

        string webContent = ScrapeTextFromWebsite(inputUrl);
        await AnalyzeWithAi(webContent, "Web Sayfasının İçeriği");


        // Methods
        static string ScrapeTextFromWebsite(string inpurUrl)
        {
            var web = new HtmlWeb();
            var doc = web.Load(inpurUrl);

            var bodyText = doc.DocumentNode.SelectSingleNode("//body")?.InnerText;

            return bodyText ?? "Page content couldn't be read.";
        }

        static async Task AnalyzeWithAi(string text, string sourceType)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You're an AI assistant that analyzes user inputted text/url and summarizes its content, formulating your response only in Turkish." },
                        new { role = "user", content = $"Analyze and summarize the following {sourceType} as per your task-specific instructions: \n\n{text}" }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                string responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                    Console.WriteLine($"\n AI Analizi ({sourceType}): \n {result.choices[0].message.content}");
                }
                else
                {
                    Console.WriteLine($"An error occurred: {responseJson}");
                }
            }
        }
    }
}