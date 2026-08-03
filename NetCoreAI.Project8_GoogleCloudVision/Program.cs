using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Google.Cloud.Vision.V1;

class Program
{
    static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        Console.Write("Enter the image file path: ");
        Console.WriteLine();
        string imagePath = Console.ReadLine();

        string credentialPath = config["GoogleCloud:CredentialsPath"];

        if (string.IsNullOrEmpty(credentialPath))
        {
            Console.WriteLine("Error: GoogleCloud:CredentialsPath not found in user secrets.");
            return;
        }

        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

        try
        {
            var client = ImageAnnotatorClient.Create();

            var image = Image.FromFile(imagePath);
            var response = client.DetectText(image);
            Console.WriteLine("===== Text from the image =====");
            Console.WriteLine();

            var textAnnotations = response.ToList();
            if (textAnnotations.Count > 0)
            {
                Console.WriteLine(textAnnotations[0].Description);
            }
            else
            {
                Console.WriteLine("(No text detected)");
            }

            Console.WriteLine("===== End =====");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong... Error message: {ex.Message}");
        }

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}