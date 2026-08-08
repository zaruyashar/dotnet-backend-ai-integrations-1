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

        Console.WriteLine("Enter the text you want to convert to speech: ");
        string userInput = Console.ReadLine();

        if (!string.IsNullOrEmpty(userInput))
        {
            Console.WriteLine("Generating audio file...");
            bool isSuccess = await GenerateSpeech(userInput);

            if (isSuccess)
            {
                Console.WriteLine("The audio file 'output.mp3' is now generated.");
                // Auto-play the generated file
                System.Diagnostics.Process.Start("explorer.exe", "output.mp3");
            }
        }
    }

    // Methods
    static async Task<bool> GenerateSpeech(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "tts-1",
                input = text,
                voice = "alloy"
                // Other voice options: fable, shimmer, echo, onyx, nova
            };

            string json = JsonConvert.SerializeObject(requestBody);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/audio/speech", content);

            if (response.IsSuccessStatusCode)
            {
                byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync("output.mp3", audioBytes);
                return true;
            }
            else
            {
                Console.WriteLine($"Something went wrong. Status Code: {response.StatusCode}");
                return false;
            }
        }
    }
}