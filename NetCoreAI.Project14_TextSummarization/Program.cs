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
            Console.Write("Text summarization in progress...");
            Console.WriteLine();

            string shortSummary = await SummarizeTextContent(userInput, "short");
            string mediumLengthSummary = await SummarizeTextContent(userInput, "medium");
            string detailedSummary = await SummarizeTextContent(userInput, "detailed");

            Console.WriteLine("========== Your Summaries ==========");
            Console.WriteLine($"*** Short Summary *** \n {shortSummary}");
            Console.WriteLine("====================================");
            Console.WriteLine($"*** Medium Length Summary *** \n {mediumLengthSummary}");
            Console.WriteLine("====================================");
            Console.WriteLine($"*** Detailed Summary *** \n {detailedSummary}");
        }
    }

    static async Task<string> SummarizeTextContent(string text, string level)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            string instruction = level switch
            {
                "short" => "Summarize this text in 1-2 sentences.",
                "medium" => "Summarize this text in 4-5 sentences.",
                "detailed" => "Summarize this text in a detailed but coherent and concise manner. No longer than 10 sentences.",
                _ => "Summarize this text"
            };

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You're an AI assistant that summarizes text in different levels of detail and complexity. The 3 levels of your summarization operations are: Short, Medium, Detailed. Only work with whatever text is passed through as input--don't add something of your own just to present a response. In that case, let the user know that their text is too short or whatever the situation is, and stop execution." },
                    new { role = "user", content = $"{instruction}\n\n{text}" }
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
                Console.WriteLine($"An error occurred: {responseJson}");
                return "Error"; // So that the method returns a str val
            }
        }
    }
}