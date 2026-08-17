using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var apiKey = config["OpenAI:ApiKey"];

        Console.Write("Enter your prompt to generate your desired image: ");
        string prompt = Console.ReadLine();

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                // Dall-E deprecated on May 2026. Switching to gpt-image instead.
                model = "gpt-image-1",
                prompt = prompt,
                n = 1,
                size = "1024x1024"
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/images/generations", content);
            string responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = JObject.Parse(responseString);
                string base64Image = json["data"][0]["b64_json"].ToString();

                byte[] imageBytes = Convert.FromBase64String(base64Image);

                string fileName = $"generated_image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(AppContext.BaseDirectory, fileName);

                await File.WriteAllBytesAsync(filePath, imageBytes);

                Console.WriteLine($"Image saved to: {filePath}");
            }
            else
            {
                Console.WriteLine($"Something went wrong. Error code: {response.StatusCode}");
                Console.WriteLine(responseString);
            }
        }
    }
}