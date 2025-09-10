using System.ComponentModel;
using System.Reflection;
using static InputValidationHelper;


internal static class ChoiceHelper
{
    internal static TChoiceEnum ChoiceMenu<TChoiceEnum>(string question) where TChoiceEnum : struct, Enum
    {
        //przygotowanie niezbednych parametrow przed petla
        //konwersja enuma na array
        var options = Enum.GetValues<TChoiceEnum>()
            .Select(element => (
                index: Convert.ToInt32(element),
                label: typeof(TChoiceEnum)
                    .GetField(element.ToString())!
                    .GetCustomAttribute<DescriptionAttribute>()!
                    .Description
            ))
            .ToArray();
        //wyciagniecie minimalnej i maksymalnej wartosci enuma
        int minChoice = options.Select(option => option.index).Min();
        int maxChoice = options.Select(option => option.index).Max();

        //petla dziala az user wybierze prawidlowa metode
        while (true)
        {
            Console.WriteLine(question);

            //wypisanie opcji
            foreach (var option in options)
                Console.WriteLine($"{option.index}: {option.label}");

            var userChoice = Console.ReadLine();

            //walidacja inputu uzytkownika
            int? validatedChoice = ValidateIntInput(userChoice, minChoice, maxChoice);

            if (validatedChoice != null)
                //konwersja na odpowiedni typ enuma i zwrocenie wyboru
                return (TChoiceEnum)Enum.ToObject(typeof(TChoiceEnum), validatedChoice.Value);
        }
    }
}