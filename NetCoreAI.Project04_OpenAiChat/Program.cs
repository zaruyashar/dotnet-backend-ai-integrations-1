using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var apiKey = config["OpenAI:ApiKey"];

        Console.WriteLine("Please type in your question for the agent: ");
        var prompt = Console.ReadLine();

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You're a helpful and truthful assistant." },
                new { role = "user", content = prompt }
            },
            max_tokens = 500
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseString);

                var answer = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                Console.WriteLine("The answer from OpenAI: ");
                Console.WriteLine(answer);
            }
            else
            {
                Console.WriteLine($"Something went wrong. Error code: {response.StatusCode}");
                Console.WriteLine(responseString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong. Error code: {ex.Message}");
        }
    }
}