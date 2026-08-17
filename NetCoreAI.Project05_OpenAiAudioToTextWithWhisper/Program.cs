using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var apiKey = config["OpenAI:ApiKey"];
        string audioFilePath = "Charlie Chaplin - Final Speech from The Great Dictator.mp3";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var form = new MultipartFormDataContent();
            var audioContent = new ByteArrayContent(File.ReadAllBytes(audioFilePath));

            audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");
            form.Add(audioContent, "file", Path.GetFileName(audioFilePath));

            form.Add(new StringContent("whisper-1"), "model");


            Console.WriteLine("Processing audio file. Please hold...");

            var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("The audio transcription: ");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Something went wrong. Error code: {response.StatusCode}");
                Console.Write(await response.Content.ReadAsStringAsync());
            }
        }
    }
}