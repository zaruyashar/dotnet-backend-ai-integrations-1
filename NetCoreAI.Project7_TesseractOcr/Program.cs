using System.Linq.Expressions;
using Tesseract;

class Program()
{
    static void Main(string[] args)
    {
        Console.Write("Enter the path of the image for which you want Tesseract to extract chars: ");
        string imagePath = Console.ReadLine();
        Console.WriteLine();

        string tessDataPath = @"C:\tessdata";

        try
        {
            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.LstmOnly))
            {
                using (var img = Pix.LoadFromFile(imagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        string text = page.GetText();
                        Console.WriteLine("===== Characters extracted =====");
                        Console.WriteLine(text);
                        Console.WriteLine("===== End =====");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong... Error message: {ex.Message}");
        }

        Console.ReadLine();
    }
}