using System.Speech.Synthesis;

class Program
{
    static void Main(string[] args)
    {
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        speechSynthesizer.Volume = 100;
        speechSynthesizer.Rate = 1; // The speed of speech.
        speechSynthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);

        Console.WriteLine("Enter the text you want to convert to speech: ");
        string userInput = Console.ReadLine();

        if (!string.IsNullOrEmpty(userInput))
        {
            speechSynthesizer.Speak(userInput);
        }
    }
}