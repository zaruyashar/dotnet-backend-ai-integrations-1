using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

class Program
{
    private static string apiKey;

    static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        apiKey = config["OpenAI:ApiKey"];


        Console.WriteLine("Select a genre:\n1 - Adventure,\n2 - Horror,\n3 - Sci-Fi,\n4 - Fantasy,\n5 - Comedy");
        int genre = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Who's the main character?");
        string mainCharacter = Console.ReadLine();

        Console.WriteLine("Where do you want the story to take place?");
        string setting = Console.ReadLine();

        Console.WriteLine("Choose story length:\n1 - Short,\n2 - Medium Length,\n3 - Long");
        int storyLength = Convert.ToInt32(Console.ReadLine());

        string systemPrompt = $"Write a {storyLength} {genre} story, having {mainCharacter} as the main character. The story will take place in {setting}, and will have an introduction, body, and a conclusion. Note on genre selection---User inputted numbers correspond to the following: 1 - Adventure, 2 - Horror, 3 - Sci-Fi, 4 - Fantasy, 5 - Comedy. Note on story length---numbers mean the following: 1 - Short, 2 - Medium Length, 3 - Long";


        string story = await ComposeAStory(systemPrompt);
        Console.WriteLine();
        Console.WriteLine("========== AI-Generated Story ==========");
        Console.WriteLine(story);


        // Methods
        static async Task<string> ComposeAStory(string systemPrompt)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4-turbo",
                messages = new[]
                    {
                    new { role = "system", content = "You're a creative writer expert in storry compositions." },
                    new { role = "user", content = systemPrompt }
                },
                max_tokens = 1000
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);

            string responseContent = await response.Content.ReadAsStringAsync();
            
            JsonDocument doc = JsonDocument.Parse(responseContent);

            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}