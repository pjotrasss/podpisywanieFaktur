using static InputValidationHelper;
using System.ComponentModel;

internal static class FileHelper
{
    internal enum OutputPathMethod
    {
        [Description("Automatyczne utworzenie nazwy (dopisek _signed.xml)")] Auto = 1,
        [Description("Ręczny wybór nazwy")] Manual = 2
    }


    internal static string GetPathFromUser(string message, bool mustExist)
    {
        while (true)
        {
            //pobranie sciezki od usera
            Console.WriteLine(message);
            var userInput = Console.ReadLine();

            //konwersja na sciezke absolutna
            var fullPath = ValidatePathInput(userInput);
            if (fullPath == null)
                continue;

            //dla plikow wejsciowych - mustExist = true
            if (mustExist)
            {
                if (File.Exists(fullPath))
                    return fullPath;

                Console.WriteLine($"Plik {fullPath} nie istnieje");
                continue;
            }
            //dla plikow wyjsciowych - mustExist = false
            else
            {
                //walidacja rozszerzenia - dodaje jesli brak, podmienia na xml jesli bledne
                var extension = Path.GetExtension(fullPath);
                if (extension != ".xml")
                {
                    Console.WriteLine("Wykryto brak rozszerzenia pliku lub bledne rozszerzenie, zmiana na '.xml'");
                    fullPath = Path.ChangeExtension(fullPath, ".xml");
                }

                if (!File.Exists(fullPath))
                    return fullPath;

                Console.WriteLine($"Plik {fullPath} już istnieje, czy chcesz go nadpisać? (t/n)");

                if (!YesOrNo())
                    continue;
                return fullPath;
            }
        }
    }

    //dodac sprawdzenie czy plik istnieje
    internal static string AutoOutputPath(string inputPath)
    {
        var fullPath = $"{Path.GetFileNameWithoutExtension(inputPath)}_signed.xml";
        if (!File.Exists(fullPath))
            return fullPath;

        Console.WriteLine($"Plik {fullPath} już istnieje, czy chcesz go nadpisać? (t/n)");
        
        if (YesOrNo())
            return fullPath;
        return GetPathFromUser (inputPath, false);
    }
}