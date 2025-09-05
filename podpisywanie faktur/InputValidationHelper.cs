internal class InputValidationHelper
{
    internal static int? ValidateIntInput(string? userInput, int bottomLim, int topLim)
    {
        //walidacja poprawnosci wyboru
        if (userInput == null)
        {
            Console.WriteLine ("Brak wyboru");
            return null;
        }

        if (!int.TryParse(userInput, out int userInputInt))
        {
            Console.WriteLine("Niepoprawny typ danych - oczekiwano liczby");
            return null;
        }

        if (userInputInt < bottomLim || userInputInt > topLim)
        {
            Console.WriteLine("Wybór spoza zakresu");
            return null;
        }

        return userInputInt;
    }

    internal static string? ValidatePathInput(string? userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            Console.WriteLine("Nazwa pliku nie może być pusta");
            return null;
        }

        return Path.GetFullPath(userInput);
    }
}