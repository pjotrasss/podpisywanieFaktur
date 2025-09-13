using static InputValidationHelper;
using System.ComponentModel;

internal static class FileHelper
{
    internal enum OutputPathMethod
    {
        [Description("Automatyczne utworzenie nazwy (dopisek _signed.xml)")] Auto = 1,
        [Description("Ręczny wybór nazwy")] Manual = 2,
        [Description("Automatyczna nazwa w wybranym folderze")] AutoToFolder = 3
    }

    internal enum InputMethod
    {
        [Description("Podpisanie pojedynczego pliku")] SingleFile = 1,
        [Description("Podpisanie wszystkich plików z folderu")] FullDir = 2
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
                
                //sprawdzenie czy sciezka jest wolna
                if (EnsureFreePath(fullPath))
                    return fullPath;
                continue;
            }
        }
    }


    internal static string AutoOutputPath(string inputPath, bool toDir, string? outputDir)
    {
        string directory;

        if (outputDir != null)
        {
            directory = outputDir;
        }
        else if (toDir)
        {
            //pobranie katalogu od usera
            directory = GetDirFromUser("Podaj folder docelowy");
        }
        else
        {
            //wyciagniecie nazwy folderu
            directory = Path.GetDirectoryName(inputPath)
                ?? Path.GetPathRoot(inputPath)!;
        }

        //wyciagniecie nazwy pliku bez rozszerzenia
        var fileName = Path.GetFileNameWithoutExtension(inputPath);
        //polaczenie spowrotem i dodanie rozszerzenia
        var fullPath = Path.Combine(directory!, $"{fileName}_signed.xml");

        //sprawdzenie czy sciezka jest wolna
        if (EnsureFreePath(fullPath))
            return fullPath;
        
        //plik istnieje i user nie zgodzil sie na nadpisanie
        return GetPathFromUser("Podaj nową nazwę podpisanej faktury i naciśnij Enter", false);
    }


    private static bool EnsureFreePath(string fullPath)
    {
        //plik nie istnieje
        if (!File.Exists(fullPath))
            return true;

        //plik juz istnieje
        Console.WriteLine($"Plik {fullPath} już istnieje, czy chcesz go nadpisać? (t/n)");
        Console.WriteLine("Jeśli wybierzesz 'n', zostaniesz poproszony o podanie innej nazwy pliku.");
        
        //zwrocenie czy uzytkownik pozwolil na nadpisanie        
        if (YesOrNo())
            return true;
        return false;
    }


    internal static string GetDirFromUser(string message)
    {
        while (true)
        {
            //zapytanie o sciezke do katalogu
            Console.WriteLine(message);
            var userInput = Console.ReadLine();

            //walidacja prawidlowego inputu i przerobienie na sciezke absolutna
            var fullPath = ValidatePathInput(userInput);
            if (fullPath == null)
                continue;

            //sprawdzenie czy katalog istnieje
            if (Directory.Exists(fullPath))
                return fullPath;

            //pytanie czy stworzyc folder (jesli nie istnieje)
            Console.WriteLine($"Katalog {fullPath} nie istnieje, chcesz go utworzyć? (t/n)");
            Console.WriteLine("Jeśli wybierzesz 'n', zostaniesz poproszony o podanie innego folderu");

            //jesli nie - powrot do poczatku petli
            if(!YesOrNo())
                continue;
            //jesli tak - utworzenie katalogu
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }
    }


    internal static string PrepareOutputDirectory(string inputDirectory)
    {
        var parentPath = Directory.GetParent(inputDirectory)!.FullName;
        var childDir = new DirectoryInfo(inputDirectory).Name;

        var baseName = $"{childDir}_signed";
        var outputDirectory = Path.Combine(parentPath, baseName);

        int counter = 1;
        while (Directory.Exists(outputDirectory))
        {
            outputDirectory = Path.Combine(parentPath, $"{baseName}_{counter}");
            counter++;
        }

        Directory.CreateDirectory(outputDirectory);
        Console.WriteLine($"Utworzono katalog wyjściowy {outputDirectory}");

        return outputDirectory;
    }
}