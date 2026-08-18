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

        Console.WriteLine("Enter the text you want to be analyzed: ");
        string userInput = Console.ReadLine();

        if (!string.IsNullOrEmpty(userInput))
        {
            Console.WriteLine();
            Console.Write("Advanced sentiment analysis in progress...");
            Console.WriteLine();

            string sentiment = await AnalyzeSentimentInDetail(userInput);
            Console.WriteLine($"Result: {sentiment}");
        }
    }

    static async Task<string> AnalyzeSentimentInDetail(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You're a helpful AI assistant expert in advanced content sentiment analysis. Your responses must be in JSON format. Identify the sentiment scores (0-100%) for the following emotions: Joy, Sadness, Anger, Fear, Surprise, Neutral." },
                    new { role = "user", content = $"Analyze the following text and return a JSON object containing sentiment scores for that text: \"{text}\"" }
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            string responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                return result.choices[0].message.content.ToString();
            }
            else
            {
                Console.WriteLine("An error occurred: ", responseJson);
                return "Error"; // So that the method returns a str val
            }
        }
    }
}