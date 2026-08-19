using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

class Program
{
    private static string apiKey;
    private static string imagePath = "";

    // Request DTOs — named types instead of nested anonymous objects---the tutorial used too many nested "new"s
    record VisionRequest(AnnotateRequest[] requests);
    record AnnotateRequest(ImagePayload image, Feature[] features);
    record ImagePayload(string content);
    record Feature(string type, int maxResults);

    static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        apiKey = config["OpenAI:ApiKey"];

        Console.WriteLine("Image content analysis with Google Cloud Vision is in progress...");

        string response = await DetectObjectsFromTheImage(imagePath);
        Console.WriteLine("========== Detected Objects ==========");
        Console.WriteLine(response);
    }

    static async Task<string> DetectObjectsFromTheImage(string imagePath)
    {
        using var client = new HttpClient();
        string apiUrl = $"https://vision.googleapis.com/v1/images:annotate?key={apiKey}";

        byte[] imageBytes = File.ReadAllBytes(imagePath);
        string base64Image = Convert.ToBase64String(imageBytes);

        var requestBody = new VisionRequest(
            requests: new[]
            {
                new AnnotateRequest(
                    image: new ImagePayload(base64Image),
                    features: new[] { new Feature("LABEL_DETECTION", 10) }
                )
            }
        );

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(apiUrl, jsonContent);
        return await response.Content.ReadAsStringAsync();
    }
}