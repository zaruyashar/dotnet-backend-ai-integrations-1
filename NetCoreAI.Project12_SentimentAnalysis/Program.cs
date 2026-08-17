using Microsoft.Extensions.Configuration;

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

        }
    }
}