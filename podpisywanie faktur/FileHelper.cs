using static InputValidationHelper;

internal static class FileHelper
{
    internal static string GetPathFromUser(string message)
    {
        while (true)
        {
            Console.WriteLine(message);
            var userInput = Console.ReadLine();

            var fullPath = ValidatePathInput(userInput);
            if (fullPath == null)
                continue;

            if (File.Exists(fullPath))
                return fullPath;

            Console.WriteLine($"Plik {fullPath} nie istnieje");
        }
    }



    internal static string BuildOutputPath(string inputPath)
    {
        return $"{Path.GetFileNameWithoutExtension(inputPath)}_signed.xml";
    }
}