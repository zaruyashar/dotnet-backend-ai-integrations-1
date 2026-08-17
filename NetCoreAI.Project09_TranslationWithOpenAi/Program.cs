using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    private static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        Console.WriteLine("İngilizceye çevrilmesini istediğiniz metni girin: ");
        string userInput = Console.ReadLine();

        var apiKey = config["OpenAI:ApiKey"];

        string translatedText = await TranslateUserInputIntoEnglish(userInput, apiKey);

        if (!string.IsNullOrEmpty(translatedText))
        {
            Console.WriteLine();
            Console.Write($"Girdiğiniz metnin İngilizce çevirisi: {translatedText}");
            Console.WriteLine();
        }
        else
        {
            Console.Write("Something went wrong.");
        }
    }


    // Methods
    private static async Task<string> TranslateUserInputIntoEnglish(string text, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("API key is missing. Check your user secrets configuration.");
            return null;
        }

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You're a helpful translation expert assisting the user." },
                new { role = "user", content = $"Please translate the following text into proper English as though it was originally formulated in English by a native speaker, without omitting any critical information: {text}" }
            }
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API call failed with status {response.StatusCode}: {responseString}");
                    return null;
                }

                dynamic responseObject = JsonConvert.DeserializeObject(responseString);

                if (responseObject?.choices == null || responseObject.choices.Count == 0)
                {
                    Console.WriteLine($"Unexpected response format: {responseString}");
                    return null;
                }

                string translation = responseObject.choices[0].message.content;
                return translation;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred. Details: {ex.Message}");
                return null;
            }
        }
    }
}
