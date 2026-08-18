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
            Console.Write("Sentiment analysis in progress...");
            Console.WriteLine();

            string sentiment = await AnalyzeSentiment(userInput);
            Console.WriteLine($"Result: {sentiment}");
        }
    }

    static async Task<string> AnalyzeSentiment(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You're a helpful AI assistant expert in content sentiment analysis. You dig through a given chunk of text in light of its context, and categorize the text as Positive, Negative, or Neutral." },
                    new { role = "user", content = $"How would you classify the dominant sentiment from this text: \"{text}\"? Remember that your options are: Positive, Negative, Neutral." }
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